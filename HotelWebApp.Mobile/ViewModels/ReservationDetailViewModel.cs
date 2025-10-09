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

        [ObservableProperty]
        private int reservationId;

        [ObservableProperty]
        private ReservationDto? reservation;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public ReservationDetailViewModel(ReservationService reservationService)
        {
            _reservationService = reservationService;
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
                }
                else
                {
                    ErrorMessage = result.Message ?? "Failed to load reservation details";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
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
                    await Shell.Current.DisplayAlert("Success", "Reservation cancelled successfully", "OK");
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
                    await Shell.Current.DisplayAlert("Success", "Check-in successful! Welcome!", "OK");

                    // Navegar de volta e voltar para forçar reload completo
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", result.Message ?? "Failed to check-in", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
