using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MySql.Data.MySqlClient;
using WpfApp1.mvvm.Common;
using WpfApp1.mvvm.Models;
using WpfApp1.mvvm.Services;

namespace WpfApp1.mvvm.ViewModels
{
    public class Window14ViewModel : ViewModelBase
    {
        private DataTable _dtUsers;

        public ObservableCollection<RoleStat> RoleStats { get; } = new ObservableCollection<RoleStat>();

        private string _txtUserName;
        public string TxtUserName { get => _txtUserName; set => SetProperty(ref _txtUserName, value); }

        private string _txtUserRole;
        public string TxtUserRole { get => _txtUserRole; set => SetProperty(ref _txtUserRole, value); }

        private DataView _dataView;
        public DataView DataView { get => _dataView; set => SetProperty(ref _dataView, value); }

        private object _selectedRow;
        public object SelectedRow { get => _selectedRow; set => SetProperty(ref _selectedRow, value); }

        private string _totalUtilisateurs = "0";
        public string TotalUtilisateurs { get => _totalUtilisateurs; set => SetProperty(ref _totalUtilisateurs, value); }

        private string _dernierUtilisateur = "—";
        public string DernierUtilisateur { get => _dernierUtilisateur; set => SetProperty(ref _dernierUtilisateur, value); }

        private string _dernierEmail = "—";
        public string DernierEmail { get => _dernierEmail; set => SetProperty(ref _dernierEmail, value); }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    AppliquerFiltre(value);
            }
        }

        public Action<Window> OuvrirFenetreEtCacherActuelle;
        public Action FermerFenetreActuelle;

        public RelayCommand SupprimerSelectionCommand { get; }
        public RelayCommand SupprimerLigneCommand { get; }
        public RelayCommand RetourCommand { get; }
        public RelayCommand DeconnecterCommand { get; }

        public Window14ViewModel()
        {
            SupprimerSelectionCommand = new RelayCommand(_ => SupprimerSelection());
            SupprimerLigneCommand = new RelayCommand(p => SupprimerLigne(p as DataRowView));
            RetourCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window3()));
            DeconnecterCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window1()));
        }

        public bool VerifierAcces()
        {
            if (Class1.Role != "Admin")
            {
                MessageBox.Show("Accès réservé à l'administrateur.", "Accès refusé",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                FermerFenetreActuelle?.Invoke();
                return false;
            }
            return true;
        }

        public void Initialize()
        {
            TxtUserName = Class1.NomComplet;
            TxtUserRole = Class1.Role;

            ChargerUtilisateurs();
        }

        private void ChargerUtilisateurs()
        {
            try
            {
                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT id, nom, prenom, email, Role, derniere_connexion FROM users ORDER BY derniere_connexion DESC";
                    var adapter = new MySqlDataAdapter(query, conn);

                    _dtUsers = new DataTable();
                    adapter.Fill(_dtUsers);

                    DataView = _dtUsers.DefaultView;

                    MettreAJourStatistiques();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des utilisateurs : " + ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MettreAJourStatistiques()
        {
            if (_dtUsers == null) return;

            int total = _dtUsers.Rows.Count;
            TotalUtilisateurs = total.ToString();

            // نبحثو على أول صف عندو derniere_connexion، اللي ماشي هو المستخدم الداخل حاليا
            DataRow dernier = _dtUsers.AsEnumerable()
                .Where(r => r["derniere_connexion"] != DBNull.Value)
                .FirstOrDefault(r => Convert.ToInt32(r["id"]) != Class1.IdUtilisateur);

            if (dernier != null)
            {
                DernierUtilisateur = dernier["nom"] + " " + dernier["prenom"];
                DernierEmail = dernier["email"].ToString();
            }
            else
            {
                DernierUtilisateur = "—";
                DernierEmail = "—";
            }

            RoleStats.Clear();

            Color[] palette = new Color[]
            {
                (Color)ColorConverter.ConvertFromString("#0B3D6B"),
                (Color)ColorConverter.ConvertFromString("#27AE60"),
                (Color)ColorConverter.ConvertFromString("#F39C12"),
                (Color)ColorConverter.ConvertFromString("#8E44AD"),
                (Color)ColorConverter.ConvertFromString("#E74C3C")
            };

            var groupes = _dtUsers.AsEnumerable()
                .GroupBy(r => r["Role"].ToString())
                .Select(g => new { Role = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToList();

            int i = 0;
            foreach (var g in groupes)
            {
                double pourcentage = total > 0 ? Math.Round((double)g.Count / total * 100, 1) : 0;

                RoleStats.Add(new RoleStat
                {
                    Role = g.Role,
                    Count = g.Count,
                    Pourcentage = pourcentage + " %",
                    LargeurBarre = Math.Max(4, pourcentage * 2.4),
                    BarreCouleur = new SolidColorBrush(palette[i % palette.Length])
                });
                i++;
            }
        }

        private void AppliquerFiltre(string filtre)
        {
            if (_dtUsers == null) return;

            string f = filtre?.Trim().Replace("'", "''") ?? "";

            _dtUsers.DefaultView.RowFilter = string.IsNullOrEmpty(f)
                ? ""
                : $"nom LIKE '%{f}%' OR prenom LIKE '%{f}%' OR email LIKE '%{f}%' OR Role LIKE '%{f}%'";
        }

        private void SupprimerSelection()
        {
            if (SelectedRow is DataRowView row)
            {
                SupprimerUtilisateur(row);
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un utilisateur dans la liste.",
                    "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SupprimerLigne(DataRowView row)
        {
            if (row != null)
                SupprimerUtilisateur(row);
        }

        private void SupprimerUtilisateur(DataRowView row)
        {
            int id = Convert.ToInt32(row["id"]);
            string nomComplet = row["nom"] + " " + row["prenom"];

            if (id == Class1.IdUtilisateur)
            {
                MessageBox.Show("Vous ne pouvez pas supprimer votre propre compte.",
                    "Action impossible", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Voulez-vous vraiment supprimer l'utilisateur \"{nomComplet}\" ?\nCette action est irréversible.",
                "Confirmer la suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM users WHERE id = @id";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Utilisateur supprimé avec succès.", "Succès",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                ChargerUtilisateurs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la suppression : " + ex.Message,
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}