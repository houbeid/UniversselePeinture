using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
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
    /// Logique d'interaction pour SFacture.xaml
    /// </summary>
    public partial class SFacture : Page
    {
        public SFacture()
        {
            InitializeComponent();
        }

        private static readonly HttpClient client = new HttpClient();

        private bool _isPopupOpen;

        public bool IsPopupOpen
        {
            get { return _isPopupOpen; }
            set
            {
                _isPopupOpen = value;
                OnPropertyChanged(nameof(IsPopupOpen));
            }
        }

        private void ClearInputs()
        {
            CodeClientBox.Text = string.Empty;
            NumFactTextBox.Text  = string.Empty;
            MontantTextBox.Text = string.Empty;
            DistrubuteurTextBox.Text = string.Empty;
        }
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
                ClearInputs();
            }
            else
            {
                var errorContent = await result.Content.ReadAsStringAsync();
                HandleError(result.StatusCode, errorContent);
            }
        }

        private async void ShowPdfInPopup(string fileUrl)
        {
            // Ajouter l'ID en tant que paramètre de requête à l'URL
            DateTime date = DateTime.Now.Date;
            string formattedDate = date.ToString("yyyy-MM-dd");
            string urlWithId = $"{fileUrl}?FactureDate={formattedDate}";

            // Ouvrir la popup
            PdfPopup.IsOpen = true;

            try
            {
                // Créer un client HTTP
                var client = new HttpClient();

                // Effectuer la requête GET avec l'URL mise à jour
                var response = await client.GetAsync(urlWithId);

                // Vérifier si la réponse est réussie
                if (!response.IsSuccessStatusCode)
                {
                    // Lire le message d'erreur depuis le backend
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error: {errorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Lire le flux de réponse PDF
                var pdfStream = await response.Content.ReadAsStreamAsync();

                // Utiliser un chemin temporaire valide pour n'importe quel PC
                string tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"fichier_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

                // Écrire le flux dans un fichier temporaire
                using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
                {
                    await pdfStream.CopyToAsync(fileStream);
                }

                // Naviguer vers le fichier PDF dans le contrôleur WebBrowser
                PdfViewer.Navigate(new Uri(tempPath));
            }
            catch (HttpRequestException httpEx)
            {
                // Erreur de réseau ou d'appel HTTP
                MessageBox.Show($"HTTP Request Error: {httpEx.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Toute autre exception non gérée
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Fermer la popup en cas d'erreur
                if (!PdfPopup.IsOpen)
                {
                    PdfPopup.IsOpen = false;
                }
            }
        }


        private void SuiviFact_Click(object sender, RoutedEventArgs e)
        {
            ShowPdfInPopup("https://universellepeintre.oneposts.io/api/Facture/GenerateFacturePdf");
        }
        private async Task<HttpResponseMessage> AddfactureAsync(AddFactureDto facture)
        {
            try
            {
                // Sérialiser l'objet Facture en JSON
                var content = new StringContent(JsonConvert.SerializeObject(facture), Encoding.UTF8, "application/json");

                // Créer une requête POST pour ajouter une facture
                var request = new HttpRequestMessage(HttpMethod.Post, "https://universellepeintre.oneposts.io/api/Facture/Add");

                // Ajouter l'en-tête Authorization avec le token JWT
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.Token);

                // Attacher le contenu JSON à la requête
                request.Content = content;

                // Envoyer la requête et retourner la réponse
                return await client.SendAsync(request);
            }
            catch (Exception ex)
            {
                // Gérer les exceptions pour éviter un crash
                Console.WriteLine($"Erreur lors de l'ajout de la facture : {ex.Message}");
                throw; // Relancer l'exception pour que l'appelant puisse la gérer
            }
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
