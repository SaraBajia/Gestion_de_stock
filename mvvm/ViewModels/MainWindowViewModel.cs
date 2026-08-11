using System;
using System.Windows;
using System.Windows.Threading;
using WpfApp1.mvvm.Common;

namespace WpfApp1.mvvm.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly DispatcherTimer _horlogeTimer;

        private string _dateText;
        public string DateText
        {
            get => _dateText;
            set => SetProperty(ref _dateText, value);
        }

        private string _heureText;
        public string HeureText
        {
            get => _heureText;
            set => SetProperty(ref _heureText, value);
        }

        public Action<Window> OuvrirFenetreEtCacherActuelle;

        public RelayCommand NaviguerVersLoginCommand { get; }

        public MainWindowViewModel()
        {
            NaviguerVersLoginCommand = new RelayCommand(_ => OuvrirFenetreEtCacherActuelle?.Invoke(new Window1()));

            MettreAJourHorloge();
            _horlogeTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _horlogeTimer.Tick += (s, e) => MettreAJourHorloge();
            _horlogeTimer.Start();
        }

        private void MettreAJourHorloge()
        {
            DateText = DateTime.Now.ToString("dddd dd MMMM yyyy");
            HeureText = DateTime.Now.ToString("HH:mm:ss");
        }
    }
}