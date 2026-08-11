using System.Windows;
using WpfApp1.mvvm.ViewModels;

namespace WpfApp1
{
    public partial class Window12 : Window
    {
        private readonly Window12ViewModel _vm;

        public Window12(string email)
        {
            InitializeComponent();

            _vm = new Window12ViewModel(email)
            {
                ObtenirNouveauMdp = () => pwdNouveau.Password,
                ObtenirConfirmerMdp = () => pwdConfirmer.Password,
                OuvrirFenetreEtFermerActuelle = (w) => { w.Show(); this.Close(); },
                OuvrirFenetreEtCacherActuelle = (w) => { w.Show(); this.Hide(); }
            };
            DataContext = _vm;
        }
    }
}