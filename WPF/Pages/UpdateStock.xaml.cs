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
    /// Logique d'interaction pour UpdateStock.xaml
    /// </summary>
    public partial class UpdateStock : Page
    {
        public UpdateStock()
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

        private void PriseCompta_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                decimal number;
                if (!decimal.TryParse(textBox.Text, out number))
                {
                    MessageBox.Show("La valeur doit être un nombre décimal.", "Erreur de validation", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            // Valider les données avant de créer l'objet
            if (ValidateInputs())
            {
                // Créer l'objet AddStockdto et le remplir avec les données saisies
                UpdateStockdto stockDto = new UpdateStockdto
                {
                    CodeClient = CodeClient.Text,
                    Visit_date = Visite_Date.SelectedDate.Value,
                    Description = description.Text,
                    StockProduitdto = new List<StockProduitdto>
                    {
                        new StockProduitdto { NameProduit = ((ComboBoxItem)Prod1.SelectedItem).Content.ToString(), Quantite = int.Parse(QtP1.Text) }
                    }
                };

                // Ajouter produit 2 si disponible
                if (Prod2.SelectedItem != null && !string.IsNullOrWhiteSpace(QtP2.Text))
                {
                    stockDto.StockProduitdto.Add(new StockProduitdto
                    {
                        NameProduit = ((ComboBoxItem)Prod2.SelectedItem).Content.ToString(),
                        Quantite = int.Parse(QtP2.Text)
                    });
                }

                // Ajouter produit 3 si disponible
                if (Prod3.SelectedItem != null && !string.IsNullOrWhiteSpace(QtP3.Text))
                {
                    stockDto.StockProduitdto.Add(new StockProduitdto
                    {
                        NameProduit = ((ComboBoxItem)Prod3.SelectedItem).Content.ToString(),
                        Quantite = int.Parse(QtP3.Text)
                    });
                }

                var result = await UpdateStockAsync(stockDto);

                // Traiter l'objet stockDto (ex: l'envoyer à un service, le sauvegarder dans une base de données, etc.)
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
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(CodeClient.Text))
            {
                MessageBox.Show("Le Code Client ne peut pas être vide.", "Erreur de validation", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Visite_Date.SelectedDate.HasValue)
            {
                MessageBox.Show("La Date de Délivration ne peut pas être vide.", "Erreur de validation", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            int qtP1;
            if (!int.TryParse(QtP1.Text, out qtP1))
            {
                MessageBox.Show("La Quantité 1 doit être un nombre entier.", "Erreur de validation", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if ((Prod2.SelectedItem != null && string.IsNullOrWhiteSpace(QtP2.Text)) ||
                (!string.IsNullOrWhiteSpace(QtP2.Text) && !int.TryParse(QtP2.Text, out _)))
            {
                MessageBox.Show("La Quantité 2 doit être un nombre entier si un produit est sélectionné.", "Erreur de validation", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if ((Prod3.SelectedItem != null && string.IsNullOrWhiteSpace(QtP3.Text)) ||
                (!string.IsNullOrWhiteSpace(QtP3.Text) && !int.TryParse(QtP3.Text, out _)))
            {
                MessageBox.Show("La Quantité 3 doit être un nombre entier si un produit est sélectionné.", "Erreur de validation", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void ProcessStockDto(AddStockdto stockDto)
        {
            // Logique pour traiter l'objet stockDto (ex: l'envoyer à un service, le sauvegarder dans une base de données, etc.)
            MessageBox.Show("Les données ont été ajoutées avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task<HttpResponseMessage> UpdateStockAsync(UpdateStockdto stock)
        {
            var content = new StringContent(JsonConvert.SerializeObject(stock), Encoding.UTF8, "application/json");

            return await client.PostAsync("https://localhost:7210/api/Stock/update", content);
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

    public class UpdateStockdto
    {
        public string CodeClient { get; set; }

        public decimal recipe_day { get; set; }

        public DateTime Visit_date { get; set; }

        public string Description { get; set; }

        public List<StockProduitdto> StockProduitdto { get; set; }
    }
}
