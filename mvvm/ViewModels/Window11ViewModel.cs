using System;
using System.Windows;
using MySql.Data.MySqlClient;
using WpfApp1.mvvm.Common;
using WpfApp1.mvvm.Services;

namespace WpfApp1.mvvm.ViewModels
{
    public class Window11ViewModel : ViewModelBase
    {
        private readonly string _email;
        private string _reponseCorrecte;

        private string _question;
        public string Question { get => _question; set => SetProperty(ref _question, value); }

        private string _reponse;
        public string Reponse { get => _reponse; set => SetProperty(ref _reponse, value); }

        private string _erreur;
        public string Erreur { get => _erreur; set => SetProperty(ref _erreur, value); }

        public Action<Window> OuvrirFenetreEtFermerActuelle;
        public Action<Window> OuvrirFenetreEtCacherActuelle;
        public Action FermerFenetreActuelle;

        public RelayCommand VerifierCommand { get; }
        public RelayCommand RetourCommand { get; }

        public Window11ViewModel(string email)
        {
            _email = email;

            VerifierCommand = new RelayCommand(_ => Verifier());
            RetourCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window1()));

            ChargerQuestionAleatoire();
        }

        private void ChargerQuestionAleatoire()
        {
            try
            {
                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    var rnd = new Random();
                    int idQuestion = rnd.Next(1, 4);

                    string qrQuestion = "SELECT texte FROM questions WHERE id=@id";
                    var cmdQuestion = new MySqlCommand(qrQuestion, conn);
                    cmdQuestion.Parameters.AddWithValue("@id", idQuestion);

                    object texteQuestion = cmdQuestion.ExecuteScalar();

                    if (texteQuestion == null)
                    {
                        MessageBox.Show("Question introuvable.", "Erreur",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        FermerFenetreActuelle?.Invoke();
                        return;
                    }

                    Question = texteQuestion.ToString();

                    string colonneReponse = "question" + idQuestion;
                    string qrReponse = $"SELECT {colonneReponse} FROM users WHERE email=@email";
                    var cmdReponse = new MySqlCommand(qrReponse, conn);
                    cmdReponse.Parameters.AddWithValue("@email", _email);

                    object reponseDb = cmdReponse.ExecuteScalar();

                    if (reponseDb == null)
                    {
                        MessageBox.Show("Aucune réponse enregistrée pour cet utilisateur.", "Erreur",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        FermerFenetreActuelle?.Invoke();
                        return;
                    }

                    _reponseCorrecte = reponseDb.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur de connexion : " + ex.Message);
            }
        }

        private void Verifier()
        {
            string reponseUtilisateur = Reponse?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(reponseUtilisateur))
            {
                Erreur = "Veuillez saisir une réponse.";
                return;
            }

            if (string.Equals(reponseUtilisateur, _reponseCorrecte, StringComparison.OrdinalIgnoreCase))
            {
                OuvrirFenetreEtFermerActuelle?.Invoke(new Window12(_email));
            }
            else
            {
                Erreur = "Réponse incorrecte. Veuillez réessayer.";
            }
        }
    }
}