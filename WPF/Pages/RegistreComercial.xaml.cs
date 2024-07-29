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
    /// Lógica de interacción para Dashboard.xaml
    /// </summary>
    public partial class RegistreComercial : Page
    {
        public RegistreComercial()
        {
            InitializeComponent();
        }
        private static readonly HttpClient client = new HttpClient();
        private async void Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SecondNavigationTabTextBox.Text))
            {
                MessageBox.Show("Entrez le Numero du telephone");
                return;
            }

            if (string.IsNullOrEmpty(FirstNavigationTabTextBox.Text))
            {
                MessageBox.Show("Entrez le nom du client.");
                return;
            }

            var comercial = new Comercial
            {
                Nom = FirstNavigationTabTextBox.Text,
                Telephone = SecondNavigationTabTextBox.Text
            };

            var result = await AddComercialAsync(comercial);
            if (result.IsSuccessStatusCode)
            {
                MessageBox.Show("Enregistrement réussi !");
            }
            else
            {
                var errorContent = await result.Content.ReadAsStringAsync();
                HandleError(result.StatusCode, errorContent);
            }
        }

        private async Task<HttpResponseMessage> AddComercialAsync(Comercial comercialDto)
        {
            var content = new StringContent(JsonConvert.SerializeObject(comercialDto), Encoding.UTF8, "application/json");

            return await client.PostAsync("https://localhost:7210/api/Commerces", content);
        }

        private void HandleError(System.Net.HttpStatusCode statusCode, string errorContent)
        {
            // Afficher le contenu brut de l'erreur pour le débogage
            Console.WriteLine("Error Content: " + errorContent);

            // Analyser le contenu de l'erreur pour afficher les messages appropriés
            var errorMessages = ExtractErrorMessages(errorContent);

            if (errorMessages.Length > 0)
            {
                MessageBox.Show(string.Join("\n", errorMessages));
            }
            else
            {
                MessageBox.Show($"An error occurred: {errorContent}");
            }
        }

        private string[] ExtractErrorMessages(string errorContent)
        {
            try
            {
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                if (errorResponse?.Errors != null)
                {
                    return errorResponse.Errors.SelectMany(e => e.Value).ToArray();
                }
            }
            catch (Exception ex)
            {
                // Afficher les erreurs de désérialisation pour le débogage
                Console.WriteLine("Deserialization Error: " + ex.Message);
            }

            // Si la désérialisation échoue ou que Errors est null, retourner le contenu brut de l'erreur
            return new[] { errorContent };
        }
    }
    public class Comercial
    {
        public string Nom { get; set; }
        public string Telephone { get; set; }
    }

    public class ErrorResponse
    {
        public Dictionary<string, string[]> Errors { get; set; }
    }
}
