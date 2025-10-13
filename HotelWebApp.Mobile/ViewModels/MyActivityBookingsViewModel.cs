using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelWebApp.Mobile.Models;
using HotelWebApp.Mobile.Services;
using HotelWebApp.Mobile.Views;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using HotelWebApp.Mobile.Messages;

namespace HotelWebApp.Mobile.ViewModels
{
    public partial class MyActivityBookingsViewModel : ObservableObject
    {
        private readonly ActivityService _activityService;

        [ObservableProperty]
        private ObservableCollection<ActivityBookingDto> bookings = new();

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private bool isCancelling;

        [ObservableProperty]
        private bool hasBookings;

        [ObservableProperty]
        private bool hasCancelledBookings;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        // Chave para guardar preferência
        private const string HideCancelledKey = "HideCancelledBookings";

        // Propriedade que lê e grava em Preferences
        private bool HideCancelled
        {
            get => Preferences.Get(HideCancelledKey, false);
            set => Preferences.Set(HideCancelledKey, value);
        }

        public MyActivityBookingsViewModel(ActivityService activityService)
        {
            _activityService = activityService;

            // Registar listener para atualizações automáticas
            WeakReferenceMessenger.Default.Register<ActivityBookedMessage>(this, async (recipient, message) =>
            {
                System.Diagnostics.Debug.WriteLine($"📨 Activity booked: {message.ActivityName}");
                await Task.Delay(300);
                await LoadBookingsInternalAsync();
            });
        }

        public async Task InitializeAsync()
        {
            await LoadBookingsAsync();
        }

        [RelayCommand]
        private async Task LoadBookingsAsync()
        {
            if (IsBusy) return;
            await LoadBookingsInternalAsync();
        }

        private async Task LoadBookingsInternalAsync()
        {
            IsBusy = true;
            IsRefreshing = false;
            ErrorMessage = string.Empty;

            try
            {
                System.Diagnostics.Debug.WriteLine("📥 Loading activity bookings...");

                // ✅ RETRY LOGIC - Tentar até 3 vezes
                ApiResponse<List<ActivityBookingDto>> result = null;
                int maxRetries = 3;

                for (int attempt = 1; attempt <= maxRetries; attempt++)
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"   Attempt {attempt}/{maxRetries}");
                        result = await _activityService.GetMyBookingsAsync();

                        if (result != null && result.Success)
                        {
                            System.Diagnostics.Debug.WriteLine($"✅ Loaded successfully on attempt {attempt}");
                            break;
                        }

                        System.Diagnostics.Debug.WriteLine($"⚠️ Attempt {attempt}: Result not successful");

                        // Se não for a última tentativa, aguardar antes de tentar novamente
                        if (attempt < maxRetries)
                        {
                            await Task.Delay(500 * attempt); // 500ms, 1s, 1.5s
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ Attempt {attempt} failed: {ex.Message}");

                        // Se não for a última tentativa, aguardar antes de tentar novamente
                        if (attempt < maxRetries)
                        {
                            await Task.Delay(500 * attempt);
                        }
                        else
                        {
                            throw; // Última tentativa falhou, propagar exceção
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Result Success: {result?.Success ?? false}");
                System.Diagnostics.Debug.WriteLine($"Data count: {result?.Data?.Count ?? 0}");

                if (result?.Success == true && result.Data != null)
                {
                    Bookings.Clear();
                    await Task.Delay(50);

                    var bookingsToShow = HideCancelled
                        ? result.Data.Where(b => !b.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                        : result.Data;

                    var orderedBookings = bookingsToShow.OrderByDescending(b => b.ScheduledDate).ToList();

                    foreach (var booking in orderedBookings)
                    {
                        System.Diagnostics.Debug.WriteLine($"   Adding: {booking.ActivityName} - {booking.ScheduledDate:dd/MM/yyyy} - {booking.Status}");
                        Bookings.Add(booking);
                    }

                    HasBookings = Bookings.Any();
                    HasCancelledBookings = Bookings.Any(b => b.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase));
                    System.Diagnostics.Debug.WriteLine($"✅ Loaded {Bookings.Count} bookings (hideCancelled={HideCancelled})");
                }
                else
                {
                    ErrorMessage = result?.Message ?? "Failed to load bookings";
                    HasBookings = false;
                    System.Diagnostics.Debug.WriteLine($"❌ Error: {ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Connection error: {ex.Message}";
                HasBookings = false;
                System.Diagnostics.Debug.WriteLine($"❌ Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   StackTrace: {ex.StackTrace}");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task CancelBookingAsync(ActivityBookingDto booking)
        {
            if (booking == null || IsCancelling) return;

            System.Diagnostics.Debug.WriteLine($"Cancel booking: {booking.ActivityBookingId}");

            bool confirm = await Shell.Current.DisplayAlert(
                "Cancel Booking",
                $"Are you sure you want to cancel your booking for {booking.ActivityName}?",
                "Yes",
                "No");

            if (!confirm)
            {
                System.Diagnostics.Debug.WriteLine("⚠User cancelled");
                return;
            }

            IsCancelling = true;
            ErrorMessage = string.Empty;

            try
            {
                var result = await _activityService.CancelBookingAsync(booking.ActivityBookingId);

                System.Diagnostics.Debug.WriteLine($"Result: {result.Success}");

                if (result.Success)
                {
                    System.Diagnostics.Debug.WriteLine("Cancelled, reloading...");

                    await LoadBookingsInternalAsync();

                    await Shell.Current.DisplayAlert("Success", "Booking cancelled successfully", "OK");
                }
                else
                {
                    ErrorMessage = result.Message ?? "Failed to cancel";
                    await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
                System.Diagnostics.Debug.WriteLine($"❌ Exception: {ex.Message}");
            }
            finally
            {
                IsCancelling = false;
            }
        }

        [RelayCommand]
        private void ClearCancelledBookings()
        {
            System.Diagnostics.Debug.WriteLine("Activating hide cancelled filter");

            var cancelledBookings = Bookings
                .Where(b => b.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var booking in cancelledBookings)
            {
                System.Diagnostics.Debug.WriteLine($"Removing: {booking.ActivityName}");
                Bookings.Remove(booking);
            }

            HideCancelled = true;

            HasBookings = Bookings.Any();
            HasCancelledBookings = false;
            System.Diagnostics.Debug.WriteLine($"Cleared {cancelledBookings.Count}, filter saved to Preferences");
        }

        [RelayCommand]
        private async Task NavigateToActivitiesAsync()
        {
            await Shell.Current.GoToAsync(nameof(ActivitiesPage));
        }

        // DESTRUCTOR - Cleanup quando ViewModel for destruído
        ~MyActivityBookingsViewModel()
        {
            WeakReferenceMessenger.Default.Unregister<ActivityBookedMessage>(this);
        }
    }
}