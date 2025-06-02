using HotelDB_WPF_Framework.Enums;
using HotelDB_WPF_Framework.Helpers.InputBuilders;
using HotelDB_WPF_Framework.Models;
using HotelDB_WPF_Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HotelDB_WPF_Framework.UserControls
{
    public partial class UCExtras : UserControl
    {
        private readonly ApiService apiService = new ApiService("http://hoteldbapi.somee.com/api");
        private readonly NetworkService networkService = new NetworkService("http://hoteldbapi.somee.com/api/extraservices");

        private List<Guest> hospedes;
        private List<Booking> bookings;

        public UCExtras()
        {
            InitializeComponent();

            cbAddServiceType.ItemsSource = Enum.GetValues(typeof(ExtraServiceType));
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await CarregarExtrasAsync();
            await CarregarHospedesEBookingsAsync();
        }

        private async Task CarregarHospedesEBookingsAsync()
        {
            if (!networkService.IsInternetAvailable())
            {
                MessageBox.Show("No internet connection.", "Network Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!await networkService.IsApiAvailableAsync())
            {
                MessageBox.Show("The API is currently unavailable.", "API Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                hospedes = await apiService.GetAllAsync<Guest>("guests");
                bookings = await apiService.GetAllAsync<Booking>("bookings");

                if (bookings != null && hospedes != null)
                {
                    // Adicionar propriedade auxiliar ao objeto Booking (GuestName)
                    foreach (var booking in bookings)
                    {
                        var guest = hospedes.FirstOrDefault(g => g.GuestId == booking.GuestId);
                        booking.GuestName = guest != null ? guest.Name : "Unknown";
                    }

                    cbAddBooking.ItemsSource = bookings;
                    cbAddBooking.DisplayMemberPath = "GuestName";
                    cbAddBooking.SelectedValuePath = "BookingId";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading guests or bookings:\n" + ex.Message);
            }
        }


        private async Task CarregarExtrasAsync()
        {
            if (!networkService.IsInternetAvailable())
            {
                MessageBox.Show("No internet connection.", "Network Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!await networkService.IsApiAvailableAsync())
            {
                MessageBox.Show("API is unavailable.", "API Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var extras = await apiService.GetAllAsync<ExtraService>("extraservices");

                if (extras != null)
                    dgExtras.ItemsSource = extras;
                else
                    dgExtras.ItemsSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading extra services:\n" + ex.Message);
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            pnlAddExtra.Visibility = Visibility.Visible;
            pnlExtrasList.Visibility = Visibility.Collapsed;
            LimparFormulario();
        }

        private async void BtnConfirmAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!networkService.IsInternetAvailable())
            {
                MessageBox.Show("No internet connection.", "Network Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!await networkService.IsApiAvailableAsync())
            {
                MessageBox.Show("The API is currently unavailable.", "API Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!(cbAddBooking.SelectedValue is int bookingId))
            {
                MessageBox.Show("Please select a valid booking.");
                return;
            }

            if (!ExtraServiceInputBuilder.TryBuildExtraService(
                bookingId.ToString(),
                cbAddServiceType.SelectedItem,
                txtAddPrice.Text,
                out ExtraService extra,
                out string error))
            {
                MessageBox.Show(error, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                bool success = await apiService.PostAsync("extraservices", extra);

                if (success)
                {
                    MessageBox.Show("Extra service added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    await CarregarExtrasAsync();
                    LimparFormulario();

                    // Voltar ao painel principal
                    pnlAddExtra.Visibility = Visibility.Collapsed;
                    pnlExtrasList.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Failed to add extra service.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to API:\n" + ex.Message);
            }
        }

        private void BtnCancelAdd_Click(object sender, RoutedEventArgs e)
        {
            pnlAddExtra.Visibility = Visibility.Collapsed;
            pnlExtrasList.Visibility = Visibility.Visible;
            LimparFormulario();
        }

        private void LimparFormulario()
        {
            cbAddBooking.SelectedIndex = -1;
            cbAddServiceType.SelectedIndex = -1;
            txtAddPrice.Clear();
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!networkService.IsInternetAvailable())
            {
                MessageBox.Show("No internet connection.", "Network Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!await networkService.IsApiAvailableAsync())
            {
                MessageBox.Show("The API is currently unavailable.", "API Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selected = dgExtras.SelectedItem as ExtraService;
            if (selected == null)
            {
                MessageBox.Show("Please select an extra service to delete.");
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to delete this service?", "Confirm", MessageBoxButton.YesNo);

            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                bool success = await apiService.DeleteAsync("extraservices", selected.ExtraServiceId);

                if (success)
                {
                    MessageBox.Show("Extra service deleted.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    await CarregarExtrasAsync();
                }
                else
                {
                    MessageBox.Show("Failed to delete service.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting service:\n" + ex.Message);
            }
        }

        private void dgExtras_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnDeleteExtra.IsEnabled = dgExtras.SelectedItem != null;
        }
    }
}

