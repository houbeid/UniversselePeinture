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
    /// Logique d'interaction pour ListeComercial.xaml
    /// </summary>
    public partial class ListeComercial : Page
    {
        public ListeComercial()
        {
            InitializeComponent();
        }

        private void RecapButton_Click(object sender, RoutedEventArgs e)
        {
            // Logic to load PDF file into the viewer
            // Open the popup
            PdfPopup.IsOpen = true;
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            // Logic to download the PDF file
            MessageBox.Show("Download button clicked");
        }
    }
}
