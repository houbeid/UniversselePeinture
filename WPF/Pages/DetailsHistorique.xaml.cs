using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;

namespace WPFModernVerticalMenu.Pages
{
    /// <summary>
    /// Logique d'interaction pour DetailsHistorique.xaml
    /// </summary>
    public partial class DetailsHistorique : Page
    {
        public ObservableCollection<HistoriqueAchat> HistoriqueAchats { get; set; }

        //public DetailsHistorique()
        //{
        //    InitializeComponent();

        //    HistoriqueAchats = new ObservableCollection<HistoriqueAchat>
        //    {
        //        new HistoriqueAchat { Produit = "Produit1", Quantite = 10, Montant = 100, Date = DateTime.Now, Commercial = "Client1" },
        //    new HistoriqueAchat { Produit = "Produit2", Quantite = 5, Montant = 50, Date = DateTime.Now, Commercial = "Client2" },
        //    new HistoriqueAchat { Produit = "Produit3", Quantite = 7, Montant = 70, Date = DateTime.Now, Commercial = "Client3" },
        //    new HistoriqueAchat { Produit = "Produit4", Quantite = 3, Montant = 30, Date = DateTime.Now, Commercial = "Client4" }
        //    };

        //    DataContext = this;
        //}

        public DetailsHistorique(List<HistoriqueAchat> achats)
        {
            InitializeComponent();

            HistoriqueAchats = new ObservableCollection<HistoriqueAchat>();
            foreach(HistoriqueAchat achat in achats)
            {
                HistoriqueAchats.Add(achat);
            }
            DataContext = this;
        }

        private void Retourner_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }

    public class HistoriqueAchat
    {
        public string Produit { get; set; }
        public int Quantite { get; set; }
        public decimal Montant { get; set; }
        public DateTime Date { get; set; }
        public string Commercial { get; set; }
    }
}
