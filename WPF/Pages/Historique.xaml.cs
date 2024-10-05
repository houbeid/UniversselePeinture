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
    /// Logique d'interaction pour Historique.xaml
    /// </summary>
    public partial class Historique : Page
    {
        private static readonly HttpClient client = new HttpClient();
        public Historique()
        {
            InitializeComponent();
        }

         private async void Rechercher_Click(object sender, RoutedEventArgs e)
        {

            var result = await GetHistoriqueClient(FirstNavigationTabTextBox.Text);
            //HistoriqueAchat historique = new HistoriqueAchat()
            //{

            //};
           // NavigationService.Navigate(new DetailsHistorique());
            NavigationService.Navigate(new DetailsHistorique(result));

        }
        private async Task<List<HistoriqueAchat>> GetHistoriqueClient(string codeclient)
        {
            // Construire l'URL avec le code client en paramètre
            var url = $"https://localhost:7210/api/Historique?codeclient={codeclient}";

            // Créer une requête GET pour récupérer l'historique du client
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Ajouter l'en-tête Authorization avec le token JWT
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.Token);

            // Envoyer la requête
            var response = await client.SendAsync(request);

            // Vérifier si la requête a réussi
            if (response.IsSuccessStatusCode)
            {
                // Lire le contenu de la réponse en chaîne de caractères
                var responseString = await response.Content.ReadAsStringAsync();

                // Désérialiser le JSON reçu en une liste d'objets HistoriqueAchat
                var historiqueAchats = JsonConvert.DeserializeObject<List<HistoriqueAchat>>(responseString);

                // Afficher la réponse pour débogage
                Console.WriteLine(responseString);

                return historiqueAchats;
            }
            else
            {
                // En cas d'échec, afficher l'erreur et retourner null
                Console.WriteLine($"Error: {response.ReasonPhrase}");
                return null;
            }
        }

    }
}
