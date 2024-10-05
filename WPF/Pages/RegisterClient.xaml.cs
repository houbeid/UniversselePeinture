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
using System.Net.Http.Headers;

namespace WPFModernVerticalMenu.Pages
{
    /// <summary>
    /// Lógica de interacción para Home.xaml
    /// </summary>
    public partial class RegisterClient : Page
    {
        public RegisterClient()
        {
            InitializeComponent();
        }

        private static readonly HttpClient client = new HttpClient();
        private async void Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            // Validation de saisie
            if (string.IsNullOrEmpty(CodeTextBox.Text) ||
                string.IsNullOrEmpty(Respnsible_NameTextBox.Text) ||
                string.IsNullOrEmpty(ZoneTextBox.Text) ||
                string.IsNullOrEmpty(CommercantIdTextBox.Text) ||
                string.IsNullOrEmpty(TelephoneTextBox.Text))
            {
                MessageBox.Show("Tous les champs marqués par * sont obligatoires.");
                return;
            }

            if (!int.TryParse(CommercantIdTextBox.Text, out int commercantId))
            {
                MessageBox.Show("CommercantId doit être un nombre entier.");
                return;
            }

            // Enregistrement des donnée
            var clientDto = new Client
            {
                Code = CodeTextBox.Text,
                Name_Society = Name_SocietyTextBox.Text,
                Respnsible_Name = Respnsible_NameTextBox.Text,
                CoordonneesGPS = CoordonnéesGPSTextBox.Text,
                Zone = ZoneTextBox.Text,
                Recommandation = RecommandationTextBox.Text,
                CommercantId = commercantId,
                Phone_Number = TelephoneTextBox.Text
            };

            var result = await AddClientAsync(clientDto);
            if (result.IsSuccessStatusCode)
            {
                MessageBox.Show("Enregistrement réussi !");
            }
            else
            {
                MessageBox.Show("Erreur du l'inscription");
            }

            
        }

        private async Task<HttpResponseMessage> AddClientAsync(Client clientDto)
        {
            try
            {
                // Convertir l'objet clientDto en JSON
                var content = new StringContent(JsonConvert.SerializeObject(clientDto), Encoding.UTF8, "application/json");

                // Créer une requête POST pour ajouter un client
                var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7210/api/Clients");

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
                Console.WriteLine($"Erreur lors de l'ajout du client : {ex.Message}");
                throw; // Relancer l'exception pour que l'appelant puisse la gérer
            }
        }
    }
    public class Client
    {

        public string Code { get; set; }
        public string NomEntreprise { get; set; }

        public string Name_Society { get; set; }

        public string Respnsible_Name { get; set; }
        public string Phone_Number { get; set; }
        public string Proprietaire { get; set; }
        public string ContactPerson { get; set; }
        public string Gerant { get; set; }
        public string Solvabilite { get; set; }
        public string CoordonneesGPS { get; set; }
        public string Zone { get; set; }
        public string Recommandation { get; set; }
        public int CommercantId { get; set; }
    }

    
}
