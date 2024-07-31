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

        public DetailsHistorique()
        {
            InitializeComponent();

            HistoriqueAchats = new ObservableCollection<HistoriqueAchat>
            {
                new HistoriqueAchat { NomClient = "Client1", NumTelephone = "1234567890", Produit = "Produit1", Quantite = 10, Montant = 100.00 },
                new HistoriqueAchat { NomClient = "Client2", NumTelephone = "0987654321", Produit = "Produit2", Quantite = 5, Montant = 50.00 },
                new HistoriqueAchat { NomClient = "Client3", NumTelephone = "1112223334", Produit = "Produit3", Quantite = 7, Montant = 70.00 },
                new HistoriqueAchat { NomClient = "Client4", NumTelephone = "5556667778", Produit = "Produit4", Quantite = 3, Montant = 30.00 }
            };

            DataContext = this;
        }

        private void Retourner_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }

    public class HistoriqueAchat
    {
        public string NomClient { get; set; }
        public string NumTelephone { get; set; }
        public string Produit { get; set; }
        public int Quantite { get; set; }
        public double Montant { get; set; }
    }
}
