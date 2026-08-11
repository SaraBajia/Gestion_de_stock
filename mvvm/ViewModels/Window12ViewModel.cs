using System;
using System.Windows;
using System.Windows.Media;
using MySql.Data.MySqlClient;
using WpfApp1.mvvm.Common;
using WpfApp1.mvvm.Services;

namespace WpfApp1.mvvm.ViewModels
{
    public class Window12ViewModel : ViewModelBase
    {
        private readonly string _email;

        private string _message;
        public string Message { get => _message; set => SetProperty(ref _message, value); }

        private Brush _messageColor = Brushes.Red;
        public Brush MessageColor { get => _messageColor; set => SetProperty(ref _messageColor, value); }

        public Func<string> ObtenirNouveauMdp;
        public Func<string> ObtenirConfirmerMdp;
        public Action<Window> OuvrirFenetreEtFermerActuelle;
        public Action<Window> OuvrirFenetreEtCacherActuelle;

        public RelayCommand ChangerCommand { get; }
        public RelayCommand RetourCommand { get; }

        public Window12ViewModel(string email)
        {
            _email = email;

            ChangerCommand = new RelayCommand(_ => Changer());
            RetourCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window11(_email)));
        }

        private void Changer()
        {
            string mdp1 = ObtenirNouveauMdp?.Invoke() ?? "";
            string mdp2 = ObtenirConfirmerMdp?.Invoke() ?? "";

            if (string.IsNullOrWhiteSpace(mdp1) || string.IsNullOrWhiteSpace(mdp2))
            {
                MessageColor = Brushes.Red;
                Message = "Veuillez remplir les deux champs.";
                return;
            }

            if (mdp1 != mdp2)
            {
                MessageColor = Brushes.Red;
                Message = "Les mots de passe ne correspondent pas.";
                return;
            }

            try
            {
                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    string qr = "UPDATE users SET pwd=@pwd WHERE email=@email";
                    var cmd = new MySqlCommand(qr, conn);
                    cmd.Parameters.AddWithValue("@pwd", mdp1);
                    cmd.Parameters.AddWithValue("@email", _email);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        MessageBox.Show("Mot de passe changé avec succès.", "Succès",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        OuvrirFenetreEtFermerActuelle?.Invoke(new Window1());
                    }
                    else
                    {
                        MessageColor = Brushes.Red;
                        Message = "Erreur : utilisateur introuvable.";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageColor = Brushes.Red;
                Message = "Erreur : " + ex.Message;
            }
        }
    }
}