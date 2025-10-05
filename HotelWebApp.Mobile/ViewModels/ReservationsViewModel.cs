using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelWebApp.Mobile.Models;
using HotelWebApp.Mobile.Services;
using HotelWebApp.Mobile.Views;
using System.Collections.ObjectModel;

namespace HotelWebApp.Mobile.ViewModels
{
    public partial class ReservationsViewModel : ObservableObject
    {
        private readonly ReservationService _reservationService;

        [ObservableProperty]
        private ObservableCollection<ReservationDto> reservations = new();

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool hasReservations;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public ReservationsViewModel(ReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        public async Task InitializeAsync()
        {
            await LoadReservationsAsync();
        }

        [RelayCommand]
        private async Task LoadReservationsAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var result = await _reservationService.GetMyReservationsAsync();

                if (result.Success && result.Data != null)
                {
                    Reservations.Clear();
                    foreach (var reservation in result.Data.OrderByDescending(r => r.CheckInDate))
                    {
                        Reservations.Add(reservation);
                    }

                    HasReservations = Reservations.Any();
                }
                else
                {
                    ErrorMessage = result.Message ?? "Failed to load reservations";
                    HasReservations = false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                HasReservations = false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ViewReservationDetailsAsync(ReservationDto reservation)
        {
            if (reservation == null) return;

            await Shell.Current.GoToAsync(nameof(ReservationDetailPage), new Dictionary<string, object>
            {
                { "ReservationId", reservation.Id }
            });
        }
    }
}
