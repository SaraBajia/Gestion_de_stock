using System.Data;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.mvvm.ViewModels;

namespace WpfApp1
{
    // بقات هي هي بلا تبديل (ماكانتش مستعملة فـ الكود المعروض، خليناها بحالها)
    public class ElementCorbeille
    {
        public int Id { get; set; }
        public string TableOrigine { get; set; }
        public string IdentifiantOrigine { get; set; }
        public string DonneesJson { get; set; }
        public System.DateTime DateSuppression { get; set; }

        public string TypeAffiche =>
            TableOrigine == "materiel" ? "Matériel" :
            TableOrigine == "consommable" ? "Consommable" : "Pièce de rechange";

        public string DateAffichee => DateSuppression.ToString("dd/MM/yyyy HH:mm");
    }

    public partial class Window8 : Window
    {
        private readonly Window8ViewModel _vm;
        private Window _previousWindow;

        public Window8()
        {
            InitializeComponent();

            _vm = new Window8ViewModel
            {
                OuvrirFenetreEtCacherActuelle = (w) => { w.Show(); this.Hide(); },
                OuvrirFenetreSeulement = (w) => { w.Show(); },
                RebuildColumnsRequested = (filter) => SetGridColumns(filter)
            };
            DataContext = _vm;

            Loaded += Window8_Loaded;
        }

        public Window8(Window previousWindow) : this()
        {
            _previousWindow = previousWindow;
        }

        private void Window8_Loaded(object sender, RoutedEventArgs e)
        {
            _vm.Initialize();
        }

        // ====== بقات هنا لأنها كتصاوب DataGridColumn مباشرة (منطق View بحت) ======
        private void SetGridColumns(string filter)
        {
            dgStock.Columns.Clear();

            switch (filter)
            {
                case "Consommable":
                    AddCol("ID", "etiquette", 0.6);
                    AddCol("Modèle", "modele", 1.2);
                    AddCol("Couleur", "couleur", 1);
                    AddCol("Référence", "reference", 1);
                    AddCol("Quantité", "quantite", 0.7);
                    break;

                case "Pièce de rechange":
                    AddCol("ID", "etiquette", 0.6);
                    AddCol("Modèle", "modele", 1.3);
                    AddCol("Pièce", "couleur", 1.5);
                    AddCol("Quantité", "quantite", 0.7);
                    break;

                case "Tous":
                    AddCol("ID / Étiquette", "etiquette", 1);
                    AddCol("Type", "type_materiel", 1);
                    AddCol("Date", "date_ajout", 1);
                    break;

                default:
                    AddCol("Étiquette", "etiquette", 0.8);
                    AddCol("Type", "type_materiel", 1.1);
                    AddCol("Nom", "nom", 1);
                    AddCol("Marque", "marque", 0.9);
                    AddCol("Modèle", "modele", 1);
                    AddCol("Num Série", "num_serie", 1.1);
                    AddCol("Stockage", "stockage", 0.8);
                    AddCol("RAM", "RAM", 0.7);
                    AddCol("Processeur", "processeur", 1.1);
                    AddCol("Adr. MAC", "adr_mac", 1.2);
                    AddCol("Date", "date_ajout", 0.9);
                    break;
            }
        }

        private void AddCol(string header, string bindingPath, double widthStar)
        {
            dgStock.Columns.Add(new DataGridTextColumn
            {
                Header = header,
                Binding = new System.Windows.Data.Binding(bindingPath),
                Width = new DataGridLength(widthStar, DataGridLengthUnitType.Star)
            });
        }
    }
}