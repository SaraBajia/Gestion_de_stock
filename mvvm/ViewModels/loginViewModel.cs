using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using MySql.Data.MySqlClient;
using WpfApp1.mvvm.Common;
using WpfApp1.mvvm.ViewModels;
using WpfApp1.mvvm.Services;
using WpfApp1.mvvm.Models;


namespace WpfApp1.mvvm.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly DispatcherTimer _notifTimer;

        public ObservableCollection<NotificationItem> Notifications { get; } = new ObservableCollection<NotificationItem>();

        private string _email = "";
        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                {
                    RechercherEmails(value);
                }
            }
        }

        private ObservableCollection<string> _emailSuggestions = new ObservableCollection<string>();
        public ObservableCollection<string> EmailSuggestions
        {
            get => _emailSuggestions;
            set => SetProperty(ref _emailSuggestions, value);
        }

        private bool _isDropDownOpen;
        public bool IsDropDownOpen
        {
            get => _isDropDownOpen;
            set => SetProperty(ref _isDropDownOpen, value);
        }

        private bool _hasNotifications;
        public bool HasNotifications
        {
            get => _hasNotifications;
            set => SetProperty(ref _hasNotifications, value);
        }

        private int _badgeCount;
        public int BadgeCount
        {
            get => _badgeCount;
            set => SetProperty(ref _badgeCount, value);
        }

        private bool _popupOpen;
        public bool PopupOpen
        {
            get => _popupOpen;
            set => SetProperty(ref _popupOpen, value);
        }

        public Action<Window> OuvrirFenetreEtCacherActuelle;
        public Action<Window> OuvrirFenetreEtFermerActuelle;
        public Func<string> ObtenirPassword;

        public RelayCommand LoginCommand { get; }
        public RelayCommand CommanderCommand { get; }
        public RelayCommand ForgotPasswordCommand { get; }
        public RelayCommand CreateAccountCommand { get; }
        public RelayCommand ToggleNotificationPopupCommand { get; }
        public RelayCommand NotificationItemClickCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(_ => Login());
            CommanderCommand = new RelayCommand(_ => Commander());
            ForgotPasswordCommand = new RelayCommand(_ => MotDePasseOublie());
            CreateAccountCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window2()));
            ToggleNotificationPopupCommand = new RelayCommand(_ => PopupOpen = !PopupOpen);
            NotificationItemClickCommand = new RelayCommand(param => OnNotificationClick(param));

            VerifierNouvellesCommandes();
            _notifTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
            _notifTimer.Tick += (s, e) => VerifierNouvellesCommandes();
            _notifTimer.Start();
        }

        private void RechercherEmails(string txtBoxText)
        {
            if (string.IsNullOrWhiteSpace(txtBoxText))
            {
                IsDropDownOpen = false;
                EmailSuggestions = new ObservableCollection<string>();
                return;
            }

            try
            {
                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    string qr = "SELECT email FROM users WHERE email LIKE @email LIMIT 5";
                    using (var cmd = new MySqlCommand(qr, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", txtBoxText + "%");
                        using (var reader = cmd.ExecuteReader())
                        {
                            var suggestions = new List<string>();
                            while (reader.Read())
                                suggestions.Add(reader["email"].ToString());

                            if (suggestions.Count > 0)
                            {
                                EmailSuggestions = new ObservableCollection<string>(suggestions);
                                IsDropDownOpen = true;
                            }
                            else
                            {
                                IsDropDownOpen = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur AutoComplete: " + ex.Message);
            }
        }

        private void VerifierNouvellesCommandes()
        {
            try
            {
                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    string qr = "SELECT id, type_pc, service, demandeur, date_commande FROM commande WHERE vu = 0 ORDER BY date_commande DESC";
                    using (var cmd = new MySqlCommand(qr, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        var idsExistants = new HashSet<int>(Notifications.Select(n => n.Id));

                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["id"]);
                            if (!idsExistants.Contains(id))
                            {
                                Notifications.Insert(0, new NotificationItem
                                {
                                    Id = id,
                                    Titre = "Nouvelle commande — " + reader["type_pc"].ToString(),
                                    Details = reader["demandeur"].ToString() + " (" + reader["service"].ToString() + ")",
                                    Date = Convert.ToDateTime(reader["date_commande"]).ToString("dd/MM/yyyy HH:mm")
                                });
                            }
                        }
                    }
                }
                MettreAJourBadge();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }

        private void MettreAJourBadge()
        {
            BadgeCount = Notifications.Count;
            HasNotifications = Notifications.Count > 0;
        }

        private void OnNotificationClick(object param)
        {
            if (param == null) return;
            int id = Convert.ToInt32(param);

            try
            {
                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("UPDATE commande SET vu = 1 WHERE id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }

                var item = Notifications.FirstOrDefault(n => n.Id == id);
                if (item != null) Notifications.Remove(item);

                MettreAJourBadge();
                PopupOpen = false;
                _notifTimer.Stop();

                OuvrirFenetreEtCacherActuelle?.Invoke(new Window13());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }

        private void Login()
        {
            string email = Email?.Trim() ?? "";
            string password = ObtenirPassword?.Invoke()?.Trim() ?? "";

            if (email == "" || password == "")
            {
                MessageBox.Show("Veuillez remplir tous les champs !");
                return;
            }

            if (email.Equals("commande.SI@airbus.com", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Vous n'avez pas l'autorisation de vous connecter ici. Veuillez utiliser le bouton 'Commander'.", "Accès Refusé", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    string qr = "SELECT * FROM users WHERE email=@email AND pwd=@pwd";
                    var cmd = new MySqlCommand(qr, conn);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@pwd", password);

                    var read = cmd.ExecuteReader();
                    if (read.HasRows)
                    {
                        read.Read();
                        Class1.IdUtilisateur = Convert.ToInt32(read["id"]);
                        Class1.Nom = read["nom"].ToString();
                        Class1.Prenom = read["prenom"].ToString();
                        Class1.Email = read["email"].ToString();
                        Class1.Role = read["role"].ToString();
                        read.Close();

                        using (var cmdMaj = new MySqlCommand("UPDATE users SET derniere_connexion = NOW() WHERE id = @id", conn))
                        {
                            cmdMaj.Parameters.AddWithValue("@id", Class1.IdUtilisateur);
                            cmdMaj.ExecuteNonQuery();
                        }

                        OuvrirFenetreEtCacherActuelle?.Invoke(new Window3());
                    }
                    else
                    {
                        MessageBox.Show("Veuillez vérifier les informations.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }

            Email = "";
            EmailSuggestions = new ObservableCollection<string>();
        }

        private void Commander()
        {
            string email = Email?.Trim() ?? "";
            string password = ObtenirPassword?.Invoke()?.Trim() ?? "";

            if (email == "" || password == "")
            {
                MessageBox.Show("Veuillez remplir tous les champs !");
                return;
            }

            if (!email.Equals("commande.SI@airbus.com", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Seul le responsable des commandes est autorisé à utiliser ce bouton !", "Accès Interdit", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            try
            {
                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    string qr = "SELECT * FROM users WHERE email=@email AND pwd=@pwd";
                    var cmd = new MySqlCommand(qr, conn);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@pwd", password);

                    var read = cmd.ExecuteReader();
                    if (read.Read())
                    {
                        Class1.IdUtilisateur = Convert.ToInt32(read["id"]);
                        Class1.Nom = read["nom"].ToString();
                        Class1.Prenom = read["prenom"].ToString();
                        Class1.Email = read["email"].ToString();
                        Class1.Role = read["role"].ToString();
                        read.Close();

                        using (var cmdMaj = new MySqlCommand("UPDATE users SET derniere_connexion = NOW() WHERE id = @id", conn))
                        {
                            cmdMaj.Parameters.AddWithValue("@id", Class1.IdUtilisateur);
                            cmdMaj.ExecuteNonQuery();
                        }

                        OuvrirFenetreEtFermerActuelle?.Invoke(new Window9());
                    }
                    else
                    {
                        MessageBox.Show("Veuillez vérifier les informations.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }

        private void MotDePasseOublie()
        {
            string email = Email?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Veuillez d'abord saisir votre email.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    string qr = "SELECT id FROM users WHERE email=@email";
                    var cmd = new MySqlCommand(qr, conn);
                    cmd.Parameters.AddWithValue("@email", email);

                    object result = cmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Aucun compte trouvé avec cet email.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
                return;
            }

            OuvrirFenetreEtCacherActuelle?.Invoke(new Window11(email));
        }
    }
}