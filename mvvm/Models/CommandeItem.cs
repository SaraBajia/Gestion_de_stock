using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace WpfApp1.mvvm.Models
{
    public class CommandeItem : INotifyPropertyChanged
    {
        private int _id;
        private string _dateCommande = "";
        private string _typePc = "";
        private string _service = "";
        private string _demandeur = "";
        private string _beneficiaire = "";
        private string _commentaire = "";
        private string _statut = "";
        private string _nouveauStatut = "";

        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public string DateCommande { get => _dateCommande; set { _dateCommande = value; OnPropertyChanged(); } }
        public string TypePc { get => _typePc; set { _typePc = value; OnPropertyChanged(); } }
        public string Service { get => _service; set { _service = value; OnPropertyChanged(); } }
        public string Demandeur { get => _demandeur; set { _demandeur = value; OnPropertyChanged(); } }
        public string Beneficiaire { get => _beneficiaire; set { _beneficiaire = value; OnPropertyChanged(); } }
        public string Commentaire { get => _commentaire; set { _commentaire = value; OnPropertyChanged(); } }

        public string Statut
        {
            get => _statut;
            set
            {
                _statut = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StatutCouleur));
            }
        }

        public string NouveauStatut { get => _nouveauStatut; set { _nouveauStatut = value; OnPropertyChanged(); } }

        public string[] StatutOptions { get; set; } = new[] { "En attente", "Validée", "Refusée" };

        public Brush StatutCouleur
        {
            get
            {
                switch (Statut)
                {
                    case "Validée": return new SolidColorBrush(Color.FromRgb(31, 174, 110));
                    case "Refusée": return new SolidColorBrush(Color.FromRgb(226, 61, 61));
                    default: return new SolidColorBrush(Color.FromRgb(245, 165, 36));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}