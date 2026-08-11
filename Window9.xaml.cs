using System.Windows;
using System.Windows.Input;
using WpfApp1.mvvm.ViewModels;

namespace WpfApp1
{
    public partial class Window9 : Window
    {
        private readonly Window9ViewModel _vm;

        public Window9()
        {
            InitializeComponent();

            _vm = new Window9ViewModel
            {
                OuvrirFenetreEtFermerActuelle = (w) => { w.Show(); this.Hide(); },
                OuvrirFenetreEtCacherActuelle = (w) => { w.Show(); this.Hide(); }
            };
            DataContext = _vm;
        }

        private void UploadZone_Click(object sender, MouseButtonEventArgs e)
        {
            _vm.SelectFileCommand.Execute(null);
        }
    }
}