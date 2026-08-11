using System.Windows.Media;

namespace WpfApp1.mvvm.Models
{
    public class RoleStat
    {
        public string Role { get; set; }
        public int Count { get; set; }
        public string Pourcentage { get; set; }
        public double LargeurBarre { get; set; }
        public SolidColorBrush BarreCouleur { get; set; }
    }
}