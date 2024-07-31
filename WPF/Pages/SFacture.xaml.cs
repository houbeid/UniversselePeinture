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
    /// Logique d'interaction pour SFacture.xaml
    /// </summary>
    public partial class SFacture : Page
    {
        public SFacture()
        {
            InitializeComponent();
        }
        private void EnregistrerFact_Click(object sender, RoutedEventArgs e)
        {
            // Validation de saisie
            //if (string.IsNullOrEmpty(DateBox.Text) ||
            //    string.IsNullOrEmpty(ClientBox.Text) ||
            //    string.IsNullOrEmpty(AddresseTextBox.Text) ||
            //    string.IsNullOrEmpty(CodeTextBox.Text) ||
            //    string.IsNullOrEmpty(NumFactTextBox.Text) ||
            //    string.IsNullOrEmpty(MontantTextBox.Text) ||
            //    string.IsNullOrEmpty(DistrubuteurTextBox.Text))
            //{
            //    MessageBox.Show("Tous les champs marqués d'un * sont obligatoires.");
            //    return;
            //}

            //if (!int.TryParse(MontantTextBox.Text, out int commercantId))
            //{
            //    MessageBox.Show("Montant doit être un nombre entier.");
            //    return;
            //}

            //// Enregistrement des donnée
            //var commercant = new Client
            //{
            //    Date = DateBox.Text,
            //    Name_Society = Name_SocietyTextBox.Text,
            //    Respnsible_Name = Respnsible_NameTextBox.Text,
            //    CoordonneesGPS = CoordonnéesGPSTextBox.Text,
            //    Zone = ZoneTextBox.Text,
            //    Recommandation = RecommandationTextBox.Text,
            //    CommercantId = commercantId
            //};

            MessageBox.Show("Enregistrement réussi !");
        }
        private void SuiviFact_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
