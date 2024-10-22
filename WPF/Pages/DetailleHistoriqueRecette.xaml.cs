using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFModernVerticalMenu.Pages
{
    /// <summary>
    /// Logique d'interaction pour DetailleHistoriqueRecette.xaml
    /// </summary>
    /// 
    
    public partial class DetailleHistoriqueRecette : Page
    {
        public ObservableCollection<HistoriqueRecette> Historiquerecettes { get; set; }
        public DetailleHistoriqueRecette(List<HistoriqueRecette> recettes)
        {
            InitializeComponent();
            Historiquerecettes = new ObservableCollection<HistoriqueRecette>();
            foreach (HistoriqueRecette recette in recettes)
            {
                Historiquerecettes.Add(recette);
            }
            DataContext = this;
        }
    }
    public class HistoriqueRecette
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string CodeClient { get; set; }
        public decimal Recette { get; set; }
    }
}
