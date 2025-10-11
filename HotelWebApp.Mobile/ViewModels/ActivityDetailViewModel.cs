using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelWebApp.Mobile.Models;
using HotelWebApp.Mobile.Services;

namespace HotelWebApp.Mobile.ViewModels
{
    [QueryProperty(nameof(ActivityId), "ActivityId")]
    public partial class ActivityDetailViewModel : ObservableObject
    {
        private readonly ActivityService _activityService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private ActivityDto? activity;

        [ObservableProperty]
        private int activityId;

        [ObservableProperty]
        private int numberOfParticipants = 1;

        [ObservableProperty]
        private DateTime scheduledDate = DateTime.Today;

        [ObservableProperty]
        private ActivityAvailabilityDto? availability;

        [ObservableProperty]
        private bool isCheckingAvailability;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isBooking;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private string successMessage = string.Empty;

        // Propriedades computadas
        public decimal TotalPrice => Activity != null ? Activity.Price * NumberOfParticipants : 0;

        public DateTime MinimumDate => DateTime.Today;

        // Propriedades seguras para binding
        public string AvailableText => Availability != null
            ? $"{Availability.AvailableSpots} spots"
            : Activity != null ? $"{Activity.MaxParticipants} spots" : "Loading...";

        public Color AvailabilityColor => Availability?.AvailabilityColor ?? Colors.Gray;

        public ActivityDetailViewModel(
            ActivityService activityService,
            INotificationService notificationService)
        {
            _activityService = activityService;
            _notificationService = notificationService;
        }

        public async Task InitializeAsync()
        {
            await LoadActivityDetailsAsync();
        }

        partial void OnActivityIdChanged(int value)
        {
            if (value > 0)
            {
                _ = LoadActivityDetailsAsync();
            }
        }

        partial void OnScheduledDateChanged(DateTime value)
        {
            _ = CheckAvailabilityAsync();
        }

        partial void OnNumberOfParticipantsChanged(int value)
        {
            OnPropertyChanged(nameof(TotalPrice));
        }

        [RelayCommand]
        private async Task LoadActivityDetailsAsync()
        {
            if (IsBusy || ActivityId <= 0) return;

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var result = await _activityService.GetActivityByIdAsync(ActivityId);

                if (result.Success && result.Data != null)
                {
                    Activity = result.Data;

                    OnPropertyChanged(nameof(TotalPrice));

                    await CheckAvailabilityAsync();
                }
                else
                {
                    ErrorMessage = result.Message ?? "Failed to load activity details";
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
        private async Task CheckAvailabilityAsync()
        {
            if (ActivityId <= 0 || IsCheckingAvailability) return;

            IsCheckingAvailability = true;

            try
            {
                System.Diagnostics.Debug.WriteLine($"Checking availability for {ScheduledDate:yyyy-MM-dd}");

                var result = await _activityService.CheckAvailabilityAsync(ActivityId, ScheduledDate);

                if (result.Success && result.Data != null)
                {
                    Availability = result.Data;
                    System.Diagnostics.Debug.WriteLine($"Available: {Availability.AvailableSpots} spots");

                    OnPropertyChanged(nameof(AvailableText));
                    OnPropertyChanged(nameof(AvailabilityColor));
                }
                else
                {
                    Availability = null;
                    System.Diagnostics.Debug.WriteLine($"Failed to check availability");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                Availability = null;
            }
            finally
            {
                IsCheckingAvailability = false;
            }
        }

        [RelayCommand]
        private async Task BookActivityAsync()
        {
            if (IsBooking || Activity == null) return;

            if (NumberOfParticipants < 1)
            {
                ErrorMessage = "Number of participants must be at least 1";
                return;
            }

            if (Availability != null && Availability.IsFull)
            {
                ErrorMessage = "This activity is fully booked for the selected date";
                return;
            }

            if (Availability != null && NumberOfParticipants > Availability.AvailableSpots)
            {
                ErrorMessage = $"Only {Availability.AvailableSpots} spots available for this date";
                return;
            }

            if (ScheduledDate < DateTime.Today)
            {
                ErrorMessage = "Please select a future date";
                return;
            }

            IsBooking = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            try
            {
                var request = new CreateActivityBookingRequest
                {
                    ActivityId = ActivityId,
                    NumberOfPeople = NumberOfParticipants,
                    ScheduledDate = ScheduledDate
                };

                var result = await _activityService.BookActivityAsync(request);

                if (result.Success)
                {
                    System.Diagnostics.Debug.WriteLine("Activity booked successfully");

                    // Mostrar notificação de atividade reservada
                    await _notificationService.ShowActivityBookedAsync(
                        Activity.Name,
                        ScheduledDate
                    );

                    SuccessMessage = "Activity booked successfully!";

                    await Shell.Current.DisplayAlert(
                        "Success",
                        $"{Activity.Name} booked for {ScheduledDate:dd MMM yyyy}!",
                        "OK"
                    );

                    // Navegar de volta
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    ErrorMessage = result.Message ?? "Failed to book activity";
                    await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
                    System.Diagnostics.Debug.WriteLine($"Booking failed: {ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
            }
            finally
            {
                IsBooking = false;
            }
        }

        [RelayCommand]
        private void IncrementParticipants()
        {
            var maxAllowed = Availability?.AvailableSpots ?? Activity?.MaxParticipants ?? 1;

            if (NumberOfParticipants < maxAllowed)
            {
                NumberOfParticipants++;
            }
        }

        [RelayCommand]
        private void DecrementParticipants()
        {
            if (NumberOfParticipants > 1)
            {
                NumberOfParticipants--;
            }
        }
    }
}