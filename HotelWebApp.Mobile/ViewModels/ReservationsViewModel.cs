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
        private bool isRefreshing;

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
            IsRefreshing = true;
            ErrorMessage = string.Empty;

            try
            {
                System.Diagnostics.Debug.WriteLine("🔵 ReservationsViewModel: Starting to load...");

                var result = await _reservationService.GetMyReservationsAsync();

                System.Diagnostics.Debug.WriteLine($"🔵 ReservationsViewModel: Got result. Success: {result.Success}");

                if (result.Success && result.Data != null)
                {
                    System.Diagnostics.Debug.WriteLine($"🔵 ReservationsViewModel: Processing {result.Data.Count} reservations");

                    Reservations.Clear();
                    foreach (var reservation in result.Data.OrderByDescending(r => r.CheckInDate))
                    {
                        System.Diagnostics.Debug.WriteLine($"   - Adding reservation #{reservation.Id}");
                        Reservations.Add(reservation);
                    }

                    HasReservations = Reservations.Any();

                    System.Diagnostics.Debug.WriteLine($"✅ ReservationsViewModel: Done. HasReservations: {HasReservations}");
                }
                else
                {
                    ErrorMessage = result.Message ?? "Failed to load reservations";
                    HasReservations = false;

                    System.Diagnostics.Debug.WriteLine($"❌ ReservationsViewModel: Failed. Message: {ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                HasReservations = false;

                System.Diagnostics.Debug.WriteLine($"❌ ReservationsViewModel: Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;  // Desativar refresh
                System.Diagnostics.Debug.WriteLine("🔵 ReservationsViewModel: IsBusy = false");
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
