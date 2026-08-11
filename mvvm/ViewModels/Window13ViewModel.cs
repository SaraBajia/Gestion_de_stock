using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using MySql.Data.MySqlClient;
using WpfApp1.mvvm.Common;
using WpfApp1.mvvm.Models;
using WpfApp1.mvvm.Services;

namespace WpfApp1.mvvm.ViewModels
{
    public class Window13ViewModel : ViewModelBase
    {
        private string _filtreActuel = "";

        public ObservableCollection<CommandeItem> Commandes { get; } = new ObservableCollection<CommandeItem>();

        public Action<Window> OuvrirFenetreEtFermerActuelle;

        public RelayCommand FiltreCommand { get; }
        public RelayCommand EnregistrerCommand { get; }
        public RelayCommand DeconnexionCommand { get; }
        public RelayCommand RetourCommand { get; }

        public Window13ViewModel()
        {
            FiltreCommand = new RelayCommand(p => AppliquerFiltre(p as string));
            EnregistrerCommand = new RelayCommand(p => Enregistrer(p as CommandeItem));
            DeconnexionCommand = new RelayCommand(_ => OuvrirFenetreEtFermerActuelle?.Invoke(new Window1()));
            RetourCommand = new RelayCommand(_ => OuvrirFenetreEtFermerActuelle?.Invoke(new Window10()));

            ChargerCommandes();
        }

        private void AppliquerFiltre(string filtre)
        {
            _filtreActuel = filtre ?? "";
            ChargerCommandes();
        }

        private void ChargerCommandes()
        {
            try
            {
                Commandes.Clear();

                string sql = "SELECT id, type_pc, service, demandeur, beneficiaire, commentaire, statut, date_commande FROM commande";
                if (!string.IsNullOrEmpty(_filtreActuel))
                    sql += " WHERE statut = @statut";
                sql += " ORDER BY date_commande DESC";

                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        if (!string.IsNullOrEmpty(_filtreActuel))
                            cmd.Parameters.AddWithValue("@statut", _filtreActuel);

                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                string statutActuel = r["statut"] == DBNull.Value ? "En attente" : r["statut"].ToString();

                                var item = new CommandeItem
                                {
                                    Id = Convert.ToInt32(r["id"]),
                                    TypePc = r["type_pc"].ToString(),
                                    Service = r["service"].ToString(),
                                    Demandeur = r["demandeur"].ToString(),
                                    Beneficiaire = r["beneficiaire"].ToString(),
                                    Commentaire = r["commentaire"] == DBNull.Value ? "" : r["commentaire"].ToString(),
                                    Statut = statutActuel,
                                    DateCommande = Convert.ToDateTime(r["date_commande"]).ToString("dd/MM/yyyy HH:mm")
                                };
                                item.NouveauStatut = statutActuel;
                                Commandes.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des commandes : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Enregistrer(CommandeItem item)
        {
            if (item == null) return;

            string ancienStatut = item.Statut;
            string nouveauStatut = item.NouveauStatut;
            string etiquetteRetiree = null;

            try
            {
                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            if (nouveauStatut == "Validée" && ancienStatut != "Validée")
                            {
                                etiquetteRetiree = TrouverEtRetirerMateriel(conn, transaction, item.TypePc);

                                if (etiquetteRetiree == null)
                                {
                                    transaction.Rollback();
                                    MessageBox.Show(
                                        $"Impossible de valider la commande : aucun \"{item.TypePc}\" disponible en stock actuellement.",
                                        "Rupture de stock",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
                                    return;
                                }

                                using (var cmdMvt = new MySqlCommand(
                                    @"INSERT INTO mvt_stock (type_mvt, table_source, quantite, date_mvt)
                                      VALUES ('sortie', 'materiel', 1, NOW())", conn, transaction))
                                {
                                    cmdMvt.ExecuteNonQuery();
                                }
                            }

                            using (var cmd = new MySqlCommand("UPDATE commande SET statut = @statut WHERE id = @id", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@statut", nouveauStatut);
                                cmd.Parameters.AddWithValue("@id", item.Id);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(etiquetteRetiree))
                {
                    MessageBox.Show(
                        $"Statut mis à jour avec succès !\nArticle retiré du stock : {etiquetteRetiree} ({item.TypePc})",
                        "Succès",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Statut mis à jour avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                ChargerCommandes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la mise à jour : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string TrouverEtRetirerMateriel(MySqlConnection conn, MySqlTransaction transaction, string typePc)
        {
            string etiquette = null;

            using (var cmdSelect = new MySqlCommand(
                "SELECT etiquette FROM materiel WHERE LOWER(type_materiel) = LOWER(@type) LIMIT 1", conn, transaction))
            {
                cmdSelect.Parameters.AddWithValue("@type", typePc);
                var result = cmdSelect.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    etiquette = result.ToString();
            }

            if (etiquette == null)
                return null;

            using (var cmdDelete = new MySqlCommand("DELETE FROM materiel WHERE etiquette = @etiquette", conn, transaction))
            {
                cmdDelete.Parameters.AddWithValue("@etiquette", etiquette);
                cmdDelete.ExecuteNonQuery();
            }

            return etiquette;
        }
    }
}