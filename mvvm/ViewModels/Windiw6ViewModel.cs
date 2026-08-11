using System;
using System.Windows;
using MySql.Data.MySqlClient;
using WpfApp1.mvvm.Common;
using WpfApp1.mvvm.Services;

namespace WpfApp1.mvvm.ViewModels
{
    public class Window6ViewModel : ViewModelBase
    {
        private string _selectedModele;
        public string SelectedModele { get => _selectedModele; set => SetProperty(ref _selectedModele, value); }

        private string _selectedCouleur;
        public string SelectedCouleur
        {
            get => _selectedCouleur;
            set
            {
                if (SetProperty(ref _selectedCouleur, value))
                {
                    Reference = value switch
                    {
                        "Black" => "006R01758",
                        "Cyan" => "006R01759",
                        "Magenta" => "006R01760",
                        "Yellow" => "006R01761",
                        _ => ""
                    };
                }
            }
        }

        private string _reference;
        public string Reference { get => _reference; set => SetProperty(ref _reference, value); }

        public Action<Window> OuvrirFenetreEtCacherActuelle;

        public RelayCommand EnregistrerCommand { get; }
        public RelayCommand RetourCommand { get; }

        public Window6ViewModel()
        {
            EnregistrerCommand = new RelayCommand(_ => Enregistrer());
            RetourCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window4()));
        }

        private void Enregistrer()
        {
            if (string.IsNullOrEmpty(SelectedModele) || string.IsNullOrEmpty(SelectedCouleur))
            {
                MessageBox.Show("Veuillez sélectionner un modèle et une couleur !");
                return;
            }

            string modele = SelectedModele;
            string couleur = SelectedCouleur;
            string reference = Reference;

            using (var con = DatabaseService.GetConnection())
            {
                try
                {
                    con.Open();

                    string checkQuery =
                        @"SELECT id, quantite
                  FROM consommable
                  WHERE modele = @modele
                  AND couleur = @couleur
                  AND reference = @reference";

                    var checkCmd = new MySqlCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@modele", modele);
                    checkCmd.Parameters.AddWithValue("@couleur", couleur);
                    checkCmd.Parameters.AddWithValue("@reference", reference);

                    var reader = checkCmd.ExecuteReader();

                    if (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["id"]);
                        int quantite = Convert.ToInt32(reader["quantite"]);

                        reader.Close();

                        string updateQuery =
                            @"UPDATE consommable
                      SET quantite = @quantite, date_ajout = NOW()
                      WHERE id = @id";

                        var updateCmd = new MySqlCommand(updateQuery, con);
                        updateCmd.Parameters.AddWithValue("@quantite", quantite + 1);
                        updateCmd.Parameters.AddWithValue("@id", id);

                        updateCmd.ExecuteNonQuery();

                        EnregistrerMouvement(con, "entree", "consommable", 1);

                        MessageBox.Show("Quantité mise à jour avec succès !");
                    }
                    else
                    {
                        reader.Close();

                        string insertQuery =
                            @"INSERT INTO consommable
                    (modele, reference, couleur, quantite, id_users, date_ajout)
                    VALUES
                    (@modele, @reference, @couleur, @quantite, @id_users, NOW())";

                        var insertCmd = new MySqlCommand(insertQuery, con);

                        insertCmd.Parameters.AddWithValue("@modele", modele);
                        insertCmd.Parameters.AddWithValue("@reference", reference);
                        insertCmd.Parameters.AddWithValue("@couleur", couleur);
                        insertCmd.Parameters.AddWithValue("@quantite", 1);
                        insertCmd.Parameters.AddWithValue("@id_users", Class1.IdUtilisateur);

                        insertCmd.ExecuteNonQuery();

                        EnregistrerMouvement(con, "entree", "consommable", 1);

                        MessageBox.Show("Consommable enregistré avec succès !");
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