﻿using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Newtonsoft.Json;

namespace WPFModernVerticalMenu
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private static readonly HttpClient client = new HttpClient();

        public bool IsDarkTheme { get; set; }
        private readonly PaletteHelper paletteHelper = new PaletteHelper();
        private void ToggleTheme(object sender, RoutedEventArgs e)
        {
            ITheme theme = paletteHelper.GetTheme();
            if (IsDarkTheme = theme.GetBaseTheme() == BaseTheme.Dark)
            {
                IsDarkTheme = false;
                theme.SetBaseTheme(Theme.Light);
            }
            else
            {
                IsDarkTheme = true;
                theme.SetBaseTheme(Theme.Light);
            }
            paletteHelper.SetTheme(theme);
        }
        public void exitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        private void CreateCompte_Click(object sender, RoutedEventArgs e)
        {
            CreateCompte create = new CreateCompte();
            create.Show();
            this.Close();
        }
        private async void login_Click(object sender, RoutedEventArgs e)
        {
            //        // Logique d'authentification ici
            //        // Si l'authentification réussit :

            // var result = postApiDataAsync(login, "https://52.47.142.28:5000/api/User/login");
            if (txtUsername.Text == "")
            {
                MessageBox.Show("Entrez username");
                return;
            }
            if (txtPassword.Password == "")
            {
                MessageBox.Show("Entrez mots de passe");
                return;
            }
            var result = await LoginAsync(txtUsername.Text, txtPassword.Password);
            var responseString = await result.Content.ReadAsStringAsync();
            var responseDetails = JsonConvert.DeserializeObject<Token>(responseString);
            if (result.IsSuccessStatusCode)
            {
                TokenStorage.Token = responseDetails.token;
                var content = await result.Content.ReadAsStringAsync();
                
                DashboardWindow dashboard = new DashboardWindow(responseDetails.clients);
                dashboard.Show();
                this.Close();
                MessageBox.Show("Connexion Valider!");
                // Traitez le token JWT ici si nécessaire
            }
                    else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        MessageBox.Show("Invalid username ou mots de passe!");
                    }
                    else
                    {
                        MessageBox.Show("Une eurrer est survenue");
                    }
        }

        private async Task<HttpResponseMessage> LoginAsync(string username, string password)
        {
            var loginDto = new
            {
                userName = username,
                password = password
            };

            var content = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");

            return await client.PostAsync("https://universellepeintre.oneposts.io/api/User/login", content);
        }

        private void HandleError(System.Net.HttpStatusCode statusCode, string errorContent)
        {
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
                return errorResponse.Errors.SelectMany(e => e.Value).ToArray();
            }
            catch
            {
                return new[] { "An unexpected error occurred." };
            }
        }

    }
    public class Token
    {
        public string token { get; set; }
        public DateTime expiration { get; set; }
        public List<ClientResponse> clients { get; set; }
    }
    public class ClientResponse
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name_Society { get; set; }
        public string Phone_Number { get; set; }

        public string Respnsible_Name { get; set; }
        public string Gérant { get; set; }
        public string Solvabilité { get; set; }
        public string CoordonnéesGPS { get; set; }
        public string Zone { get; set; }
        public string Recommandation { get; set; }
        public DateTime? Visit_Date { get; set; }

        public DateTime? Delivery_Date { get; set; }
        public string Description { get; set; }

        public int CommercantId { get; set; }
    }

    public class ErrorResponse
    {
        public Dictionary<string, string[]> Errors { get; set; }
    }
}

public static class TokenStorage
{
    public static string Token { get; set; }
}
