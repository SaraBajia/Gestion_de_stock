using System.Windows;
using WpfApp1.mvvm.ViewModels;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();

            _vm = new MainWindowViewModel
            {
                OuvrirFenetreEtCacherActuelle = (w) => { w.Show(); this.Hide(); }
            };
            DataContext = _vm;
        }
    }
}