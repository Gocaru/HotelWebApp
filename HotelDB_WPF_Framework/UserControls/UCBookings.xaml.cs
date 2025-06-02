using HotelDB_WPF_Framework.Enums;
using HotelDB_WPF_Framework.Helpers;
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
    public partial class UCBookings : UserControl
    {
        private readonly NetworkService networkService = new NetworkService("http://hoteldbapi.somee.com/api/bookings");

        private readonly ApiService apiService = new ApiService("http://hoteldbapi.somee.com/api");

        private Booking reservaSelecionada;
        
        private List<Guest> hospedes;
        
        private List<Room> quartos;

        public UCBookings()
        {
            InitializeComponent();

            cbAddRoomType.ItemsSource = Enum.GetValues(typeof(RoomType));
            cbEditRoomType.ItemsSource = Enum.GetValues(typeof(RoomType));
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await CarregarHospedesEQuartosAsync();
            await CarregarReservasAsync();;
        }

        private async Task CarregarHospedesEQuartosAsync()
        {
            try
            {
                hospedes = await apiService.GetAllAsync<Guest>("guests");
                quartos = await apiService.GetAllAsync<Room>("rooms");

                if (hospedes != null)
                {
                    cbAddGuest.ItemsSource = hospedes;
                    cbAddGuest.DisplayMemberPath = "Name";
                    cbAddGuest.SelectedValuePath = "GuestId";

                    cbEditGuest.ItemsSource = hospedes;
                    cbEditGuest.DisplayMemberPath = "Name";
                    cbEditGuest.SelectedValuePath = "GuestId";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading guests or rooms: " + ex.Message);
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            pnlAddBooking.Visibility = Visibility.Visible;
            pnlBookingList.Visibility = Visibility.Collapsed;

            txtAddGuests.Clear();
            dpAddCheckIn.SelectedDate = null;
            dpAddCheckOut.SelectedDate = null;
        }

        private void BtnCancelAdd_Click(object sender, RoutedEventArgs e)
        {
            pnlAddBooking.Visibility = Visibility.Collapsed;
            pnlBookingList.Visibility = Visibility.Visible;
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

            if (!(cbAddGuest.SelectedItem is Guest guest))
            {
                MessageBox.Show("Please select a guest.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!(cbAddRoomType.SelectedItem is RoomType selectedType))
            {
                MessageBox.Show("Please select a room type.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtAddGuests.Text, out int numGuests) || numGuests <= 0)
            {
                MessageBox.Show("Enter a valid number of guests.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!dpAddCheckIn.SelectedDate.HasValue || !dpAddCheckOut.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select both check-in and check-out dates.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime checkIn = dpAddCheckIn.SelectedDate.Value;
            DateTime checkOut = dpAddCheckOut.SelectedDate.Value;

            if (checkIn >= checkOut)
            {
                MessageBox.Show("Check-out must be after check-in.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Obter lista de quartos
            List<Room> rooms = await apiService.GetAllAsync<Room>("rooms");

            if (rooms == null || rooms.Count == 0)
            {
                MessageBox.Show("No rooms available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            List<Booking> reservas = await apiService.GetAllAsync<Booking>("bookings");

            // Filtrar quartos disponíveis para o tipo e capacidade
            var quartosDisponiveis = rooms.Where(r =>
                r.Type == selectedType &&
                r.Capacity >= numGuests &&
                r.Status == RoomStatus.Available &&
                BookingHelper.EstaDisponivel(r.RoomId, checkIn, checkOut, reservas)
            ).ToList();

            if (quartosDisponiveis.Count == 0)
            {
                MessageBox.Show("No available room found for the selected type and number of guests.", "No Availability", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Selecionar o primeiro quarto livre (poderia usar lógica mais avançada)
            var quartoEscolhido = quartosDisponiveis[0];

            // Usar o builder para construir a reserva base (sem RoomId)
            if (!BookingInputBuilder.TryBuildBooking(
                guest,
                txtAddGuests.Text,
                selectedType,
                dpAddCheckIn.SelectedDate,
                dpAddCheckOut.SelectedDate,
                out Booking novaReserva,
                out string error))
            {
                MessageBox.Show(error, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Atribuir RoomId com base na disponibilidade
            novaReserva.RoomId = quartoEscolhido.RoomId;

            // Validar
            if (!ValidationHelper.ValidateBooking(novaReserva, out error))
            {
                MessageBox.Show(error, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                bool success = await apiService.PostAsync("bookings", novaReserva);

                if (success)
                {
                    MessageBox.Show("Booking added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    pnlAddBooking.Visibility = Visibility.Collapsed;
                    pnlBookingList.Visibility = Visibility.Visible;
                    await CarregarReservasAsync();
                }
                else
                {
                    MessageBox.Show("Failed to add booking.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to the API:\n" + ex.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Carrega a lista de reservas da API e preenche a grelha.
        /// </summary>
        private async Task CarregarReservasAsync()
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
                // Carregar reservas da API
                List<Booking> reservas = await apiService.GetAllAsync<Booking>("bookings");

                if (reservas == null || reservas.Count == 0)
                {
                    dgBookings.ItemsSource = new List<Booking>();
                    return;
                }

                // Preencher propriedades auxiliares (GuestName, RoomTypeName, RoomNumber)
                foreach (var reserva in reservas)
                {
                    var hospede = hospedes?.FirstOrDefault(h => h.GuestId == reserva.GuestId);
                    reserva.GuestName = hospede != null ? hospede.Name : "Unknown";

                    var quarto = quartos?.FirstOrDefault(q => q.RoomId == reserva.RoomId);
                    reserva.RoomTypeName = quarto != null ? quarto.Type.ToString() : "Unknown";
                    reserva.RoomNumber = quarto != null ? quarto.Number : 0;
                }

                dgBookings.ItemsSource = reservas;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading bookings:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void dgBookings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnEditBooking.IsEnabled = dgBookings.SelectedItem != null;
            btnDeleteBooking.IsEnabled = dgBookings.SelectedItem != null;
        }

        private void BtnCancelEdit_Click(object sender, RoutedEventArgs e)
        {
            pnlEditBooking.Visibility = Visibility.Collapsed;
            pnlBookingList.Visibility = Visibility.Visible;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            reservaSelecionada = dgBookings.SelectedItem as Booking;

            if (reservaSelecionada == null)
            {
                MessageBox.Show("Please select a booking.");
                return;
            }

            // Mostrar painel de edição
            pnlEditBooking.Visibility = Visibility.Visible;
            pnlBookingList.Visibility = Visibility.Collapsed;

            // Preencher os campos com os dados atuais
            cbEditGuest.SelectedValue = reservaSelecionada.GuestId;
            txtEditGuests.Text = reservaSelecionada.NumberOfGuests.ToString();
            cbEditRoomType.SelectedItem = quartos.FirstOrDefault(q => q.RoomId == reservaSelecionada.RoomId)?.Type;
            dpEditCheckIn.SelectedDate = reservaSelecionada.CheckInDate;
            dpEditCheckOut.SelectedDate = reservaSelecionada.CheckOutDate;
        }

        private async void BtnConfirmEdit_Click(object sender, RoutedEventArgs e)
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

            if (reservaSelecionada == null)
            {
                MessageBox.Show("No booking selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Obter os dados do formulário (com casts seguros)
            if (!(cbEditGuest.SelectedItem is Guest guest))
            {
                MessageBox.Show("Please select a guest.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!(cbEditRoomType.SelectedItem is RoomType selectedType))
            {
                MessageBox.Show("Please select a room type.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Construir objeto temporário da reserva com os dados introduzidos
            if (!BookingInputBuilder.TryBuildBooking(
                guest,
                txtEditGuests.Text,
                selectedType,
                dpEditCheckIn.SelectedDate,
                dpEditCheckOut.SelectedDate,
                out Booking novaReserva,
                out string erro))
            {
                MessageBox.Show(erro, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Obter todas as reservas para validar conflitos
            List<Booking> todasAsReservas = await apiService.GetAllAsync<Booking>("bookings");

            if (todasAsReservas == null)
            {
                MessageBox.Show("Could not retrieve bookings for availability check.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Procurar um quarto disponível que cumpra os critérios
            Room quartoDisponivel = quartos.FirstOrDefault(q =>
                q.Type == selectedType &&
                q.Status == RoomStatus.Available &&
                BookingHelper.EstaDisponivel(q.RoomId, novaReserva.CheckInDate, novaReserva.CheckOutDate, todasAsReservas, reservaSelecionada.BookingId)
            );

            if (quartoDisponivel == null)
            {
                MessageBox.Show("No available room found with the selected type and date range.", "No Availability", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Atualizar a reserva selecionada com os novos dados
            reservaSelecionada.GuestId = guest.GuestId;
            reservaSelecionada.RoomId = quartoDisponivel.RoomId;
            reservaSelecionada.CheckInDate = novaReserva.CheckInDate;
            reservaSelecionada.CheckOutDate = novaReserva.CheckOutDate;
            reservaSelecionada.NumberOfGuests = novaReserva.NumberOfGuests;
            reservaSelecionada.Status = BookingStatus.Reserved;

            try
            {
                bool success = await apiService.PutAsync($"bookings/{reservaSelecionada.BookingId}", reservaSelecionada);

                if (success)
                {
                    MessageBox.Show("Booking updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    pnlEditBooking.Visibility = Visibility.Collapsed;
                    pnlBookingList.Visibility = Visibility.Visible;
                    await CarregarReservasAsync();
                }
                else
                {
                    MessageBox.Show("Failed to update booking.", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating booking:\n" + ex.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedBooking = dgBookings.SelectedItem as Booking;

            if (selectedBooking == null)
            {
                MessageBox.Show("Please select a booking.");
                return;
            }

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

            var confirm = MessageBox.Show("Are you sure you want to delete this booking?", "Confirm", MessageBoxButton.YesNo);

            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                bool success = await apiService.DeleteAsync("bookings", selectedBooking.BookingId);

                if (success)
                {
                    MessageBox.Show("Booking deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    await CarregarReservasAsync();
                }
                else
                {
                    MessageBox.Show("Failed to delete booking.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting booking:\n" + ex.Message);
            }
        }
    }
}
