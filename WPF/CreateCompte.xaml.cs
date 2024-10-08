using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
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

namespace WPFModernVerticalMenu
{
    /// <summary>
    /// Logique d'interaction pour CreateCompte.xaml
    /// </summary>
    public partial class CreateCompte : Window, INotifyPropertyChanged
    {
        private bool _isAdmin;

        public bool IsAdmin
        {
            get { return _isAdmin; }
            set
            {
                _isAdmin = value;
                OnPropertyChanged();
            }
        }

        public CreateCompte()
        {
            InitializeComponent();
            DataContext = this;
            IsAdmin = false; // Default value
        }

        private static readonly HttpClient client = new HttpClient();

        private async void Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            // Validation de saisie
            if (string.IsNullOrEmpty(IdTextBox.Text) ||
                string.IsNullOrEmpty(PasswordTextBox.Text) ||
                string.IsNullOrEmpty(PhoneTextBox.Text))
            {
                MessageBox.Show("Tous les champs marqués par * sont obligatoires.");
                return;
            }

            // Enregistrement des donnée
            var UserDto = new AddUserDto
            {
                userName = IdTextBox.Text,
                password = PasswordTextBox.Text,
                phone = PhoneTextBox.Text,
                email = EmailTextBox.Text,
                IsAdmin = IsAdmin
            };

            var result = await AddComptAsync(UserDto);
            if (result.IsSuccessStatusCode)
            {
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
                MessageBox.Show("Enregistrement réussi !");
            }
            else
            {
                MessageBox.Show("Erreur du l'inscription");
            }


        }

        private async Task<HttpResponseMessage> AddComptAsync(AddUserDto userdto)
        {
            var content = new StringContent(JsonConvert.SerializeObject(userdto), Encoding.UTF8, "application/json");

            return await client.PostAsync("https://universellepeintre.oneposts.io/api/User/Register", content);
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
    }

    public class AddUserDto
    {
        
        public string userName { get; set; } = string.Empty;

        
        public string password { get; set; } = string.Empty;
        public string email { get; set; }

        public string phone { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }
    }
}
