using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;
using WpfApp1.mvvm.Common;
using WpfApp1.mvvm.Services;

namespace WpfApp1.mvvm.ViewModels
{
    public class Window8ViewModel : ViewModelBase
    {
        private string _currentEditType = "";
        private string _originalEtiquette = "";

        // ====== Infos utilisateur ======
        private string _txtUserName;
        public string TxtUserName { get => _txtUserName; set => SetProperty(ref _txtUserName, value); }

        private string _txtUserEmail;
        public string TxtUserEmail { get => _txtUserEmail; set => SetProperty(ref _txtUserEmail, value); }

        private bool _isAdmin;
        public bool IsAdmin { get => _isAdmin; set => SetProperty(ref _isAdmin, value); }

        // ====== Filtre / Recherche ======
        private string _currentFilter = "Tous";
        public string CurrentFilter { get => _currentFilter; set => SetProperty(ref _currentFilter, value); }

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    PlaceholderVisible = string.IsNullOrEmpty(value);
            }
        }

        private bool _placeholderVisible = true;
        public bool PlaceholderVisible { get => _placeholderVisible; set => SetProperty(ref _placeholderVisible, value); }

        public ObservableCollection<string> SearchFieldOptions { get; } = new ObservableCollection<string>();

        private string _selectedSearchField;
        public string SelectedSearchField { get => _selectedSearchField; set => SetProperty(ref _selectedSearchField, value); }

        // ====== DataGrid ======
        private DataView _dataView;
        public DataView DataView { get => _dataView; set => SetProperty(ref _dataView, value); }

        private object _selectedRow;
        public object SelectedRow
        {
            get => _selectedRow;
            set
            {
                if (SetProperty(ref _selectedRow, value))
                    PanelModifierVisible = false;
            }
        }

        // ====== Panneau de modification ======
        private bool _panelModifierVisible;
        public bool PanelModifierVisible { get => _panelModifierVisible; set => SetProperty(ref _panelModifierVisible, value); }

        private string _lblEtiquette = "Étiquette";
        public string LblEtiquette { get => _lblEtiquette; set => SetProperty(ref _lblEtiquette, value); }

        private string _lblCouleur = "Couleur";
        public string LblCouleur { get => _lblCouleur; set => SetProperty(ref _lblCouleur, value); }

        private string _editEtiquette;
        public string EditEtiquette { get => _editEtiquette; set => SetProperty(ref _editEtiquette, value); }

        private string _editType;
        public string EditType { get => _editType; set => SetProperty(ref _editType, value); }

        private bool _editTypeReadOnly;
        public bool EditTypeReadOnly { get => _editTypeReadOnly; set => SetProperty(ref _editTypeReadOnly, value); }

        private string _editNom;
        public string EditNom { get => _editNom; set => SetProperty(ref _editNom, value); }

        private string _editModele;
        public string EditModele { get => _editModele; set => SetProperty(ref _editModele, value); }

        private string _editRAM;
        public string EditRAM { get => _editRAM; set => SetProperty(ref _editRAM, value); }

        private string _editStockage;
        public string EditStockage { get => _editStockage; set => SetProperty(ref _editStockage, value); }

        private string _editNumSerie;
        public string EditNumSerie { get => _editNumSerie; set => SetProperty(ref _editNumSerie, value); }

        private string _editMarque;
        public string EditMarque { get => _editMarque; set => SetProperty(ref _editMarque, value); }

        private string _editProcesseur;
        public string EditProcesseur { get => _editProcesseur; set => SetProperty(ref _editProcesseur, value); }

        private string _editAdrMac;
        public string EditAdrMac { get => _editAdrMac; set => SetProperty(ref _editAdrMac, value); }

        private string _editCouleur;
        public string EditCouleur { get => _editCouleur; set => SetProperty(ref _editCouleur, value); }

        private string _editReference;
        public string EditReference { get => _editReference; set => SetProperty(ref _editReference, value); }

        private string _editQuantite;
        public string EditQuantite { get => _editQuantite; set => SetProperty(ref _editQuantite, value); }

        private string _editDate;
        public string EditDate { get => _editDate; set => SetProperty(ref _editDate, value); }

        private bool _groupTypeVisible;
        public bool GroupTypeVisible { get => _groupTypeVisible; set => SetProperty(ref _groupTypeVisible, value); }

        private bool _groupNomVisible;
        public bool GroupNomVisible { get => _groupNomVisible; set => SetProperty(ref _groupNomVisible, value); }

        private bool _groupModeleVisible;
        public bool GroupModeleVisible { get => _groupModeleVisible; set => SetProperty(ref _groupModeleVisible, value); }

        private bool _groupRAMVisible;
        public bool GroupRAMVisible { get => _groupRAMVisible; set => SetProperty(ref _groupRAMVisible, value); }

        private bool _groupStockageVisible;
        public bool GroupStockageVisible { get => _groupStockageVisible; set => SetProperty(ref _groupStockageVisible, value); }

        private bool _groupNumSerieVisible;
        public bool GroupNumSerieVisible { get => _groupNumSerieVisible; set => SetProperty(ref _groupNumSerieVisible, value); }

        private bool _groupMarqueVisible;
        public bool GroupMarqueVisible { get => _groupMarqueVisible; set => SetProperty(ref _groupMarqueVisible, value); }

        private bool _groupProcesseurVisible;
        public bool GroupProcesseurVisible { get => _groupProcesseurVisible; set => SetProperty(ref _groupProcesseurVisible, value); }

        private bool _groupAdrMacVisible;
        public bool GroupAdrMacVisible { get => _groupAdrMacVisible; set => SetProperty(ref _groupAdrMacVisible, value); }

        private bool _groupCouleurVisible;
        public bool GroupCouleurVisible { get => _groupCouleurVisible; set => SetProperty(ref _groupCouleurVisible, value); }

        private bool _groupReferenceVisible;
        public bool GroupReferenceVisible { get => _groupReferenceVisible; set => SetProperty(ref _groupReferenceVisible, value); }

        private bool _groupQuantiteVisible;
        public bool GroupQuantiteVisible { get => _groupQuantiteVisible; set => SetProperty(ref _groupQuantiteVisible, value); }

        private bool _groupDateVisible;
        public bool GroupDateVisible { get => _groupDateVisible; set => SetProperty(ref _groupDateVisible, value); }

        // ====== Callbacks ======
        public Action<string> RebuildColumnsRequested;
        public Action<Window> OuvrirFenetreEtCacherActuelle;
        public Action<Window> OuvrirFenetreSeulement;

        // ====== Commands ======
        public RelayCommand FilterCommand { get; }
        public RelayCommand RechercherCommand { get; }
        public RelayCommand AjouterCommand { get; }
        public RelayCommand RetirerCommand { get; }
        public RelayCommand ModifierCommand { get; }
        public RelayCommand EnregistrerCommand { get; }
        public RelayCommand FermerModifierCommand { get; }
        public RelayCommand AccueilCommand { get; }
        public RelayCommand ConsommableCommand { get; }
        public RelayCommand PieceRechangeCommand { get; }
        public RelayCommand MvtCommand { get; }
        public RelayCommand MaterielCommand { get; }
        public RelayCommand UtilisateursCommand { get; }
        public RelayCommand DeconnecterCommand { get; }
        public RelayCommand RetourCommand { get; }

        public Window8ViewModel()
        {
            FilterCommand = new RelayCommand(p => AppliquerFiltre(p as string));
            RechercherCommand = new RelayCommand(_ => ChargerDonnees());
            AjouterCommand = new RelayCommand(_ => Ajouter());
            RetirerCommand = new RelayCommand(_ => Retirer());
            ModifierCommand = new RelayCommand(_ => Modifier());
            EnregistrerCommand = new RelayCommand(_ => EnregistrerModification());
            FermerModifierCommand = new RelayCommand(_ => { PanelModifierVisible = false; SelectedRow = null; });
            AccueilCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window3()));
            ConsommableCommand = new RelayCommand(_ => OuvrirFenetreSeulement?.Invoke(new Window6()));
            PieceRechangeCommand = new RelayCommand(_ => OuvrirFenetreSeulement?.Invoke(new Window7()));
            MvtCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window10()));
            MaterielCommand = new RelayCommand(_ => OuvrirFenetreSeulement?.Invoke(new Window5()));
            UtilisateursCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window14()));
            DeconnecterCommand = new RelayCommand(_ =>
            {
                Class1.Deconnecter();
                OuvrirFenetreEtCacherActuelle?.Invoke(new Window1());
            });
            RetourCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window3()));
        }

        public void Initialize()
        {
            TxtUserName = Class1.NomComplet;
            TxtUserEmail = Class1.Email;
            IsAdmin = Class1.Role != null && Class1.Role.ToLower() == "admin";

            UpdateSearchFieldOptions(CurrentFilter);
            ChargerDonnees();
        }

        private void AppliquerFiltre(string filtre)
        {
            if (string.IsNullOrEmpty(filtre)) return;
            CurrentFilter = filtre;
            PanelModifierVisible = false;
            UpdateSearchFieldOptions(CurrentFilter);
            ChargerDonnees();
        }

        private void UpdateSearchFieldOptions(string filter)
        {
            SearchFieldOptions.Clear();

            switch (filter)
            {
                case "Consommable":
                    SearchFieldOptions.Add("Tous les champs");
                    SearchFieldOptions.Add("ID");
                    SearchFieldOptions.Add("Modèle");
                    SearchFieldOptions.Add("Couleur");
                    SearchFieldOptions.Add("Référence");
                    SearchFieldOptions.Add("Quantité");
                    break;

                case "Pièce de rechange":
                    SearchFieldOptions.Add("Tous les champs");
                    SearchFieldOptions.Add("ID");
                    SearchFieldOptions.Add("Modèle");
                    SearchFieldOptions.Add("Pièce");
                    SearchFieldOptions.Add("Quantité");
                    break;

                default:
                    SearchFieldOptions.Add("Tous les champs");
                    SearchFieldOptions.Add("Étiquette");
                    SearchFieldOptions.Add("Nom");
                    SearchFieldOptions.Add("Marque");
                    SearchFieldOptions.Add("Modèle");
                    SearchFieldOptions.Add("Num. Série");
                    break;
            }

            SelectedSearchField = SearchFieldOptions.Count > 0 ? SearchFieldOptions[0] : null;
        }

        private void ChargerDonnees()
        {
            try
            {
                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    var cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = BuildQuery(cmd);

                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    RebuildColumnsRequested?.Invoke(CurrentFilter);
                    DataView = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur de chargement : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string BuildQuery(MySqlCommand cmd)
        {
            string search = SearchText?.Trim() ?? "";
            string searchField = SelectedSearchField ?? "Tous les champs";

            if (!string.IsNullOrEmpty(search))
                cmd.Parameters.AddWithValue("@search", "%" + search + "%");

            switch (CurrentFilter)
            {
                case "PC portable":
                case "PC fixe":
                case "Tablette":
                case "Téléphone":
                    cmd.Parameters.AddWithValue("@typeFiltre", CurrentFilter);
                    return BuildMaterielQuery(" AND type_materiel = @typeFiltre", search, searchField);

                case "Consommable":
                    return BuildConsommableQuery(search, searchField);

                case "Pièce de rechange":
                    return BuildPieceQuery(search, searchField);

                default:
                    return BuildTousQuery();
            }
        }

        private string BuildMaterielQuery(string extraWhere, string search, string searchField)
        {
            string where = extraWhere;
            if (!string.IsNullOrEmpty(search))
            {
                if (searchField == "Tous les champs")
                    where += " AND (etiquette LIKE @search OR nom LIKE @search OR marque LIKE @search OR modele LIKE @search OR num_serie LIKE @search OR processeur LIKE @search)";
                else if (searchField == "Étiquette") where += " AND etiquette LIKE @search";
                else if (searchField == "Nom") where += " AND nom LIKE @search";
                else if (searchField == "Marque") where += " AND marque LIKE @search";
                else if (searchField == "Modèle") where += " AND modele LIKE @search";
                else if (searchField == "Num. Série") where += " AND num_serie LIKE @search";
                else if (searchField == "Processeur") where += " AND processeur LIKE @search";
            }

            return $@"SELECT etiquette, type_materiel, nom, marque, modele, num_serie, stockage, RAM, processeur, adr_mac, localisation AS couleur, date_ajout FROM materiel WHERE 1=1{where}";
        }

        private string BuildConsommableQuery(string search, string searchField)
        {
            string where = "";
            if (!string.IsNullOrEmpty(search))
            {
                if (searchField == "Tous les champs")
                    where += " AND (CAST(id AS CHAR) LIKE @search OR modele LIKE @search OR couleur LIKE @search OR reference LIKE @search OR CAST(quantite AS CHAR) LIKE @search)";
                else if (searchField == "ID") where += " AND CAST(id AS CHAR) LIKE @search";
                else if (searchField == "Modèle") where += " AND modele LIKE @search";
                else if (searchField == "Couleur") where += " AND couleur LIKE @search";
                else if (searchField == "Référence") where += " AND reference LIKE @search";
                else if (searchField == "Quantité") where += " AND CAST(quantite AS CHAR) LIKE @search";
            }

            return $@"SELECT CAST(id AS CHAR) AS etiquette, modele, couleur, reference, quantite FROM consommable WHERE 1=1{where}";
        }

        private string BuildPieceQuery(string search, string searchField)
        {
            string where = "";
            if (!string.IsNullOrEmpty(search))
            {
                if (searchField == "Tous les champs")
                    where += " AND (CAST(id AS CHAR) LIKE @search OR modele LIKE @search OR piece LIKE @search OR CAST(quantite AS CHAR) LIKE @search)";
                else if (searchField == "ID") where += " AND CAST(id AS CHAR) LIKE @search";
                else if (searchField == "Modèle") where += " AND modele LIKE @search";
                else if (searchField == "Pièce") where += " AND piece LIKE @search";
                else if (searchField == "Quantité") where += " AND CAST(quantite AS CHAR) LIKE @search";
            }

            return $@"SELECT CAST(id AS CHAR) AS etiquette, modele, piece AS couleur, quantite FROM piece_de_rechange WHERE 1=1{where}";
        }

        private string BuildTousQuery()
        {
            return @"
        SELECT etiquette, type_materiel, IFNULL(CAST(date_ajout AS CHAR),'') AS date_ajout
        FROM materiel
        UNION ALL
        SELECT CAST(id AS CHAR), 'Consommable', IFNULL(CAST(date_ajout AS CHAR),'')
        FROM consommable
        UNION ALL
        SELECT CAST(id AS CHAR), 'Pièce de rechange', IFNULL(CAST(date_ajout AS CHAR),'')
        FROM piece_de_rechange";
        }

        private void Ajouter()
        {
            switch (CurrentFilter)
            {
                case "Consommable":
                    new Window6().ShowDialog();
                    break;

                case "Pièce de rechange":
                    new Window7().ShowDialog();
                    break;

                default:
                    new Window5().ShowDialog();
                    break;
            }
            ChargerDonnees();
        }

        private void Retirer()
        {
            if (SelectedRow == null)
            {
                MessageBox.Show("Veuillez sélectionner l'élément à supprimer dans le tableau d'abord.", "Attention", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            var result = MessageBox.Show(
                "Êtes-vous sûr de vouloir supprimer cet élément du stock ?",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result != MessageBoxResult.Yes) return;

            try
            {
                var rowView = (DataRowView)SelectedRow;
                var row = rowView.Row;

                string etiquetteOrId = row["etiquette"]?.ToString();
                string targetTable;
                string idColumnName;

                if (CurrentFilter == "Consommable" || (CurrentFilter == "Tous" && row.Table.Columns.Contains("type_materiel") && row["type_materiel"]?.ToString() == "Consommable"))
                {
                    targetTable = "consommable";
                    idColumnName = "id";
                }
                else if (CurrentFilter == "Pièce de rechange" || (CurrentFilter == "Tous" && row.Table.Columns.Contains("type_materiel") && row["type_materiel"]?.ToString() == "Pièce de rechange"))
                {
                    targetTable = "piece_de_rechange";
                    idColumnName = "id";
                }
                else
                {
                    targetTable = "materiel";
                    idColumnName = "etiquette";
                }

                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    string query = $"DELETE FROM {targetTable} WHERE {idColumnName} = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", etiquetteOrId);
                        cmd.ExecuteNonQuery();
                    }
                }

                ChargerDonnees();
                MessageBox.Show("L'élément a été supprimé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur est survenue lors de la suppression : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Modifier()
        {
            if (CurrentFilter == "Tous")
            {
                MessageBox.Show("Sélectionnez un type précis (Matériel, Consommable, Pièce de rechange) pour modifier.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (SelectedRow == null)
            {
                MessageBox.Show("Sélectionnez une ligne à modifier.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            AfficherFormulaireModification();
        }

        private bool RemplirChamp(Action<bool> setGroupVisible, Action<string> setText, object valeurBrute)
        {
            string valeur = valeurBrute?.ToString();
            bool estVide = valeurBrute == null || valeurBrute == DBNull.Value
                           || string.IsNullOrWhiteSpace(valeur)
                           || valeur.Equals("NULL", StringComparison.OrdinalIgnoreCase);

            if (estVide)
            {
                setText("");
                setGroupVisible?.Invoke(false);
                return false;
            }

            setText(valeur);
            setGroupVisible?.Invoke(true);
            return true;
        }

        private void HideAllEditGroups()
        {
            GroupTypeVisible = false;
            GroupNomVisible = false;
            GroupModeleVisible = false;
            GroupRAMVisible = false;
            GroupStockageVisible = false;
            GroupNumSerieVisible = false;
            GroupMarqueVisible = false;
            GroupProcesseurVisible = false;
            GroupAdrMacVisible = false;
            GroupCouleurVisible = false;
            GroupReferenceVisible = false;
            GroupQuantiteVisible = false;
            GroupDateVisible = false;
        }

        private void AfficherFormulaireModification()
        {
            if (!(SelectedRow is DataRowView row)) return;

            HideAllEditGroups();

            _originalEtiquette = row["etiquette"]?.ToString();
            EditEtiquette = _originalEtiquette;

            LblEtiquette = (CurrentFilter == "Consommable" || CurrentFilter == "Pièce de rechange") ? "ID" : "Étiquette";

            if (CurrentFilter == "Consommable")
            {
                _currentEditType = "Consommable";
                LblCouleur = "Couleur";

                RemplirChamp(v => GroupModeleVisible = v, t => EditModele = t, row["modele"]);
                RemplirChamp(v => GroupCouleurVisible = v, t => EditCouleur = t, row["couleur"]);
                RemplirChamp(v => GroupReferenceVisible = v, t => EditReference = t, row["reference"]);

                EditQuantite = row["quantite"]?.ToString() ?? "";
                GroupQuantiteVisible = true;
            }
            else if (CurrentFilter == "Pièce de rechange")
            {
                _currentEditType = "Pièce de rechange";
                LblCouleur = "Pièce";

                RemplirChamp(v => GroupModeleVisible = v, t => EditModele = t, row["modele"]);
                RemplirChamp(v => GroupCouleurVisible = v, t => EditCouleur = t, row["couleur"]);

                EditQuantite = row["quantite"]?.ToString() ?? "";
                GroupQuantiteVisible = true;
            }
            else
            {
                _currentEditType = row["type_materiel"]?.ToString();

                RemplirChamp(v => GroupTypeVisible = v, t => EditType = t, row["type_materiel"]);
                RemplirChamp(v => GroupNomVisible = v, t => EditNom = t, row["nom"]);
                RemplirChamp(v => GroupModeleVisible = v, t => EditModele = t, row["modele"]);
                RemplirChamp(v => GroupRAMVisible = v, t => EditRAM = t, row["RAM"]);
                RemplirChamp(v => GroupStockageVisible = v, t => EditStockage = t, row["stockage"]);
                RemplirChamp(v => GroupNumSerieVisible = v, t => EditNumSerie = t, row["num_serie"]);
                RemplirChamp(v => GroupMarqueVisible = v, t => EditMarque = t, row["marque"]);
                RemplirChamp(v => GroupProcesseurVisible = v, t => EditProcesseur = t, row["processeur"]);
                RemplirChamp(v => GroupAdrMacVisible = v, t => EditAdrMac = t, row["adr_mac"]);
                RemplirChamp(v => GroupCouleurVisible = v, t => EditCouleur = t, row["couleur"]);
                RemplirChamp(v => GroupDateVisible = v, t => EditDate = t, row["date_ajout"]);

                EditTypeReadOnly = true;
            }

            PanelModifierVisible = true;
        }

        private void EnregistrerModification()
        {
            string nouvelleEtiquette = EditEtiquette?.Trim();

            if (string.IsNullOrEmpty(nouvelleEtiquette))
            {
                MessageBox.Show("L'étiquette/ID ne peut pas être vide.", "Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(_originalEtiquette))
            {
                MessageBox.Show("Aucun enregistrement sélectionné.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd;

                    if (_currentEditType == "Consommable")
                    {
                        string query = @"UPDATE consommable SET id=@nouvelId, modele=@modele, couleur=@couleur, reference=@reference, quantite=@quantite WHERE id=@ancienId";

                        cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@nouvelId", nouvelleEtiquette);
                        cmd.Parameters.AddWithValue("@modele", EditModele);
                        cmd.Parameters.AddWithValue("@couleur", EditCouleur);
                        cmd.Parameters.AddWithValue("@reference", EditReference);
                        cmd.Parameters.AddWithValue("@quantite", int.TryParse(EditQuantite, out int qte) ? (object)qte : DBNull.Value);
                        cmd.Parameters.AddWithValue("@ancienId", _originalEtiquette);
                    }
                    else if (_currentEditType == "Pièce de rechange")
                    {
                        string query = @"UPDATE piece_de_rechange SET id=@nouvelId, modele=@modele, piece=@piece, quantite=@quantite WHERE id=@ancienId";

                        cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@nouvelId", nouvelleEtiquette);
                        cmd.Parameters.AddWithValue("@modele", EditModele);
                        cmd.Parameters.AddWithValue("@piece", EditCouleur);
                        cmd.Parameters.AddWithValue("@quantite", int.TryParse(EditQuantite, out int qte) ? (object)qte : DBNull.Value);
                        cmd.Parameters.AddWithValue("@ancienId", _originalEtiquette);
                    }
                    else
                    {
                        string query = @"UPDATE materiel SET 
                                etiquette=@nouvelleEtiquette, type_materiel=@type, nom=@nom, 
                                marque=@marque, modele=@modele, num_serie=@num, stockage=@stockage, 
                                RAM=@ram, processeur=@cpu, adr_mac=@mac
                                WHERE etiquette=@ancienneEtiquette";

                        cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@nouvelleEtiquette", nouvelleEtiquette);
                        cmd.Parameters.AddWithValue("@type", EditType);
                        cmd.Parameters.AddWithValue("@nom", EditNom);
                        cmd.Parameters.AddWithValue("@marque", EditMarque);
                        cmd.Parameters.AddWithValue("@modele", EditModele);
                        cmd.Parameters.AddWithValue("@num", EditNumSerie);
                        cmd.Parameters.AddWithValue("@stockage", EditStockage);
                        cmd.Parameters.AddWithValue("@ram", EditRAM);
                        cmd.Parameters.AddWithValue("@cpu", EditProcesseur);
                        cmd.Parameters.AddWithValue("@mac", EditAdrMac);
                        cmd.Parameters.AddWithValue("@ancienneEtiquette", _originalEtiquette);
                    }

                    cmd.ExecuteNonQuery();
                }

                ChargerDonnees();
                MessageBox.Show("Modification enregistrée.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                PanelModifierVisible = false;
                _originalEtiquette = "";
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MessageBox.Show("Cette étiquette/ID existe déjà. Choisissez une valeur unique.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}