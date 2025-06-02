using HotelDB_WPF_Framework.Models;
using HotelDB_WPF_Framework.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace HotelDB_WPF_Framework.UserControls
{
    /// <summary>
    /// Interaction logic for UCDashboard.xaml
    /// </summary>
    public partial class UCDashboard : UserControl
    {
        private readonly ApiService apiService = new ApiService("http://hoteldbapi.somee.com/api/");

        public UCDashboard()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var stats = await apiService.GetByIdAsync<DashboardStats>("dashboard", 0); 


                if (stats != null)
                {
                    lblTotalGuests.Text = stats.TotalGuests.ToString();
                    lblTotalRooms.Text = stats.TotalRooms.ToString();
                    lblTotalBookings.Text = stats.TotalBookings.ToString();
                    lblTotalInvoices.Text = stats.TotalInvoices.ToString();
                    lblTotalRevenue.Text = stats.TotalRevenue.ToString("C", System.Globalization.CultureInfo.CurrentCulture);
                }
                else
                {
                    MessageBox.Show("No dashboard data received.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to API:\n" + ex.Message, "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

