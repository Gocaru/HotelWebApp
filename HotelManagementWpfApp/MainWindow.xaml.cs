using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HotelManagementWpfApp_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();
            IniciarRelogio(); // Ativa o relógio no topo
        }

        private void IniciarRelogio()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            lblClock.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("pt-PT"));
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new UserControls.UCDashboard();
        }

        private void BtnRooms_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new TextBlock
            {
                Text = "UC Rooms loaded!",
                FontSize = 24,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
        }

        private void BtnGuests_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new UserControls.UCGuests();
        }

        private void BtnBookings_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new TextBlock
            {
                Text = "UC Bookings loaded!",
                FontSize = 24,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
        }

        private void BtnExtras_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new TextBlock
            {
                Text = "UC Extras loaded!",
                FontSize = 24,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
        }

        private void BtnInvoices_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new TextBlock
            {
                Text = "UC Invoices loaded!",
                FontSize = 24,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
        }

        private void BtnCredits_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new TextBlock
            {
                Text = "UC Credits loaded!",
                FontSize = 24,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
        }

    }
}