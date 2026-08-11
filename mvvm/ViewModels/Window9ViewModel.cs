using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using WpfApp1.mvvm.Common;
using WpfApp1.mvvm.Services;

namespace WpfApp1.mvvm.ViewModels
{
    public class Window9ViewModel : ViewModelBase
    {
        private string _fichierJointPath = string.Empty;

        private string _userNameHeader;
        public string UserNameHeader { get => _userNameHeader; set => SetProperty(ref _userNameHeader, value); }

        private string _userEmailHeader;
        public string UserEmailHeader { get => _userEmailHeader; set => SetProperty(ref _userEmailHeader, value); }

        private string _selectedTypePc;
        public string SelectedTypePc
        {
            get => _selectedTypePc;
            set
            {
                if (SetProperty(ref _selectedTypePc, value))
                {
                    OnTypePcChanged(value);
                }
            }
        }

        private string _selectedService;
        public string SelectedService { get => _selectedService; set => SetProperty(ref _selectedService, value); }

        private string _demandeur;
        public string Demandeur { get => _demandeur; set => SetProperty(ref _demandeur, value); }

        private string _beneficiaire;
        public string Beneficiaire { get => _beneficiaire; set => SetProperty(ref _beneficiaire, value); }

        private string _commentaire;
        public string Commentaire { get => _commentaire; set => SetProperty(ref _commentaire, value); }

        private string _fileName;
        public string FileName { get => _fileName; set => SetProperty(ref _fileName, value); }

        private bool _fileNameVisible;
        public bool FileNameVisible { get => _fileNameVisible; set => SetProperty(ref _fileNameVisible, value); }

        private bool _formEnabled = true;
        public bool FormEnabled { get => _formEnabled; set => SetProperty(ref _formEnabled, value); }

        public Action<Window> OuvrirFenetreEtFermerActuelle;
        public Action<Window> OuvrirFenetreEtCacherActuelle;

        public RelayCommand SelectFileCommand { get; }
        public RelayCommand EnvoyerCommand { get; }
        public RelayCommand AnnulerCommand { get; }
        public RelayCommand RetourCommand { get; }
        public RelayCommand LogoutCommand { get; }

        public Window9ViewModel()
        {
            SelectFileCommand = new RelayCommand(_ => SelectFile());
            EnvoyerCommand = new RelayCommand(_ => Envoyer());
            AnnulerCommand = new RelayCommand(_ => Annuler());
            RetourCommand = new RelayCommand(_ => OuvrirFenetreEtFermerActuelle?.Invoke(new Window1()));
            LogoutCommand = new RelayCommand(_ => Logout());

            string nom = string.IsNullOrWhiteSpace(Class1.NomComplet) ? "Sara El Bajia" : Class1.NomComplet;
            string email = string.IsNullOrWhiteSpace(Class1.Email) ? "bajiasara71@gmail.com" : Class1.Email;

            UserNameHeader = "Bonjour, " + nom;
            UserEmailHeader = email;
        }

        private void SelectFile()
        {
            var dlg = new OpenFileDialog { Filter = "Fichiers supportés|*.pdf;*.doc;*.docx;*.xls;*.xlsx|Tous|*.*" };
            if (dlg.ShowDialog() == true)
            {
                _fichierJointPath = dlg.FileName;
                FileName = "📎 " + Path.GetFileName(_fichierJointPath);
                FileNameVisible = true;
            }
        }

        private void OnTypePcChanged(string typeChoisi)
        {
            if (string.IsNullOrEmpty(typeChoisi)) return;

            if (!VerifierDisponibiliteStock(typeChoisi))
            {
                MessageBox.Show(
                    $"Impossible d'envoyer la commande : aucun \"{typeChoisi}\" disponible en stock actuellement.",
                    "Rupture de stock",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                FormEnabled = false;
            }
            else
            {
                FormEnabled = true;
            }
        }

        private bool VerifierDisponibiliteStock(string typePc)
        {
            int quantiteDisponible = 0;
            try
            {
                using var conn = DatabaseService.GetConnection();
                conn.Open();

                const string sql = "SELECT COUNT(*) FROM materiel WHERE LOWER(type_materiel) = LOWER(@type)";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@type", typePc);

                var result = cmd.ExecuteScalar();
                quantiteDisponible = (result == null || result == DBNull.Value) ? 0 : Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la vérification du stock : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return quantiteDisponible > 0;
        }

        private void Envoyer()
        {
            if (string.IsNullOrEmpty(SelectedTypePc) || string.IsNullOrEmpty(SelectedService) ||
                string.IsNullOrWhiteSpace(Demandeur) || string.IsNullOrWhiteSpace(Beneficiaire))
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires.", "Champ requis", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string typeChoisi = SelectedTypePc;

            if (!VerifierDisponibiliteStock(typeChoisi))
            {
                MessageBox.Show(
                    $"Impossible d'envoyer la commande : aucun \"{typeChoisi}\" disponible en stock actuellement.",
                    "Rupture de stock",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                FormEnabled = false;
                return;
            }

            try
            {
                using var conn = DatabaseService.GetConnection();
                conn.Open();
                const string sql = @"INSERT INTO commande (type_pc, service, demandeur, beneficiaire, commentaire, fichier_joint, date_commande, statut)
                                     VALUES (@type_pc, @service, @demandeur, @beneficiaire, @commentaire, @fichier_joint, NOW(), 'En attente')";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@type_pc", typeChoisi);
                cmd.Parameters.AddWithValue("@service", SelectedService);
                cmd.Parameters.AddWithValue("@demandeur", Demandeur.Trim());
                cmd.Parameters.AddWithValue("@beneficiaire", Beneficiaire.Trim());
                cmd.Parameters.AddWithValue("@commentaire", Commentaire?.Trim() ?? "");
                cmd.Parameters.AddWithValue("@fichier_joint", string.IsNullOrEmpty(_fichierJointPath) ? DBNull.Value : (object)Path.GetFileName(_fichierJointPath));

                cmd.ExecuteNonQuery();
                MessageBox.Show("✔ Commande envoyée avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                ReinitialiserFormulaire();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Annuler()
        {
            if (MessageBox.Show("Voulez-vous vraiment annuler ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                ReinitialiserFormulaire();
        }

        private void ReinitialiserFormulaire()
        {
            SelectedTypePc = null;
            SelectedService = null;
            Demandeur = string.Empty;
            Beneficiaire = string.Empty;
            Commentaire = string.Empty;
            _fichierJointPath = string.Empty;
            FileName = string.Empty;
            FileNameVisible = false;
            FormEnabled = true;
        }

        private void Logout()
        {
            if (MessageBox.Show("Voulez-vous vraiment vous déconnecter ?", "Déconnexion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                OuvrirFenetreEtCacherActuelle?.Invoke(new Window1());
            }
        }
    }
}