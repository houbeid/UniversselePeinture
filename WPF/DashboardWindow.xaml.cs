using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class DashboardWindow : Window
    {

        public ObservableCollection<Notification> Notifications { get; set; }
        public DashboardWindow(List<ClientResponse> clients)
        {

            InitializeComponent();

            Notifications = new ObservableCollection<Notification>();
            DataContext = this;
            foreach (var client in clients)
            {
                Notifications.Add(new Notification { CommercialId = client.CommercantId, ClientName = client.Respnsible_Name });
            }

        }

        public void AddNotification(int CommercantId, string clientName)
        {
            Notifications.Add(new Notification { CommercialId = CommercantId, ClientName = clientName });
        }
        private void BG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Tg_Btn.IsChecked = true;
        }

        // Start: MenuLeft PopupButton //
        private void btnHome_MouseEnter(object sender, MouseEventArgs e)
        {
            //if (Tg_Btn.IsChecked == false)
            //{
            Popup.PlacementTarget = btnHome;
            //Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "Home";
            // }
        }

        private void btnHome_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void btnDashboard_MouseEnter(object sender, MouseEventArgs e)
        {
            //if (Tg_Btn.IsChecked == false)
            //{
            Popup.PlacementTarget = btnDashboard;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "Dashboard";
            //}
        }

        private void btnDashboard_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void btnProducts_MouseEnter(object sender, MouseEventArgs e)
        {
            //if (Tg_Btn.IsChecked == false)
            //{
            Popup.PlacementTarget = btnProducts;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "Products";
            //}
        }

        private void btnProducts_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void btnProductStock_MouseEnter(object sender, MouseEventArgs e)
        {
            //if (Tg_Btn.IsChecked == false)
            //{
            Popup.PlacementTarget = btnStatistique;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "Product Stock";
            //}
        }

        private void btnProductStock_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        //private void btnOrderList_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    //if (Tg_Btn.IsChecked == false)
        //    //{
        //    Popup.PlacementTarget = btnOrderList;
        //    Popup.Placement = PlacementMode.Right;
        //    Popup.IsOpen = true;
        //    Header.PopupText.Text = "Order List";
        //    // }
        //}

        //private void btnOrderList_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    Popup.Visibility = Visibility.Collapsed;
        //    Popup.IsOpen = false;
        //}

        //private void btnBilling_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    //if (Tg_Btn.IsChecked == false)
        //    //{
        //    Popup.PlacementTarget = btnBilling;
        //    Popup.Placement = PlacementMode.Right;
        //    Popup.IsOpen = true;
        //    Header.PopupText.Text = "Billing";
        //    //}
        //}

        //private void btnBilling_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    Popup.Visibility = Visibility.Collapsed;
        //    Popup.IsOpen = false;
        //}

        //private void btnPointOfSale_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    //if (Tg_Btn.IsChecked == false)
        //    //{
        //    Popup.PlacementTarget = btnPointOfSale;
        //    Popup.Placement = PlacementMode.Right;
        //    Popup.IsOpen = true;
        //    Header.PopupText.Text = "Poin Of Sale";
        //    //}
        //}

        //private void btnPointOfSale_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    Popup.Visibility = Visibility.Collapsed;
        //    Popup.IsOpen = false;
        //}

        //private void btnSecurity_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    //if (Tg_Btn.IsChecked == false)
        //    //{
        //    Popup.PlacementTarget = btnSecurity;
        //    Popup.Placement = PlacementMode.Right;
        //    Popup.IsOpen = true;
        //    Header.PopupText.Text = "Security";
        //    //}
        //}

        //private void btnSecurity_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    Popup.Visibility = Visibility.Collapsed;
        //    Popup.IsOpen = false;
        //}
        //private void btnSetting_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    //if (Tg_Btn.IsChecked == false)
        //    //{
        //    Popup.PlacementTarget = btnSetting;
        //    Popup.Placement = PlacementMode.Right;
        //    Popup.IsOpen = true;
        //    Header.PopupText.Text = "Setting";
        //    // }
        //}

        //private void btnSetting_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    Popup.Visibility = Visibility.Collapsed;
        //    Popup.IsOpen = false;
        //}
        // End: MenuLeft PopupButton //

        // Start: Button Close | Restore | Minimize 
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        // End: Button Close | Restore | Minimize

        private void btnRegistreCl_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("Pages/RegisterClient.xaml", UriKind.RelativeOrAbsolute));
            Button b = sender as Button;
            b.Foreground = new SolidColorBrush(Colors.Yellow);
        }

        private void btnRegistreCo_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("Pages/RegistreComercial.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnLivraison_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("Pages/Livraison.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnAddStock_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("Pages/AddStock.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnUpdateStock_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("Pages/UpdateStock.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnListCom_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("Pages/ListeComercial.xaml", UriKind.RelativeOrAbsolute));
        }
        private void btnHistorique_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("Pages/Historique.xaml", UriKind.RelativeOrAbsolute));
        }
        private void btnSuiviFact_Clic(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("Pages/SFacture.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnStatistique_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("Pages/Statistique.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnNotification_Click(object sender, RoutedEventArgs e)
        {
            // Toggle the visibility of the notification popup
            if (notificationPopup.IsOpen)
            {
                notificationPopup.IsOpen = false;
            }
            else
            {
                notificationPopup.IsOpen = true;
            }
        }

        private void fContainer_Navigated(object sender, NavigationEventArgs e)
        {

        }

        private void MenuItem_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Handle logout button click
            //fContainer.Navigate(new System.Uri("MainWindow.xaml", UriKind.RelativeOrAbsolute));
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
    public class Notification
    {
        public int CommercialId { get; set; }
        public string ClientName { get; set; }

        public string Message
        {
            get
            {
                return $"{CommercialId} doit visiter {ClientName}";
            }
        }
    }
}

