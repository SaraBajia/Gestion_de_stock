using System.Windows;
using WpfApp1.mvvm.ViewModels;

namespace WpfApp1
{
    public partial class Window4 : Window
    {
        private readonly Window4ViewModel _vm;

        public Window4()
        {
            InitializeComponent();

            _vm = new Window4ViewModel
            {
                OuvrirFenetreEtCacherActuelle = (w) => { w.Show(); this.Hide(); }
            };
            DataContext = _vm;
        }
    }
}