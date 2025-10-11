using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelWebApp.Mobile.Models;
using HotelWebApp.Mobile.Services;

namespace HotelWebApp.Mobile.ViewModels
{
    [QueryProperty(nameof(ReservationId), "ReservationId")]
    public partial class ReservationDetailViewModel : ObservableObject
    {
        private readonly ReservationService _reservationService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private int reservationId;

        [ObservableProperty]
        private ReservationDto? reservation;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public ReservationDetailViewModel(
            ReservationService reservationService,
            INotificationService notificationService)
        {
            _reservationService = reservationService;
            _notificationService = notificationService;
        }

        partial void OnReservationIdChanged(int value)
        {
            LoadReservationDetailsAsync().ConfigureAwait(false);
        }

        private async Task LoadReservationDetailsAsync()
        {
            if (IsBusy || ReservationId == 0) return;

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var result = await _reservationService.GetReservationDetailsAsync(ReservationId);

                if (result.Success && result.Data != null)
                {
                    Reservation = result.Data;

                    // Agendar lembrete de check-in se for uma reserva confirmada
                    if (Reservation.Status?.ToLower() == "confirmed")
                    {
                        await _notificationService.ScheduleCheckInReminderAsync(
                            Reservation.Id,
                            Reservation.RoomNumber,
                            Reservation.CheckInDate
                        );
                    }
                }
                else
                {
                    ErrorMessage = result.Message ?? "Failed to load reservation details";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"❌ Error loading reservation: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CancelReservationAsync()
        {
            if (Reservation == null || !Reservation.CanCancel) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Cancel Reservation",
                "Are you sure you want to cancel this reservation?",
                "Yes, Cancel",
                "No");

            if (!confirm) return;

            IsBusy = true;

            try
            {
                var result = await _reservationService.CancelReservationAsync(ReservationId);

                if (result.Success)
                {
                    // Cancelar lembrete de check-in
                    await _notificationService.CancelNotificationAsync(2000 + ReservationId);

                    await Shell.Current.DisplayAlert("Success", "Reservation cancelled successfully", "OK");

                    // Navegar de volta à lista
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", result.Message ?? "Failed to cancel reservation", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Error cancelling reservation: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CheckInAsync()
        {
            if (Reservation == null || !Reservation.CanCheckIn) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Check-In",
                "Confirm check-in for this reservation?",
                "Yes, Check-In",
                "Cancel");

            if (!confirm) return;

            IsBusy = true;

            try
            {
                var result = await _reservationService.CheckInReservationAsync(ReservationId);

                if (result.Success)
                {
                    System.Diagnostics.Debug.WriteLine("Check-in successful");

                    // Mostrar notificação de check-in bem-sucedido
                    await _notificationService.ShowCheckInSuccessAsync(Reservation.RoomNumber);

                    // Cancelar lembrete (já fez check-in)
                    await _notificationService.CancelNotificationAsync(2000 + ReservationId);

                    await Shell.Current.DisplayAlert("Success", $"Check-in successful! Welcome to Room {Reservation.RoomNumber}!", "OK");

                    // Navegar de volta à lista (resolve o problema do botão)
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", result.Message ?? "Failed to check-in", "OK");
                    System.Diagnostics.Debug.WriteLine($"Check-in failed: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Exception during check-in: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}