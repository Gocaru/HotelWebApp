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
    public partial class UCInvoices : UserControl
    {
        private readonly ApiService apiService = new ApiService("http://hoteldbapi.somee.com/api");
        private readonly NetworkService networkService = new NetworkService("http://hoteldbapi.somee.com/api/invoices");

        private List<Booking> bookings;

        public UCInvoices()
        {
            InitializeComponent();
            cbAddPaymentMethod.ItemsSource = Enum.GetValues(typeof(InvoicePaymentMethod));
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await CarregarFaturasAsync();
            await CarregarBookingsAsync();
        }

        private async Task CarregarFaturasAsync()
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
                var faturas = await apiService.GetAllAsync<Invoice>("invoices");
                var reservas = await apiService.GetAllAsync<Booking>("bookings");
                var hospedes = await apiService.GetAllAsync<Guest>("guests");

                if (faturas != null && reservas != null && hospedes != null)
                {
                    foreach (var fatura in faturas)
                    {
                        var booking = reservas.Find(b => b.BookingId == fatura.BookingId);
                        if (booking != null)
                        {
                            fatura.CheckIn = booking.CheckInDate;
                            fatura.CheckOut = booking.CheckOutDate;

                            var guest = hospedes.Find(g => g.GuestId == booking.GuestId);
                            fatura.GuestName = guest != null ? guest.Name : "Unknown";
                        }
                    }

                    dgInvoices.ItemsSource = faturas;
                }
                else
                {
                    dgInvoices.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading invoices:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task CarregarBookingsAsync()
        {
            try
            {
                var todasAsFaturas = await apiService.GetAllAsync<Invoice>("invoices");
                var todasAsReservas = await apiService.GetAllAsync<Booking>("bookings");

                if (todasAsFaturas != null && todasAsReservas != null)
                {
                    // Obter IDs de reservas que já têm fatura emitida
                    var idsComFatura = new HashSet<int>(todasAsFaturas.Select(f => f.BookingId));

                    // Filtrar apenas as reservas ainda não faturadas
                    bookings = todasAsReservas
                        .Where(b => !idsComFatura.Contains(b.BookingId))
                        .ToList();

                    cbAddBooking.ItemsSource = bookings;
                    cbAddBooking.DisplayMemberPath = "BookingId";
                    cbAddBooking.SelectedValuePath = "BookingId";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading bookings:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void BtnAddInvoice_Click(object sender, RoutedEventArgs e)
        {
            pnlAddInvoice.Visibility = Visibility.Visible;
            pnlInvoiceList.Visibility = Visibility.Collapsed;

            cbAddBooking.SelectedIndex = -1;
            cbAddPaymentMethod.SelectedIndex = -1;
            txtAddExtrasTotal.Clear();
        }

        private void BtnCancelAdd_Click(object sender, RoutedEventArgs e)
        {
            pnlAddInvoice.Visibility = Visibility.Collapsed;
            pnlInvoiceList.Visibility = Visibility.Visible;
        }

        private async void BtnConfirmAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!(cbAddBooking.SelectedItem is Booking selectedBooking))
            {
                MessageBox.Show("Please select a booking.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!InvoiceInputBuilder.TryBuildInvoice(
                    selectedBooking.BookingId.ToString(),
                    txtAddExtrasTotal.Text,
                    cbAddPaymentMethod.SelectedItem,
                    out Invoice invoice,
                    out string error))
            {
                MessageBox.Show(error, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Preencher StayTotal e Total com base na reserva
            decimal stayTotal = (decimal)(selectedBooking.CheckOutDate - selectedBooking.CheckInDate).TotalDays * 50; // Exemplo fixo
            invoice.StayTotal = stayTotal;
            invoice.Total = stayTotal + invoice.ExtrasTotal;

            try
            {
                bool success = await apiService.PostAsync("invoices", invoice);

                if (success)
                {
                    MessageBox.Show("Invoice created successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    pnlAddInvoice.Visibility = Visibility.Collapsed;
                    pnlInvoiceList.Visibility = Visibility.Visible;
                    await CarregarFaturasAsync();
                }
                else
                {
                    MessageBox.Show("Failed to create invoice.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to the API:\n" + ex.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void cbAddBooking_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbAddBooking.SelectedItem is Booking selectedBooking)
            {
                try
                {
                    // Obter todos os quartos
                    var quartos = await apiService.GetAllAsync<Room>("rooms");
                    var quarto = quartos?.Find(q => q.RoomId == selectedBooking.RoomId);
                    if (quarto == null)
                    {
                        MessageBox.Show("Room not found for the selected booking.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Calcular StayTotal
                    int noites = (selectedBooking.CheckOutDate - selectedBooking.CheckInDate).Days;
                    decimal stayTotal = noites * quarto.PricePerNight;

                    // Obter extras
                    var extras = await apiService.GetAllAsync<ExtraService>("extraservices");
                    decimal extrasTotal = 0;
                    if (extras != null)
                    {
                        extrasTotal = extras
                            .FindAll(ex => ex.BookingId == selectedBooking.BookingId)
                            .Sum(ex => ex.Price);
                    }

                    // Calcular Total
                    decimal total = stayTotal + extrasTotal;

                    // Preencher campos
                    txtAddStayTotal.Text = stayTotal.ToString("F2");
                    txtAddExtrasTotal.Text = extrasTotal.ToString("F2");
                    txtAddTotal.Text = total.ToString("F2");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error calculating totals:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}

