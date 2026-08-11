using System.Windows;
using WpfApp1.mvvm.ViewModels;

namespace WpfApp1
{
    public partial class Window11 : Window
    {
        private readonly Window11ViewModel _vm;

        public Window11(string email)
        {
            InitializeComponent();

            _vm = new Window11ViewModel(email)
            {
                OuvrirFenetreEtFermerActuelle = (w) => { w.Show(); this.Close(); },
                OuvrirFenetreEtCacherActuelle = (w) => { w.Show(); this.Hide(); },
                FermerFenetreActuelle = () => this.Close()
            };
            DataContext = _vm;
        }
    }
}