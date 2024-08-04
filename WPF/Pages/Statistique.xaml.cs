using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WPFModernVerticalMenu.Pages
{
    /// <summary>
    /// Interaction logic for Statistique.xaml
    /// </summary>
    public partial class Statistique : Page
    {
        public ObservableCollection<Statistiques> Statistiques { get; set; }

        public Statistique()
        {
            InitializeComponent();

            Statistiques = new ObservableCollection<Statistiques>
            {
                new Statistiques { Produit = "Produit1", StockF = 100, StockA = 50, Pourcentage = 50},
                new Statistiques { Produit = "Produit2", StockF = 200, StockA = 100, Pourcentage = 50},
                new Statistiques { Produit = "Produit3", StockF = 150, StockA = 75, Pourcentage = 50},
                new Statistiques { Produit = "Produit4", StockF = 120, StockA = 60, Pourcentage = 50}
            };

            DataContext = this;
        }
    }

    public class Statistiques
    {
        public string Produit { get; set; }
        public int StockF { get; set; } // Stock Fabriquer
        public int StockA { get; set; } // Stock Actuel
        public double Pourcentage { get; set; } // Pourcentage De Vente
    }
}
