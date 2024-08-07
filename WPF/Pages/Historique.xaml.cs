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
            var query = JsonConvert.SerializeObject(codeclient);
            var url = $"https://localhost:7210/api/Historique?codeclient={codeclient}";
            var test = await client.GetAsync(url);
            if (test.IsSuccessStatusCode)
            {
                var responseString = await test.Content.ReadAsStringAsync();
                var responseDetails = JsonConvert.DeserializeObject<List<HistoriqueAchat>>(responseString);

                // La reponse 
                Console.WriteLine(responseString);
                return responseDetails;
            }
            else
            {
                Console.WriteLine($"Error: {test.ReasonPhrase}");
                return null;
            }
        }
    }
}
