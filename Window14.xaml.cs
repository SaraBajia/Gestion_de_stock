using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfApp1.mvvm.ViewModels;

namespace WpfApp1
{
    public partial class Window14 : Window
    {
        private readonly Window14ViewModel _vm;
        private bool isDarkMode = false;

        public Window14()
        {
            InitializeComponent();

            _vm = new Window14ViewModel
            {
                OuvrirFenetreEtCacherActuelle = (w) => { w.Show(); this.Hide(); },
                FermerFenetreActuelle = () => this.Close()
            };
            DataContext = _vm;

            if (!_vm.VerifierAcces())
                return;

            _vm.Initialize();
        }

        // ====== كودة تبديل الثيم بقات هي هي (كتمس عناصر XAML مباشرة بأسماء، View بحت) ======
        private void BtnToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            isDarkMode = !isDarkMode;

            if (isDarkMode)
            {
                this.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#121212"));
                headerBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"));
                headerBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D2D"));

                txtHeaderTitle.Foreground = Brushes.White;
                txtHeaderDesc.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#AAAAAA"));

                btnThemeToggle.Content = "\uE706";
                btnThemeToggle.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D2D"));
                btnThemeToggle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F1C40F"));

                kpiCard1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"));
                kpiCard1.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D2D"));
                kpiCard2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"));
                kpiCard2.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D2D"));
                kpiCard3.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"));
                kpiCard3.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D2D"));
                kpiCard4.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"));
                kpiCard4.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D2D"));

                txtKpiTitle1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888"));
                txtKpiTitle2.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888"));
                txtKpiTitle3.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888"));
                txtKpiTitle4.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888"));

                txtTotalUtilisateurs.Foreground = Brushes.White;
                txtDernierUtilisateur.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ECC71"));

                gridContainer.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"));
                gridContainer.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D2D"));
                dgUsers.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"));
                dgUsers.RowBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"));
                dgUsers.AlternatingRowBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#252525"));
                dgUsers.Foreground = Brushes.White;
                dgUsers.HorizontalGridLinesBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D2D"));

                txtListTitle.Foreground = Brushes.White;
            }
            else
            {
                this.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F2F5"));
                headerBorder.Background = Brushes.White;
                headerBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E2E6EA"));

                txtHeaderTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0B3D6B"));
                txtHeaderDesc.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888"));

                btnThemeToggle.Content = "\uE706";
                btnThemeToggle.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F2F5"));
                btnThemeToggle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0B3D6B"));

                kpiCard1.Background = Brushes.White;
                kpiCard1.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E2E6EA"));
                kpiCard2.Background = Brushes.White;
                kpiCard2.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E2E6EA"));
                kpiCard3.Background = Brushes.White;
                kpiCard3.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E2E6EA"));
                kpiCard4.Background = Brushes.White;
                kpiCard4.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E2E6EA"));

                txtKpiTitle1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#999999"));
                txtKpiTitle2.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#999999"));
                txtKpiTitle3.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#999999"));
                txtKpiTitle4.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#999999"));

                txtTotalUtilisateurs.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0B3D6B"));
                txtDernierUtilisateur.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27AE60"));

                gridContainer.Background = Brushes.White;
                gridContainer.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E2E6EA"));
                dgUsers.Background = Brushes.White;
                dgUsers.RowBackground = Brushes.White;
                dgUsers.AlternatingRowBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F8FAFC"));
                dgUsers.Foreground = Brushes.Black;
                dgUsers.HorizontalGridLinesBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EAECEF"));

                txtListTitle.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0B3D6B"));
            }
        }
    }
}