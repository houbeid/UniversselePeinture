using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Logique d'interaction pour Recette.xaml
    /// </summary>
    public partial class Recette : Page
    {
        public Recette()
        {
            InitializeComponent();
        }

        private static readonly HttpClient client = new HttpClient();
        private void Delivery_Date_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            if (datePicker != null && datePicker.SelectedDate.HasValue)
            {
                DateTime selectedDate = datePicker.SelectedDate.Value;
                // Valider le format de la date
                if (!DateTime.TryParseExact(selectedDate.ToString("dd.MM.yyyy"), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    MessageBox.Show("Le format de la date doit être 'dd.MM.yyyy'.", "Erreur de validation", MessageBoxButton.OK, MessageBoxImage.Error);
                    datePicker.SelectedDate = null;
                }
            }
        }
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
         
            var  recette = new RecetteDto
            {
                CodeClient = FirstNavigationTabTextBox.Text,
                priseCompta = decimal.Parse(SecondNavigationTabTextBox.Text, System.Globalization.CultureInfo.InvariantCulture),
                Recette_Date = Recette_Date.SelectedDate.Value

            };

            var result = await AddrecetteAsync(recette);
            if (result.IsSuccessStatusCode)
            {
                MessageBox.Show("Enregistrement réussi !");
                FirstNavigationTabTextBox.Text = string.Empty;
                SecondNavigationTabTextBox.Text = string.Empty;
            }
            else
            {
                MessageBox.Show("Erreur du l'insertion");
                FirstNavigationTabTextBox.Text = string.Empty;
                SecondNavigationTabTextBox.Text = string.Empty;
            }
        }

        private async Task<HttpResponseMessage> AddrecetteAsync(RecetteDto recette)
        {
            try
            {
                // Sérialiser l'objet recette en JSON
                var content = new StringContent(JsonConvert.SerializeObject(recette), Encoding.UTF8, "application/json");

                // Créer une requête POST pour ajouter la recette
                var request = new HttpRequestMessage(HttpMethod.Post, "https://universellepeintre.oneposts.io/api/Stock/recette");

                // Ajouter l'en-tête Authorization avec le token JWT
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.Token);

                // Attacher le contenu JSON à la requête
                request.Content = content;

                // Envoyer la requête et retourner la réponse
                return await client.SendAsync(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout de la recette : {ex.Message}");
                throw; // Relancer l'exception pour que l'appelant puisse la gérer
            }
        }

    }
    public class RecetteDto
    {
        public string CodeClient { get; set; }
        public decimal priseCompta { get; set; }
        public DateTime Recette_Date { get; set; }
    }
}
