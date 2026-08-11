using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace WpfApp1.mvvm.Models
{
    public class RecentActivityItem
    {
        public string Titre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Temps { get; set; } = string.Empty;
        public PackIconKind IconKind { get; set; }
        public Brush IconBg { get; set; } = Brushes.Transparent;
        public Brush IconColor { get; set; } = Brushes.Black;
    }
}