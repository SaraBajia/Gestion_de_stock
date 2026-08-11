using System;
using System.Collections.ObjectModel;
using System.Windows;
using MySql.Data.MySqlClient;
using WpfApp1.mvvm.Common;
using WpfApp1.mvvm.Services;

namespace WpfApp1.mvvm.ViewModels
{
    public class Window7ViewModel : ViewModelBase
    {
        private string _selectedModele;
        public string SelectedModele
        {
            get => _selectedModele;
            set
            {
                if (SetProperty(ref _selectedModele, value))
                {
                    MettreAJourPieces(value);
                }
            }
        }

        private string _selectedPiece;
        public string SelectedPiece { get => _selectedPiece; set => SetProperty(ref _selectedPiece, value); }

        public ObservableCollection<string> PieceOptions { get; } = new ObservableCollection<string>();

        public Action<Window> OuvrirFenetreEtCacherActuelle;

        public RelayCommand EnregistrerCommand { get; }
        public RelayCommand RetourCommand { get; }

        public Window7ViewModel()
        {
            EnregistrerCommand = new RelayCommand(_ => Enregistrer());
            RetourCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window4()));
        }

        private void MettreAJourPieces(string selectedModele)
        {
            PieceOptions.Clear();
            SelectedPiece = null;

            if (string.IsNullOrEmpty(selectedModele)) return;

            if (selectedModele == "B9100")
            {
                PieceOptions.Add("Photorécepteur");
                PieceOptions.Add("Cartouche de nettoyage");
                PieceOptions.Add("Bac à déchets");
            }
            else if (selectedModele == "C8155" || selectedModele == "B7130")
            {
                PieceOptions.Add("Photorécepteur");
                PieceOptions.Add("Bac à déchets");
            }
        }

        private void Enregistrer()
        {
            if (string.IsNullOrEmpty(SelectedModele) || string.IsNullOrEmpty(SelectedPiece))
            {
                MessageBox.Show("Veuillez sélectionner un modèle et une pièce !");
                return;
            }

            string modele = SelectedModele;
            string piece = SelectedPiece;

            using (var con = DatabaseService.GetConnection())
            {
                try
                {
                    con.Open();

                    string checkQuery =
                        @"SELECT id, quantite
                  FROM piece_de_rechange
                  WHERE modele = @modele
                  AND piece = @piece";

                    var checkCmd = new MySqlCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@modele", modele);
                    checkCmd.Parameters.AddWithValue("@piece", piece);

                    var reader = checkCmd.ExecuteReader();

                    if (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["id"]);
                        int quantite = Convert.ToInt32(reader["quantite"]);

                        reader.Close();

                        string updateQuery =
                            @"UPDATE piece_de_rechange
                      SET quantite = @quantite, date_ajout = NOW()
                      WHERE id = @id";

                        var updateCmd = new MySqlCommand(updateQuery, con);
                        updateCmd.Parameters.AddWithValue("@quantite", quantite + 1);
                        updateCmd.Parameters.AddWithValue("@id", id);

                        updateCmd.ExecuteNonQuery();

                        EnregistrerMouvement(con, "entree", "piece_de_rechange", 1);

                        MessageBox.Show("Quantité mise à jour avec succès !");
                    }
                    else
                    {
                        reader.Close();

                        string insertQuery =
                            @"INSERT INTO piece_de_rechange
                    (modele, piece, quantite, id_users, date_ajout)
                    VALUES
                    (@modele, @piece, @quantite, @id_users, NOW())";

                        var insertCmd = new MySqlCommand(insertQuery, con);

                        insertCmd.Parameters.AddWithValue("@modele", modele);
                        insertCmd.Parameters.AddWithValue("@piece", piece);
                        insertCmd.Parameters.AddWithValue("@quantite", 1);
                        insertCmd.Parameters.AddWithValue("@id_users", Class1.IdUtilisateur);

                        insertCmd.ExecuteNonQuery();

                        EnregistrerMouvement(con, "entree", "piece_de_rechange", 1);

                        MessageBox.Show("Pièce de rechange enregistrée avec succès !");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur : " + ex.Message);
                }
            }
        }

        private void EnregistrerMouvement(MySqlConnection con, string typeMvt, string tableSource, int quantite)
        {
            string mvtQuery = @"INSERT INTO mvt_stock (type_mvt, table_source, quantite, date_mvt) 
                               VALUES (@typeMvt, @tableSource, @quantite, NOW())";

            using (var mvtCmd = new MySqlCommand(mvtQuery, con))
            {
                mvtCmd.Parameters.AddWithValue("@typeMvt", typeMvt);
                mvtCmd.Parameters.AddWithValue("@tableSource", tableSource);
                mvtCmd.Parameters.AddWithValue("@quantite", quantite);
                mvtCmd.ExecuteNonQuery();
            }
        }
    }
}