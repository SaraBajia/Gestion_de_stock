using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using MySql.Data.MySqlClient;
using WpfApp1.mvvm.Common;
using WpfApp1.mvvm.Models;
using WpfApp1.mvvm.Services;

namespace WpfApp1.mvvm.ViewModels
{
    public class Window10ViewModel : ViewModelBase
    {
        private const int SeuilStockFaible = 5;

        public static readonly string[] NomsMois = { "Janvier", "Février", "Mars", "Avril", "Mai", "Juin", "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Décembre" };

        // ====== Infos utilisateur ======
        private string _txtUserName = "Sara El Bajia";
        public string TxtUserName { get => _txtUserName; set => SetProperty(ref _txtUserName, value); }

        private string _txtUserRole = "bajiasara71@gmail.com";
        public string TxtUserRole { get => _txtUserRole; set => SetProperty(ref _txtUserRole, value); }

        // ====== KPIs ======
        private int _kpiMateriel;
        public int KpiMateriel { get => _kpiMateriel; set => SetProperty(ref _kpiMateriel, value); }

        private int _kpiConsommable;
        public int KpiConsommable { get => _kpiConsommable; set => SetProperty(ref _kpiConsommable, value); }

        private int _kpiPiece;
        public int KpiPiece { get => _kpiPiece; set => SetProperty(ref _kpiPiece, value); }

        private int _kpiCommandes;
        public int KpiCommandes { get => _kpiCommandes; set => SetProperty(ref _kpiCommandes, value); }

        // ====== Légende donut ======
        private string _legendMateriel = "0 unités";
        public string LegendMateriel { get => _legendMateriel; set => SetProperty(ref _legendMateriel, value); }

        private string _legendConsommable = "0 unités";
        public string LegendConsommable { get => _legendConsommable; set => SetProperty(ref _legendConsommable, value); }

        private string _legendPiece = "0 unités";
        public string LegendPiece { get => _legendPiece; set => SetProperty(ref _legendPiece, value); }

        // ====== Données pour les graphiques (lues par le code-behind pour dessiner) ======
        public List<int> EvolutionValeurs { get; private set; } = new List<int>();
        public List<string> EvolutionMois { get; private set; } = new List<string>();
        public List<int> MouvementsEntrees { get; private set; } = new List<int>();
        public List<int> MouvementsSorties { get; private set; } = new List<int>();
        public List<string> MouvementsJours { get; private set; } = new List<string>();

        public Action GraphiquesCharges;

        // ====== Notifications ======
        public ObservableCollection<Notification> Notifications { get; } = new ObservableCollection<Notification>();

        private int _notifCount;
        public int NotifCount { get => _notifCount; set => SetProperty(ref _notifCount, value); }

        private bool _notifBadgeVisible;
        public bool NotifBadgeVisible { get => _notifBadgeVisible; set => SetProperty(ref _notifBadgeVisible, value); }

        private bool _popupNotificationsOpen;
        public bool PopupNotificationsOpen { get => _popupNotificationsOpen; set => SetProperty(ref _popupNotificationsOpen, value); }

        // ====== Activités récentes ======
        public ObservableCollection<RecentActivityItem> Activites { get; } = new ObservableCollection<RecentActivityItem>();
        public ObservableCollection<RecentActivityItem> ToutesActivites { get; } = new ObservableCollection<RecentActivityItem>();

        private bool _popupToutesActivitesOpen;
        public bool PopupToutesActivitesOpen { get => _popupToutesActivitesOpen; set => SetProperty(ref _popupToutesActivitesOpen, value); }

        // ====== Filtre date ======
        public ObservableCollection<int> AnneeOptions { get; } = new ObservableCollection<int>();

        private int _selectedAnnee;
        public int SelectedAnnee { get => _selectedAnnee; set => SetProperty(ref _selectedAnnee, value); }

        private int _selectedMoisIndex;
        public int SelectedMoisIndex { get => _selectedMoisIndex; set => SetProperty(ref _selectedMoisIndex, value); }

        private string _periodeActuelle = "Ce mois-ci";
        public string PeriodeActuelle { get => _periodeActuelle; set => SetProperty(ref _periodeActuelle, value); }

        private bool _popupDateFiltreOpen;
        public bool PopupDateFiltreOpen { get => _popupDateFiltreOpen; set => SetProperty(ref _popupDateFiltreOpen, value); }

        // ====== Callbacks navigation ======
        public Action<Window> OuvrirFenetreEtCacherActuelle;

        // ====== Commands ======
        public RelayCommand ToggleFiltreCommand { get; }
        public RelayCommand ToggleNotifCommand { get; }
        public RelayCommand AppliquerDateCommand { get; }
        public RelayCommand VoirToutActivitesCommand { get; }
        public RelayCommand FermerPopupActivitesCommand { get; }
        public RelayCommand DeconnecterCommand { get; }
        public RelayCommand GestionCommandesCommand { get; }
        public RelayCommand NavAccueilCommand { get; }
        public RelayCommand NavStockCommand { get; }
        public RelayCommand NavMaterielCommand { get; }
        public RelayCommand NavConsommableCommand { get; }
        public RelayCommand NavPieceCommand { get; }
        public RelayCommand NavCommandeCommand { get; }

        public Window10ViewModel()
        {
            ToggleFiltreCommand = new RelayCommand(_ => PopupDateFiltreOpen = !PopupDateFiltreOpen);
            ToggleNotifCommand = new RelayCommand(_ => PopupNotificationsOpen = !PopupNotificationsOpen);
            AppliquerDateCommand = new RelayCommand(_ => AppliquerDate());
            VoirToutActivitesCommand = new RelayCommand(_ => VoirToutActivites());
            FermerPopupActivitesCommand = new RelayCommand(_ => PopupToutesActivitesOpen = false);
            DeconnecterCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window1()));
            GestionCommandesCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window13()));
            NavAccueilCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window3()));
            NavStockCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window8()));
            NavMaterielCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window5()));
            NavConsommableCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window6()));
            NavPieceCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window7()));
            NavCommandeCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window9()));
        }

        public void Initialize()
        {
            for (int annee = DateTime.Now.Year - 5; annee <= DateTime.Now.Year + 5; annee++)
                AnneeOptions.Add(annee);

            SelectedAnnee = DateTime.Now.Year;
            SelectedMoisIndex = DateTime.Now.Month - 1;

            ChargerTout();
        }

        private void ChargerTout()
        {
            ChargerInfosUtilisateur();
            ChargerKpis();
            ChargerGraphiquesDonnees();
            ChargerNotifications();
            ChargerActivitesRecentes();

            GraphiquesCharges?.Invoke();
        }

        private void ChargerInfosUtilisateur()
        {
            TxtUserName = "Sara El Bajia";
            TxtUserRole = "bajiasara71@gmail.com";
        }

        private int CompterLignes(MySqlConnection conn, string sql)
        {
            using (var cmd = new MySqlCommand(sql, conn))
            {
                var result = cmd.ExecuteScalar();
                return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
            }
        }

        private void ChargerKpis()
        {
            try
            {
                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    KpiMateriel = CompterLignes(conn, "SELECT COUNT(*) FROM materiel");
                    KpiConsommable = CompterLignes(conn, "SELECT COUNT(*) FROM consommable");
                    KpiPiece = CompterLignes(conn, "SELECT COUNT(*) FROM piece_de_rechange");

                    int nbCommandesAttente = 0;
                    try { nbCommandesAttente = CompterLignes(conn, "SELECT COUNT(*) FROM commande WHERE statut = 'En attente'"); } catch { }
                    KpiCommandes = nbCommandesAttente;
                }

                int total = KpiMateriel + KpiConsommable + KpiPiece;
                if (total == 0) total = 1;

                double pMat = (double)KpiMateriel / total;
                double pConso = (double)KpiConsommable / total;
                double pPiece = (double)KpiPiece / total;

                LegendMateriel = $"{KpiMateriel} unités ({pMat:P0})";
                LegendConsommable = $"{KpiConsommable} unités ({pConso:P0})";
                LegendPiece = $"{KpiPiece} unités ({pPiece:P0})";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des KPI : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ChargerGraphiquesDonnees()
        {
            try
            {
                EvolutionValeurs = new List<int>();
                EvolutionMois = new List<string> { "Févr", "Mars", "Avr", "Mai", "Juin", "Juil" };
                DateTime maintenant = DateTime.Now;

                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    for (int i = 5; i >= 0; i--)
                    {
                        DateTime moisRef = maintenant.AddMonths(-i);
                        int val = CompterLignes(conn, $"SELECT IFNULL(SUM(quantite),0) FROM mvt_stock WHERE type_mvt='entree' AND MONTH(date_mvt)={moisRef.Month} AND YEAR(date_mvt)={moisRef.Year}");
                        EvolutionValeurs.Add(val);
                    }
                }

                MouvementsEntrees = new List<int>();
                MouvementsSorties = new List<int>();
                MouvementsJours = new List<string>();

                var cultureFr = new System.Globalization.CultureInfo("fr-FR");
                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    for (int i = 6; i >= 0; i--)
                    {
                        DateTime jRef = DateTime.Now.AddDays(-i);
                        string nomJour = jRef.ToString("ddd", cultureFr);
                        nomJour = char.ToUpper(nomJour[0]) + nomJour.Substring(1).Replace(".", "");
                        MouvementsJours.Add(nomJour);

                        MouvementsEntrees.Add(CompterLignes(conn, $"SELECT IFNULL(SUM(quantite),0) FROM mvt_stock WHERE type_mvt='entree' AND DATE(date_mvt)='{jRef:yyyy-MM-dd}'"));
                        MouvementsSorties.Add(CompterLignes(conn, $"SELECT IFNULL(SUM(quantite),0) FROM mvt_stock WHERE type_mvt='sortie' AND DATE(date_mvt)='{jRef:yyyy-MM-dd}'"));
                    }
                }
            }
            catch { }
        }

        private void ChargerNotifications()
        {
            try
            {
                Notifications.Clear();
                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand("SELECT modele, quantite FROM consommable WHERE quantite <= @seuil ORDER BY quantite ASC LIMIT 2", conn))
                    {
                        cmd.Parameters.AddWithValue("@seuil", SeuilStockFaible);
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                Notifications.Add(new Notification
                                {
                                    Titre = $"Stock faible — {r["modele"]}",
                                    Description = $"Il reste seulement {r["quantite"]} unités",
                                    IconKind = PackIconKind.PackageVariant,
                                    IconBg = new SolidColorBrush(Color.FromRgb(229, 247, 239)),
                                    IconColor = new SolidColorBrush(Color.FromRgb(31, 174, 110))
                                });
                            }
                        }
                    }

                    using (var cmd = new MySqlCommand("SELECT type_pc, service, demandeur FROM commande WHERE statut = 'En attente' ORDER BY date_commande DESC LIMIT 2", conn))
                    {
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                Notifications.Add(new Notification
                                {
                                    Titre = $"Commande en attente — {r["type_pc"]}",
                                    Description = $"{r["demandeur"]} ({r["service"]})",
                                    IconKind = PackIconKind.ClipboardTextOutline,
                                    IconBg = new SolidColorBrush(Color.FromRgb(240, 235, 255)),
                                    IconColor = new SolidColorBrush(Color.FromRgb(124, 92, 252))
                                });
                            }
                        }
                    }
                }

                NotifCount = Notifications.Count;
                NotifBadgeVisible = Notifications.Count > 0;
            }
            catch { }
        }

        private void ChargerActivitesRecentes()
        {
            try
            {
                Activites.Clear();
                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT type_mvt, table_source, quantite, date_mvt FROM mvt_stock ORDER BY date_mvt DESC LIMIT 4";
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            string type = r["type_mvt"].ToString().ToLower();
                            string source = r["table_source"].ToString();
                            int qte = Convert.ToInt32(r["quantite"]);
                            DateTime dateMvt = Convert.ToDateTime(r["date_mvt"]);

                            bool isEntree = type == "entree";
                            string titre = isEntree ? "Nouvelle Entrée Stock" : "Nouvelle Sortie Stock";
                            string desc = isEntree ? $"+{qte} unités ajoutées à {source}" : $"-{qte} unités retirées de {source}";

                            Activites.Add(new RecentActivityItem
                            {
                                Titre = titre,
                                Description = desc,
                                Temps = dateMvt.ToString("HH:mm"),
                                IconKind = isEntree ? PackIconKind.ArrowDownBoldCircleOutline : PackIconKind.ArrowUpBoldCircleOutline,
                                IconBg = isEntree ? new SolidColorBrush(Color.FromRgb(229, 247, 239)) : new SolidColorBrush(Color.FromRgb(253, 236, 236)),
                                IconColor = isEntree ? new SolidColorBrush(Color.FromRgb(31, 174, 110)) : new SolidColorBrush(Color.FromRgb(226, 61, 61))
                            });
                        }
                    }
                }
            }
            catch { }
        }

        private void AppliquerDate()
        {
            int annee = SelectedAnnee;
            int mois = SelectedMoisIndex + 1;

            PeriodeActuelle = $"{NomsMois[SelectedMoisIndex]} {annee}";
            PopupDateFiltreOpen = false;
            ChargerTout();
        }

        private void VoirToutActivites()
        {
            try
            {
                ToutesActivites.Clear();

                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT type_mvt, table_source, quantite, date_mvt FROM mvt_stock " +
                                   "WHERE DATE(date_mvt) = CURDATE() ORDER BY date_mvt DESC";

                    using (var cmd = new MySqlCommand(query, conn))
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            string type = r["type_mvt"].ToString().ToLower();
                            string source = r["table_source"].ToString();
                            int qte = Convert.ToInt32(r["quantite"]);
                            DateTime dateMvt = Convert.ToDateTime(r["date_mvt"]);

                            bool isEntree = type == "entree";
                            string titre = isEntree ? "Nouvelle Entrée Stock" : "Nouvelle Sortie Stock";
                            string desc = isEntree ? $"+{qte} unités ajoutées à {source}" : $"-{qte} unités retirées de {source}";

                            ToutesActivites.Add(new RecentActivityItem
                            {
                                Titre = titre,
                                Description = desc,
                                Temps = dateMvt.ToString("HH:mm"),
                                IconKind = isEntree ? PackIconKind.ArrowDownBoldCircleOutline : PackIconKind.ArrowUpBoldCircleOutline,
                                IconBg = isEntree ? new SolidColorBrush(Color.FromRgb(229, 247, 239)) : new SolidColorBrush(Color.FromRgb(253, 236, 236)),
                                IconColor = isEntree ? new SolidColorBrush(Color.FromRgb(31, 174, 110)) : new SolidColorBrush(Color.FromRgb(226, 61, 61))
                            });
                        }
                    }
                }

                PopupToutesActivitesOpen = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des activités du jour : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}