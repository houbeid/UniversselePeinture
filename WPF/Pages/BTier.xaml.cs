using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    /// Logique d'interaction pour BTier.xaml
    /// </summary>
    public partial class BTier : Page
    {
        public BTier()
        {
            InitializeComponent();
        }
         private static readonly HttpClient client = new HttpClient();
        private async void Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            // Validation de saisie
            if (string.IsNullOrEmpty(SecondNavigationTabTextBox.Text) ||
                string.IsNullOrEmpty(FirstNavigationTabTextBox.Text))
            {
                MessageBox.Show("Tous les champs marqués par * sont obligatoires.");
                return;
            }

            // Enregistrement des donnée
         
            var  recette = new PriseComptaDto
            {
                CodeClient = FirstNavigationTabTextBox.Text,
                priseCompta = decimal.Parse(SecondNavigationTabTextBox.Text, System.Globalization.CultureInfo.InvariantCulture),
                
            };

            var result = await AddPriseAsync(recette);
            if (result.IsSuccessStatusCode)
            {
                MessageBox.Show("Enregistrement réussi !");
            }
            else
            {
                MessageBox.Show("Erreur du l'insertion");
            }
        }

        private async Task<HttpResponseMessage> AddPriseAsync(PriseComptaDto recette)
        {
            var content = new StringContent(JsonConvert.SerializeObject(recette), Encoding.UTF8, "application/json");

            return await client.PostAsync("https://localhost:7210/api/Stock/PriseCompta", content);
        }
    }
    public class PriseComptaDto
    {
        public string CodeClient { get; set; }
        public decimal priseCompta { get; set; }
    }
}
