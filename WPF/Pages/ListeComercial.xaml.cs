using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System;
using System.IO;
using System.Net.Http.Headers;

namespace WPFModernVerticalMenu.Pages
{
    /// <summary>
    /// Logique d'interaction pour ListeComercial.xaml
    /// </summary>
    public partial class ListeComercial : Page
    {

        private static readonly HttpClient client = new HttpClient();


        private bool _isPopupOpen;
        public ObservableCollection<Commercial> Commercials { get; set; }

        public bool IsPopupOpen
        {
            get { return _isPopupOpen; }
            set
            {
                _isPopupOpen = value;
                OnPropertyChanged(nameof(IsPopupOpen));
            }
        }

        public ICommand ShowRecapCommand { get; }
        public ICommand ShowPortefeuilleCommand { get; }
        public ICommand RecapCommand { get; }
        public ListeComercial()
        {
            InitializeComponent();
            Commercials = new ObservableCollection<Commercial>();
            DataContext = this;
            LoadCommercials();
            ShowRecapCommand = new RelayCommand(RecapButton_Click);
            ShowPortefeuilleCommand = new RelayCommand(PortefeuilleButton_Click);

        }

        //private async void ShowPdfInPopup(string fileUrl, int id)
        //{
        //    // Ajouter l'ID en tant que paramètre de requête à l'URL
        //    string urlWithId = $"{fileUrl}?idcomerce={id}";

        //    // Ouvrir la popup
        //    PdfPopup.IsOpen = true;

        //    try
        //    {
        //        // Créer un client HTTP
        //        var client = new HttpClient();
        //        var request = new HttpRequestMessage(HttpMethod.Get, urlWithId);

        //        // Ajouter l'en-tête Authorization avec le token JWT
        //        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.Token);

        //        // Effectuer la requête GET avec l'URL mise à jour
        //        var response = await client.SendAsync(request);

        //        // Vérifier si la réponse est réussie
        //        if (!response.IsSuccessStatusCode)
        //        {
        //            // Lire le message d'erreur depuis le backend
        //            var errorMessage = await response.Content.ReadAsStringAsync();
        //            MessageBox.Show($"Error: {errorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //            return;
        //        }

        //        // Lire le flux de réponse PDF
        //        var pdfStream = await response.Content.ReadAsStreamAsync();
        //        var pdfPath = System.IO.Path.GetTempFileName() + ".pdf";

        //        // Écrire le flux dans un fichier temporaire
        //        using (var fileStream = new FileStream(pdfPath, FileMode.Create, FileAccess.Write))
        //        {
        //            await pdfStream.CopyToAsync(fileStream);
        //        }

        //        // Naviguer vers le fichier PDF dans le contrôleur WebBrowser
        //        PdfViewer.Navigate(new Uri(pdfPath));
        //    }
        //    catch (HttpRequestException httpEx)
        //    {
        //        // Erreur de réseau ou d'appel HTTP
        //        MessageBox.Show($"HTTP Request Error: {httpEx.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Toute autre exception non gérée
        //        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //    finally
        //    {
        //        // Fermer la popup en cas d'erreur
        //        if (!PdfPopup.IsOpen)
        //        {
        //            PdfPopup.IsOpen = false;
        //        }
        //    }
        //}

        private string _currentPdfUrl;

        private async void ShowPdfInPopup(string fileUrl, int id)
        {
            // Ajouter l'ID en tant que paramètre de requête à l'URL
            string urlWithId = $"{fileUrl}?idcomerce={id}";
            _currentPdfUrl = urlWithId;  // Stocke l'URL du PDF actuellement visualisé

            // Ouvrir la popup
            PdfPopup.IsOpen = true;

            try
            {
                // Créer un client HTTP
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlWithId);

                // Ajouter l'en-tête Authorization avec le token JWT
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.Token);

                // Effectuer la requête GET avec l'URL mise à jour
                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
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
                MessageBox.Show($"HTTP Request Error: {httpEx.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentPdfUrl))
            {
                MessageBox.Show("Aucun PDF à télécharger.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Créer un client HTTP pour télécharger le PDF
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, _currentPdfUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.Token);

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    // Lire le flux de réponse
                    var pdfStream = await response.Content.ReadAsStreamAsync();

                    // Obtenir le chemin du dossier Téléchargements de l'utilisateur
                    string userDownloadsPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

                    // Générer un nom unique avec la date et l'heure actuelles
                    string uniqueFileName = $"fichier_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"; // Exemple: fichier_20241010_153045.pdf
                    string savePath = System.IO.Path.Combine(userDownloadsPath, uniqueFileName);

                    using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                    {
                        await pdfStream.CopyToAsync(fileStream);
                    }

                    MessageBox.Show($"PDF téléchargé avec succès : {savePath}", "Téléchargement terminé", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Erreur lors du téléchargement du fichier.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du téléchargement : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        //private void ShowRecap(object parameter)
        //{
        //    // Logic to load recap data
        //    PdfPopup.IsOpen = true;
        //}



        //private void ShowPortefeuille(object parameter)
        //{
        //    // Logic to load portefeuille data
        //    PdfPopup.IsOpen = true;
        //}

        private void RecapButton_Click(object sender, RoutedEventArgs e)
        {
            // Utilisez une URL de votre API pour obtenir le PDF
            Button button = sender as Button;
            if (button != null)
            {
                int commercialId = (int)button.Tag;
                // Logique pour afficher le récapitulatif du commercial
                ShowPdfInPopup("https://universellepeintre.oneposts.io/api/Commerces/GenerateRecettePdf", commercialId);
            }
            
        }

        private void PortefeuilleButton_Click(object sender, RoutedEventArgs e)
        {
            // Utilisez une URL de votre API pour obtenir le PDF
            Button button = sender as Button;
            if (button != null)
            {
                int commercialId = (int)button.Tag;
                // Logique pour afficher le récapitulatif du commercial
                ShowPdfInPopup("https://universellepeintre.oneposts.io/api/Commerces/GenerateRecapPdf", commercialId);
            }
            
        }

        //private void DownloadButton_Click(object sender, RoutedEventArgs e)
        //{
        //    // Logic to download the PDF file
        //    MessageBox.Show("Download button clicked");
        //}

        private async void LoadCommercials()
        {
            // Créer une requête GET pour récupérer l'historique du client

            var request = new HttpRequestMessage(HttpMethod.Get, "https://universellepeintre.oneposts.io/api/Commerces");

            // Ajouter l'en-tête Authorization avec le token JWT
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.Token);

            // Envoyer la requête


            // Ajouter l'en-tête Authorization avec le token JWT
            var response = await client.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var commercials = JsonConvert.DeserializeObject<List<Commercial>>(jsonString);
                if (commercials != null)
                {
                    foreach (var commercial in commercials)
                    {
                        Commercials.Add(commercial);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Commercial
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Telephone { get; set; }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;
        private Action<object, RoutedEventArgs> recapButton_Click;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Action<object, RoutedEventArgs> recapButton_Click)
        {
            this.recapButton_Click = recapButton_Click;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
