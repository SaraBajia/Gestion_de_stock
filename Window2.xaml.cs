using System.Windows;
using System.Windows.Controls;
using WpfApp1.mvvm.ViewModels;

namespace WpfApp1
{
    public partial class Window2 : Window
    {
        private readonly Window2ViewModel _vm;

        public Window2()
        {
            InitializeComponent();

            _vm = new Window2ViewModel
            {
                ObtenirPassword = () => PasswordBox.Password,
                OuvrirFenetreEtCacherActuelle = (w) => { w.Show(); this.Hide(); }
            };
            DataContext = _vm;
        }

        // ====== بقات فـ code-behind لأن PasswordBox ماشي bindable لأسباب أمنية فـ WPF ======
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _vm.OnPasswordChanged(PasswordBox.Password);
        }
    }
}