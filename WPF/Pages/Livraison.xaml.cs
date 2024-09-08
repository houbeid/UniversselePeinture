using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
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
    /// Logique d'interaction pour Livraison.xaml
    /// </summary>
    public partial class Livraison : Page
    {
        public Livraison()
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

        //private void Enregistrer_Click(object sender, RoutedEventArgs e) {

        //}
        private async void Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            // Validation de saisie
            if (string.IsNullOrEmpty(CodeTextBox.Text) ||
                string.IsNullOrEmpty(Quantite1TextBox.Text))
            {
                MessageBox.Show("Tous les champs marqués d'un * sont obligatoires.");
                return;
            }

            if (!int.TryParse(CashTextBox.Text, out int commercantId))
            {
                MessageBox.Show("Montant doit être un nombre entier.");
                return;
            }

            AddCommandDto facture = new AddCommandDto
            {
                CodeClient = CodeTextBox.Text,
                Command_date = Delivery_Date.SelectedDate.Value,
                A_Payer = decimal.Parse(apayerTextBox.Text, System.Globalization.CultureInfo.InvariantCulture),
                cach = decimal.Parse(CashTextBox.Text, System.Globalization.CultureInfo.InvariantCulture),

                StockCommanddto = new List<StockCommandDto>
                    {
                        new StockCommandDto { NameProduit = ((ComboBoxItem)Prod1.SelectedItem).Content.ToString(), Quantite = int.Parse(Quantite1TextBox.Text) }
                    }
            };

            if (Prod2.SelectedItem != null && !string.IsNullOrWhiteSpace(Quantite2TextBox.Text))
            {
                facture.StockCommanddto.Add(new StockCommandDto
                {
                    NameProduit = ((ComboBoxItem)Prod2.SelectedItem).Content.ToString(),
                    Quantite = int.Parse(Quantite2TextBox.Text)
                });
            }

            // Ajouter produit 3 si disponible
            if (Prod3.SelectedItem != null && !string.IsNullOrWhiteSpace(Quantite3TextBox.Text))
            {
                facture.StockCommanddto.Add(new StockCommandDto
                {
                    NameProduit = ((ComboBoxItem)Prod3.SelectedItem).Content.ToString(),
                    Quantite = int.Parse(Quantite3TextBox.Text)
                });
            }

            var result = await AddCommandAsync(facture);

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
        private async void ShowPdfInPopup(string fileUrl)
        {
            // Ajouter l'ID en tant que paramètre de requête à l'URL
            DateTime date = DateTime.Now.Date;
            string formattedDate = date.ToString("yyyy-MM-dd");
            string urlWithId = $"{fileUrl}?commandDate={formattedDate}";

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
                var pdfPath = System.IO.Path.GetTempFileName() + ".pdf";

                // Écrire le flux dans un fichier temporaire
                using (var fileStream = new FileStream(pdfPath, FileMode.Create, FileAccess.Write))
                {
                    await pdfStream.CopyToAsync(fileStream);
                }

                // Naviguer vers le fichier PDF dans le contrôleur WebBrowser
                PdfViewer.Navigate(new Uri(pdfPath));
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

        private void SuiviCommand_Click(object sender, RoutedEventArgs e)
        {
            ShowPdfInPopup("https://localhost:7210/api/Command/GenerateCommandPdf");
        }
        private async Task<HttpResponseMessage> AddCommandAsync(AddCommandDto Facture)
        {
            var content = new StringContent(JsonConvert.SerializeObject(Facture), Encoding.UTF8, "application/json");

            return await client.PostAsync("https://localhost:7210/api/Command", content);
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
    public class AddCommandDto
    {
        public string CodeClient { get; set; }

        public DateTime Command_date { get; set; }
        public decimal A_Payer { get; set; }

        public decimal cach { get; set; }



        public List<StockCommandDto> StockCommanddto { get; set; }
    }

    public class StockCommandDto
    {
        public string NameProduit { get; set; }
        public int Quantite { get; set; }
    }


}
