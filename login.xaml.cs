using System.Windows;
using System.Windows.Controls;
using WpfApp1.mvvm.ViewModels;

namespace WpfApp1
{
    public partial class Window1 : Window
    {
        private readonly LoginViewModel _vm;

        public Window1()
        {
            InitializeComponent();

            _vm = new LoginViewModel
            {
                ObtenirPassword = () => mdp.Password,
                OuvrirFenetreEtCacherActuelle = (w) => { w.Show(); this.Hide(); },
                OuvrirFenetreEtFermerActuelle = (w) => { w.Show(); this.Close(); }
            };
            DataContext = _vm;
        }

        // ====== كودة الـ Eye-toggle، بقات هي هي بلا تبديل ======
        private void BtnToggleShow_Checked(object sender, RoutedEventArgs e)
        {
            if (mdpText != null && mdp != null)
            {
                mdpText.Text = mdp.Password;
                mdp.Visibility = Visibility.Collapsed;
                mdpText.Visibility = Visibility.Visible;
                mdpText.Focus();
            }
        }

        private void BtnToggleShow_Unchecked(object sender, RoutedEventArgs e)
        {
            if (mdpText != null && mdp != null)
            {
                mdp.Password = mdpText.Text;
                mdpText.Visibility = Visibility.Collapsed;
                mdp.Visibility = Visibility.Visible;
                mdp.Focus();
            }
        }
    }
}