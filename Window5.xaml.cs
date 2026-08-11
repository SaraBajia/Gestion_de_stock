using System.Windows;
using WpfApp1.mvvm.ViewModels;

namespace WpfApp1
{
    public partial class Window5 : Window
    {
        private readonly Window5ViewModel _vm;

        public Window5()
        {
            InitializeComponent();

            _vm = new Window5ViewModel
            {
                OuvrirFenetreEtFermerActuelle = (w) => { w.Show(); this.Close(); }
            };
            DataContext = _vm;
        }
    }
}