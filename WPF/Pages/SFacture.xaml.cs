using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Logique d'interaction pour SFacture.xaml
    /// </summary>
    public partial class SFacture : Page
    {
        public SFacture()
        {
            InitializeComponent();
        }

        private static readonly HttpClient client = new HttpClient();
        private void Date_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
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
        private async void EnregistrerFact_Click(object sender, RoutedEventArgs e)
        {
            // Validation de saisie
            if (string.IsNullOrEmpty(NumFactTextBox.Text) ||
                string.IsNullOrEmpty(MontantTextBox.Text) ||
                string.IsNullOrEmpty(DistrubuteurTextBox.Text) ||
                string.IsNullOrEmpty(CodeClientBox.Text))
            {
                MessageBox.Show("Tous les champs marqués d'un * sont obligatoires.");
                return;
            }

            if (!int.TryParse(MontantTextBox.Text, out int commercantId))
            {
                MessageBox.Show("Montant doit être un nombre entier.");
                return;
            }

            AddFactureDto facture = new AddFactureDto
            {
                date = Date.SelectedDate.Value,
                CodeClient = CodeClientBox.Text,
                Facture = NumFactTextBox.Text,
                Montant = decimal.Parse(MontantTextBox.Text, System.Globalization.CultureInfo.InvariantCulture),
                distribiteur = DistrubuteurTextBox.Text
            };

            var result = await AddfactureAsync(facture);

            if (result.IsSuccessStatusCode)
            {
                MessageBox.Show("Les données ont été ajoutées avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                var errorContent = await result.Content.ReadAsStringAsync();
                HandleError(result.StatusCode, errorContent);
            }
        }
        private void SuiviFact_Click(object sender, RoutedEventArgs e)
        {

        }
        private async Task<HttpResponseMessage> AddfactureAsync(AddFactureDto Facture)
        {
            var content = new StringContent(JsonConvert.SerializeObject(Facture), Encoding.UTF8, "application/json");

            return await client.PostAsync("https://localhost:7210/api/Facture/Add", content);
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

    public class AddFactureDto
    {
        public DateTime date { get; set; }

        public string CodeClient { get; set; }

        public string Facture { get; set; }

        public decimal Montant { get; set; }

        public string distribiteur { get; set; }
    }
}
