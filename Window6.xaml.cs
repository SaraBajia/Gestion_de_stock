using System.Windows;
using WpfApp1.mvvm.ViewModels;

namespace WpfApp1
{
    public partial class Window6 : Window
    {
        private readonly Window6ViewModel _vm;

        public Window6()
        {
            InitializeComponent();

            _vm = new Window6ViewModel
            {
                OuvrirFenetreEtCacherActuelle = (w) => { w.Show(); this.Hide(); }
            };
            DataContext = _vm;
        }
    }
}