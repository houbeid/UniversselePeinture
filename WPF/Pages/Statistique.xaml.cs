using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;

namespace WPFModernVerticalMenu.Pages
{
    public partial class Statistique : Page
    {
        public ObservableCollection<StatistiquProduit> Statistiques { get; set; }
        public ObservableCollection<CoverageData> CoverageData { get; set; }

        private static readonly HttpClient client = new HttpClient();

        public Statistique()
        {
            InitializeComponent();

            //Statistiques = new ObservableCollection<Statistiques>
            //{
            //    new Statistiques { Produit = "Produit1", StockF = 100, StockA = 50, PourcentageV = 50, PourcentagePs = 20},
            //    new Statistiques { Produit = "Produit2", StockF = 200, StockA = 100, PourcentageV = 50, PourcentagePs = 30},
            //    new Statistiques { Produit = "Produit3", StockF = 150, StockA = 75, PourcentageV = 50, PourcentagePs = 40},
            //    new Statistiques { Produit = "Produit4", StockF = 120, StockA = 60, PourcentageV = 50, PourcentagePs = 50}
            //};

            //CoverageData = new ObservableCollection<CoverageData>
            //{
            //    new CoverageData { Address = "Address1", Coverage = 75 },
            //    new CoverageData { Address = "Address2", Coverage = 50 },
            //    new CoverageData { Address = "Address3", Coverage = 90 },
            //};
            Statistiques = new ObservableCollection<StatistiquProduit>();
            CoverageData = new ObservableCollection<CoverageData>();


            LoadStatistique();

            DataContext = this;

            // Load the Leaflet map
           // LoadMap();
        }

        private async void LoadStatistique()
        {
            var response = await client.GetAsync("https://localhost:7210/api/Statistique");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var statistiques = JsonConvert.DeserializeObject<StatistiqueResponse>(jsonString);
                if (statistiques != null)
                {
                    foreach (var statistique in statistiques.statistiquProduits)
                    {
                        Statistiques.Add(statistique);
                    }
                    foreach(var statistique in statistiques.coverageDatas)
                    {
                        CoverageData.Add(statistique);
                    }
                }
            }
        }

        //private void LoadMap()
        //{
        //    var mapPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Map.html");
        //    mapBrowser.Navigate(new Uri(mapPath, UriKind.Absolute));
        //}
    }

    public class StatistiqueResponse
    {
        public List<StatistiquProduit> statistiquProduits { get; set; }
        public List<CoverageData> coverageDatas { get; set; }

        public StatistiqueResponse()
        {
            statistiquProduits = new List<StatistiquProduit>();
            coverageDatas = new List<CoverageData>();
        }
    }
    public class StatistiquProduit
    {
        public string produit { get; set; }

        public int stock_fabrique { get; set; }

        public int stock_actuel { get; set; }

        public double pourcentage_vent { get; set; }

        public decimal pourcentage_produit { get; set; }


    }

    public class CoverageData
    {
        public string Address { get; set; }
        public double Coverage { get; set; }
    }
}
