using HotelManagementWpfApp_.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace HotelManagementWpfApp_.UserControls
{
    /// <summary>
    /// Interaction logic for UCDashboard.xaml
    /// </summary>
    public partial class UCDashboard : UserControl
    {
        public UCDashboard()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7176/api/");

                    HttpResponseMessage response = await client.GetAsync("dashboard");

                    if (response.IsSuccessStatusCode)
                    {
                        DashboardStats stats = await response.Content.ReadFromJsonAsync<DashboardStats>();

                        lblTotalGuests.Text = stats.TotalGuests.ToString();
                        lblTotalRooms.Text = stats.TotalRooms.ToString();
                        lblTotalBookings.Text = stats.TotalBookings.ToString();
                        lblTotalInvoices.Text = stats.TotalInvoices.ToString();
                        lblTotalRevenue.Text = stats.TotalRevenue.ToString("C"); // Formato moeda
                    }
                    else
                    {
                        MessageBox.Show("Erro ao obter dados do dashboard (HTTP " + (int)response.StatusCode + ").");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao comunicar com a API:\n" + ex.Message);
            }
        }

    }
}
