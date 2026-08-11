using System.Windows;
using WpfApp1.mvvm.ViewModels;

namespace WpfApp1
{
    public partial class Window13 : Window
    {
        private readonly Window13ViewModel _vm;

        public Window13()
        {
            InitializeComponent();

            _vm = new Window13ViewModel
            {
                OuvrirFenetreEtFermerActuelle = (w) => { w.Show(); this.Close(); }
            };
            DataContext = _vm;
        }
    }
}