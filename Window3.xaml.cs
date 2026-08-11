using System.Windows;
using WpfApp1.mvvm.ViewModels;

namespace WpfApp1
{
    public partial class Window3 : Window
    {
        private readonly Window3ViewModel _vm;

        public Window3()
        {
            InitializeComponent();

            _vm = new Window3ViewModel
            {
                OuvrirFenetreEtFermerActuelle = (w) => { w.Show(); this.Close(); }
            };
            DataContext = _vm;
        }
    }
}