using System;
using System.Windows;
using WpfApp1.mvvm.Common;

namespace WpfApp1.mvvm.ViewModels
{
    public class Window3ViewModel : ViewModelBase
    {
        public Action<Window> OuvrirFenetreEtFermerActuelle;

        public RelayCommand MaterielCommand { get; }
        public RelayCommand ConsommableCommand { get; }
        public RelayCommand AfficherCommand { get; }
        public RelayCommand RetourCommand { get; }

        public Window3ViewModel()
        {
            MaterielCommand = new RelayCommand(_ => OuvrirFenetreEtFermerActuelle?.Invoke(new Window5()));
            ConsommableCommand = new RelayCommand(_ => OuvrirFenetreEtFermerActuelle?.Invoke(new Window4()));
            AfficherCommand = new RelayCommand(_ => OuvrirFenetreEtFermerActuelle?.Invoke(new Window8()));
            RetourCommand = new RelayCommand(_ => OuvrirFenetreEtFermerActuelle?.Invoke(new Window1()));
        }
    }
}