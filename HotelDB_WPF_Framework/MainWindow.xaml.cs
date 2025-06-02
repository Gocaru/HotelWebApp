using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace HotelDB_WPF_Framework
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
            MainContentControl.Content = new UserControls.UCRooms();
        }

        private void BtnGuests_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new UserControls.UCGuests();
        }

        private void BtnBookings_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new UserControls.UCBookings();
        }

        private void BtnExtras_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new UserControls.UCExtras();
        }

        private void BtnInvoices_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new UserControls.UCInvoices();
        }

        private void BtnCredits_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new UserControls.UCAbout();
        }

    }
}
