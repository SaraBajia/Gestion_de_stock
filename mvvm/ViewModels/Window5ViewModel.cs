using System;
using System.Windows;
using MySql.Data.MySqlClient;
using WpfApp1.mvvm.Common;
using WpfApp1.mvvm.Services;

namespace WpfApp1.mvvm.ViewModels
{
    public class Window5ViewModel : ViewModelBase
    {
        private bool _isChanging = false;

        private bool _isPcPortableChecked;
        public bool IsPcPortableChecked
        {
            get => _isPcPortableChecked;
            set
            {
                if (_isChanging) { SetProperty(ref _isPcPortableChecked, value); return; }
                if (SetProperty(ref _isPcPortableChecked, value) && value) SetExclusive("PcPortable");
                UpdateVisibility();
            }
        }

        private bool _isPcFixeChecked;
        public bool IsPcFixeChecked
        {
            get => _isPcFixeChecked;
            set
            {
                if (_isChanging) { SetProperty(ref _isPcFixeChecked, value); return; }
                if (SetProperty(ref _isPcFixeChecked, value) && value) SetExclusive("PcFixe");
                UpdateVisibility();
            }
        }

        private bool _isTabletteChecked;
        public bool IsTabletteChecked
        {
            get => _isTabletteChecked;
            set
            {
                if (_isChanging) { SetProperty(ref _isTabletteChecked, value); return; }
                if (SetProperty(ref _isTabletteChecked, value) && value) SetExclusive("Tablette");
                UpdateVisibility();
            }
        }

        private bool _isTelephoneChecked;
        public bool IsTelephoneChecked
        {
            get => _isTelephoneChecked;
            set
            {
                if (_isChanging) { SetProperty(ref _isTelephoneChecked, value); return; }
                if (SetProperty(ref _isTelephoneChecked, value) && value) SetExclusive("Telephone");
                UpdateVisibility();
            }
        }

        private bool _panelPCVisible;
        public bool PanelPCVisible { get => _panelPCVisible; set => SetProperty(ref _panelPCVisible, value); }

        private bool _panelMobileVisible;
        public bool PanelMobileVisible { get => _panelMobileVisible; set => SetProperty(ref _panelMobileVisible, value); }

        private bool _btnEnregistrerVisible;
        public bool BtnEnregistrerVisible { get => _btnEnregistrerVisible; set => SetProperty(ref _btnEnregistrerVisible, value); }

        private string _etiquette;
        public string Etiquette { get => _etiquette; set => SetProperty(ref _etiquette, value); }

        private string _nom;
        public string Nom { get => _nom; set => SetProperty(ref _nom, value); }

        private string _marque;
        public string Marque { get => _marque; set => SetProperty(ref _marque, value); }

        private string _modele;
        public string Modele { get => _modele; set => SetProperty(ref _modele, value); }

        private string _numSerie;
        public string NumSerie { get => _numSerie; set => SetProperty(ref _numSerie, value); }

        private string _stockage;
        public string Stockage { get => _stockage; set => SetProperty(ref _stockage, value); }

        private string _memoire;
        public string Memoire { get => _memoire; set => SetProperty(ref _memoire, value); }

        private string _macAdresse;
        public string MacAdresse { get => _macAdresse; set => SetProperty(ref _macAdresse, value); }

        private string _processeur;
        public string Processeur { get => _processeur; set => SetProperty(ref _processeur, value); }

        private string _etiquetteMobile;
        public string EtiquetteMobile { get => _etiquetteMobile; set => SetProperty(ref _etiquetteMobile, value); }

        private string _marqueMobile;
        public string MarqueMobile { get => _marqueMobile; set => SetProperty(ref _marqueMobile, value); }

        private string _modeleMobile;
        public string ModeleMobile { get => _modeleMobile; set => SetProperty(ref _modeleMobile, value); }

        private string _couleur;
        public string Couleur { get => _couleur; set => SetProperty(ref _couleur, value); }

        private string _stockageMobile;
        public string StockageMobile { get => _stockageMobile; set => SetProperty(ref _stockageMobile, value); }

        public Action<Window> OuvrirFenetreEtFermerActuelle;

        public RelayCommand EnregistrerCommand { get; }
        public RelayCommand RetourCommand { get; }

        public Window5ViewModel()
        {
            EnregistrerCommand = new RelayCommand(_ => Enregistrer());
            RetourCommand = new RelayCommand(_ => OuvrirFenetreEtFermerActuelle?.Invoke(new Window3()));
        }

        private void SetExclusive(string chosen)
        {
            _isChanging = true;
            if (chosen != "PcPortable") IsPcPortableChecked = false;
            if (chosen != "PcFixe") IsPcFixeChecked = false;
            if (chosen != "Tablette") IsTabletteChecked = false;
            if (chosen != "Telephone") IsTelephoneChecked = false;
            _isChanging = false;
        }

        private void UpdateVisibility()
        {
            PanelPCVisible = false;
            PanelMobileVisible = false;
            BtnEnregistrerVisible = false;

            if (IsPcPortableChecked || IsPcFixeChecked)
            {
                PanelPCVisible = true;
                BtnEnregistrerVisible = true;
            }
            else if (IsTabletteChecked || IsTelephoneChecked)
            {
                PanelMobileVisible = true;
                BtnEnregistrerVisible = true;
            }
        }

        private void Enregistrer()
        {
            try
            {
                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    if (IsPcPortableChecked || IsPcFixeChecked)
                    {
                        if (string.IsNullOrWhiteSpace(Etiquette))
                        {
                            MessageBox.Show("L'étiquette est obligatoire !");
                            return;
                        }

                        string req = @"INSERT INTO materiel(etiquette, type_materiel, nom, marque, modele,num_serie, stockage, RAM, processeur, adr_mac, couleur, id_users) VALUES(@etiquette, @type_materiel, @nom, @marque, @modele,@num_serie, @stockage, @RAM, @processeur, @adr_mac, @couleur, @id_users)";

                        var cmd = new MySqlCommand(req, conn);
                        cmd.Parameters.AddWithValue("@etiquette", Etiquette.Trim());
                        cmd.Parameters.AddWithValue("@type_materiel", IsPcPortableChecked ? "PC Portable" : "PC Fixe");
                        cmd.Parameters.AddWithValue("@nom", Nom?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@marque", Marque?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@modele", Modele?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@num_serie", NumSerie?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@stockage", Stockage?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@RAM", Memoire?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@processeur", Processeur?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@adr_mac", MacAdresse?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@couleur", "");
                        cmd.Parameters.AddWithValue("@id_users", Class1.IdUtilisateur);
                        cmd.ExecuteNonQuery();

                        EnregistrerMouvement(conn, "entree", "materiel", 1);
                    }
                    else if (IsTabletteChecked || IsTelephoneChecked)
                    {
                        if (string.IsNullOrWhiteSpace(EtiquetteMobile))
                        {
                            MessageBox.Show("L'étiquette est obligatoire !");
                            return;
                        }

                        string req = @"INSERT INTO materiel(etiquette, type_materiel, marque, modele, couleur, id_users) VALUES(@etiquette, @type_materiel, @marque, @modele, @clour, @id_users)";

                        var cmd = new MySqlCommand(req, conn);
                        cmd.Parameters.AddWithValue("@etiquette", EtiquetteMobile.Trim());
                        cmd.Parameters.AddWithValue("@type_materiel", IsTelephoneChecked ? "Téléphone" : "Tablette");
                        cmd.Parameters.AddWithValue("@marque", MarqueMobile?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@modele", ModeleMobile?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@couleur", Couleur?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@id_users", Class1.IdUtilisateur);
                        cmd.ExecuteNonQuery();

                        EnregistrerMouvement(conn, "entree", "materiel", 1);
                    }

                    MessageBox.Show("Matériel enregistré avec succès !");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur : " + ex.Message);
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