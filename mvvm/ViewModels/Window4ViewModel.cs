using System;
using System.Windows;
using WpfApp1.mvvm.Common;

namespace WpfApp1.mvvm.ViewModels
{
    public class Window4ViewModel : ViewModelBase
    {
        public Action<Window> OuvrirFenetreEtCacherActuelle;

        public RelayCommand PieceRechangeCommand { get; }
        public RelayCommand ConsommableCommand { get; }
        public RelayCommand RetourCommand { get; }

        public Window4ViewModel()
        {
            PieceRechangeCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window7()));
            ConsommableCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window6()));
            RetourCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window3()));
        }
    }
}