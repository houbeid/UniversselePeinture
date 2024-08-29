using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace WPFModernVerticalMenu.Pages
{
    public partial class Statistique : Page
    {
        public ObservableCollection<Statistiques> Statistiques { get; set; }
        public ObservableCollection<CoverageData> CoverageData { get; set; }

        public Statistique()
        {
            InitializeComponent();

            Statistiques = new ObservableCollection<Statistiques>
            {
                new Statistiques { Produit = "Produit1", StockF = 100, StockA = 50, PourcentageV = 50, PourcentagePs = 20},
                new Statistiques { Produit = "Produit2", StockF = 200, StockA = 100, PourcentageV = 50, PourcentagePs = 30},
                new Statistiques { Produit = "Produit3", StockF = 150, StockA = 75, PourcentageV = 50, PourcentagePs = 40},
                new Statistiques { Produit = "Produit4", StockF = 120, StockA = 60, PourcentageV = 50, PourcentagePs = 50}
            };

            CoverageData = new ObservableCollection<CoverageData>
            {
                new CoverageData { Address = "Address1", Coverage = 75 },
                new CoverageData { Address = "Address2", Coverage = 50 },
                new CoverageData { Address = "Address3", Coverage = 90 },
            };

            DataContext = this;

            // Load the Leaflet map
            LoadMap();
        }

        private void LoadMap()
        {
            var mapPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Map.html");
            mapBrowser.Navigate(new Uri(mapPath, UriKind.Absolute));
        }
    }

    public class Statistiques
    {
        public string Produit { get; set; }
        public int StockF { get; set; } // Stock Fabriquer
        public int StockA { get; set; } // Stock Actuel
        public double PourcentageV { get; set; } // Pourcentage de Vente
        public double PourcentagePs { get; set; } // Pourcentage par rapport aux autres produits
    }

    public class CoverageData
    {
        public string Address { get; set; }
        public double Coverage { get; set; }
    }
}
