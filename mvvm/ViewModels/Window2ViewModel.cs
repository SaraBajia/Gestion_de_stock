using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using MySql.Data.MySqlClient;
using WpfApp1.mvvm.Common;
using WpfApp1.mvvm.Services;

namespace WpfApp1.mvvm.ViewModels
{
    public class Window2ViewModel : ViewModelBase
    {
        private string _nom;
        public string Nom { get => _nom; set => SetProperty(ref _nom, value); }

        private string _prenom;
        public string Prenom { get => _prenom; set => SetProperty(ref _prenom, value); }

        private string _role;
        public string Role { get => _role; set => SetProperty(ref _role, value); }

        private string _email;
        public string Email { get => _email; set => SetProperty(ref _email, value); }

        private string _reponse1;
        public string Reponse1 { get => _reponse1; set => SetProperty(ref _reponse1, value); }

        private string _reponse2;
        public string Reponse2 { get => _reponse2; set => SetProperty(ref _reponse2, value); }

        private string _reponse3;
        public string Reponse3 { get => _reponse3; set => SetProperty(ref _reponse3, value); }

        private string _question1Text;
        public string Question1Text { get => _question1Text; set => SetProperty(ref _question1Text, value); }

        private string _question2Text;
        public string Question2Text { get => _question2Text; set => SetProperty(ref _question2Text, value); }

        private string _question3Text;
        public string Question3Text { get => _question3Text; set => SetProperty(ref _question3Text, value); }

        private string _passwordInfoText = "12 caractères min., 1 majuscule, 1 minuscule, 1 chiffre, 1 caractère spécial";
        public string PasswordInfoText { get => _passwordInfoText; set => SetProperty(ref _passwordInfoText, value); }

        private Brush _passwordInfoColor = Brushes.Gray;
        public Brush PasswordInfoColor { get => _passwordInfoColor; set => SetProperty(ref _passwordInfoColor, value); }

        public Func<string> ObtenirPassword;
        public Action<Window> OuvrirFenetreEtCacherActuelle;

        public RelayCommand InscrireCommand { get; }
        public RelayCommand RetourCommand { get; }

        public Window2ViewModel()
        {
            InscrireCommand = new RelayCommand(_ => Inscrire());
            RetourCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window1()));

            ChargerQuestions();
        }

        private void ChargerQuestions()
        {
            try
            {
                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    string req = "SELECT id, texte FROM questions ORDER BY id ASC";
                    var cmd = new MySqlCommand(req, conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["id"]);
                            string texte = reader["texte"].ToString();

                            if (id == 1) Question1Text = texte;
                            else if (id == 2) Question2Text = texte;
                            else if (id == 3) Question3Text = texte;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des questions : " + ex.Message);
            }
        }

        private bool MotDePasseEstValide(string mdp)
        {
            if (string.IsNullOrEmpty(mdp) || mdp.Length < 12)
                return false;

            bool aMajuscule = Regex.IsMatch(mdp, "[A-Z]");
            bool aMinuscule = Regex.IsMatch(mdp, "[a-z]");
            bool aChiffre = Regex.IsMatch(mdp, "[0-9]");
            bool aSpecial = Regex.IsMatch(mdp, @"[!@#$%^&*()\-_=+\[\]{};:'"",.<>/?\\|`~]");

            return aMajuscule && aMinuscule && aChiffre && aSpecial;
        }

        // كتنداه من login.xaml.cs (code-behind) كل مرة كيتبدل الـ PasswordBox
        public void OnPasswordChanged(string mdp)
        {
            if (string.IsNullOrEmpty(mdp))
            {
                PasswordInfoColor = Brushes.Gray;
                PasswordInfoText = "12 caractères min., 1 majuscule, 1 minuscule, 1 chiffre, 1 caractère spécial";
            }
            else if (MotDePasseEstValide(mdp))
            {
                PasswordInfoColor = Brushes.Green;
                PasswordInfoText = "Mot de passe valide ✓";
            }
            else
            {
                PasswordInfoColor = Brushes.Red;
                PasswordInfoText = "12 caractères min., 1 majuscule, 1 minuscule, 1 chiffre, 1 caractère spécial";
            }
        }

        private void Inscrire()
        {
            string pwd = ObtenirPassword?.Invoke() ?? "";
            string reponse1 = Reponse1?.Trim() ?? "";
            string reponse2 = Reponse2?.Trim() ?? "";
            string reponse3 = Reponse3?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(reponse1) || string.IsNullOrWhiteSpace(reponse2) || string.IsNullOrWhiteSpace(reponse3))
            {
                MessageBox.Show("Veuillez répondre aux trois questions de sécurité.");
                return;
            }

            if (!MotDePasseEstValide(pwd))
            {
                MessageBox.Show("Le mot de passe doit contenir au moins 12 caractères, une majuscule, une minuscule, un chiffre et un caractère spécial.",
                    "Mot de passe invalide", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    string req = "INSERT INTO users(nom,prenom,role,email,pwd,question1,question2,question3) " +
                                 "VALUES(@nom,@prenom,@role,@email,@pwd,@q1,@q2,@q3)";
                    var cmd = new MySqlCommand(req, conn);
                    cmd.Parameters.AddWithValue("@nom", Nom);
                    cmd.Parameters.AddWithValue("@prenom", Prenom);
                    cmd.Parameters.AddWithValue("@role", Role);
                    cmd.Parameters.AddWithValue("@email", Email);
                    cmd.Parameters.AddWithValue("@pwd", pwd);
                    cmd.Parameters.AddWithValue("@q1", reponse1);
                    cmd.Parameters.AddWithValue("@q2", reponse2);
                    cmd.Parameters.AddWithValue("@q3", reponse3);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Inscription réussie !");
                OuvrirFenetreEtCacherActuelle?.Invoke(new Window3());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }
    }
}