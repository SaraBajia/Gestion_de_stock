using System.Windows;
using WpfApp1.mvvm.ViewModels;

namespace WpfApp1
{
    public partial class Window7 : Window
    {
        private readonly Window7ViewModel _vm;
        private Window _previousWindow;

        public Window7()
        {
            InitializeComponent();

            _vm = new Window7ViewModel
            {
                OuvrirFenetreEtCacherActuelle = (w) => { w.Show(); this.Hide(); }
            };
            DataContext = _vm;
        }

        public Window7(Window previousWindow) : this()
        {
            _previousWindow = previousWindow;
        }
    }
}