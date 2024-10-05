using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
            // Convertir l'objet recette en JSON
            var content = new StringContent(JsonConvert.SerializeObject(recette), Encoding.UTF8, "application/json");

            // Créer une requête POST pour ajouter la prise comptable
            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7210/api/Stock/PriseCompta");

            // Ajouter l'en-tête Authorization avec le token JWT
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.Token);

            // Attacher le contenu JSON à la requête
            request.Content = content;

            // Envoyer la requête
            return await client.SendAsync(request);
        }
    }
    public class PriseComptaDto
    {
        public string CodeClient { get; set; }
        public decimal priseCompta { get; set; }
    }
}
