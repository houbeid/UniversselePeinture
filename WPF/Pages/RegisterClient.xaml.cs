using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Lógica de interacción para Home.xaml
    /// </summary>
    public partial class RegisterClient : Page
    {
        public RegisterClient()
        {
            InitializeComponent();
        }
        private void Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            // Validation de saisie
            if (string.IsNullOrEmpty(CodeTextBox.Text) ||
                string.IsNullOrEmpty(Name_SocietyTextBox.Text) ||
                string.IsNullOrEmpty(Respnsible_NameTextBox.Text) ||
                string.IsNullOrEmpty(ZoneTextBox.Text) ||
                string.IsNullOrEmpty(CommercantIdTextBox.Text))
            {
                MessageBox.Show("Tous les champs marqués d'un * sont obligatoires.");
                return;
            }

            if (!int.TryParse(CommercantIdTextBox.Text, out int commercantId))
            {
                MessageBox.Show("CommercantId doit être un nombre entier.");
                return;
            }

            // Enregistrement des donnée
            var commercant = new Client
            {
                Code = CodeTextBox.Text,
                Name_Society = Name_SocietyTextBox.Text,
                Respnsible_Name = Respnsible_NameTextBox.Text,
                CoordonneesGPS = CoordonnéesGPSTextBox.Text,
                Zone = ZoneTextBox.Text,
                Recommandation = RecommandationTextBox.Text,
                CommercantId = commercantId
            };

            MessageBox.Show("Enregistrement réussi !");
        }
    }
    public class Client
    {

        public string Code { get; set; }
        public string NomEntreprise { get; set; }

        public string Name_Society { get; set; }

        public string Respnsible_Name { get; set; }
        public string Telephone { get; set; }
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
