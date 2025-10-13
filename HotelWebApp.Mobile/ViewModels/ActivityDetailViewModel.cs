using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelWebApp.Mobile.Models;
using HotelWebApp.Mobile.Services;
using HotelWebApp.Mobile.Views;
using CommunityToolkit.Mvvm.Messaging;
using HotelWebApp.Mobile.Messages;

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

        [ObservableProperty]
        private DateTime minimumDate = DateTime.Today;

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

            System.Diagnostics.Debug.WriteLine($"ViewModel Constructor");
            System.Diagnostics.Debug.WriteLine($"   DateTime.Now: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            System.Diagnostics.Debug.WriteLine($"   DateTime.Today: {DateTime.Today:yyyy-MM-dd HH:mm:ss}");
            System.Diagnostics.Debug.WriteLine($"   DateTime.UtcNow: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
            System.Diagnostics.Debug.WriteLine($"   ScheduledDate inicial: {scheduledDate:yyyy-MM-dd HH:mm:ss}");
            System.Diagnostics.Debug.WriteLine($"   MinimumDate: {MinimumDate:yyyy-MM-dd HH:mm:ss}");
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
            System.Diagnostics.Debug.WriteLine($" OnScheduledDateChanged");
            System.Diagnostics.Debug.WriteLine($"   OLD Value: {scheduledDate:yyyy-MM-dd HH:mm:ss zzz}");
            System.Diagnostics.Debug.WriteLine($"   NEW Value: {value:yyyy-MM-dd HH:mm:ss zzz}");
            System.Diagnostics.Debug.WriteLine($"   DateTime.Today: {DateTime.Today:yyyy-MM-dd}");
            System.Diagnostics.Debug.WriteLine($"   DateTime.UtcNow: {DateTime.UtcNow:yyyy-MM-dd}");

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

                    // Definir data inicial baseada nas reservas
                    await SetInitialDateAsync();

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

        private async Task SetInitialDateAsync()
        {
            try
            {
                var reservationService = Application.Current.Handler.MauiContext.Services.GetService<ReservationService>();
                var reservationsResult = await reservationService.GetMyReservationsAsync();

                if (reservationsResult?.Success == true && reservationsResult.Data?.Any() == true)
                {
                    var activeReservations = reservationsResult.Data
                        .Where(r => r.Status == "Confirmed" || r.Status == "CheckedIn")
                        .OrderBy(r => r.CheckInDate)
                        .ToList();

                    if (activeReservations.Any())
                    {
                        // Procurar primeira reserva que inclui hoje OU está no futuro
                        var validReservation = activeReservations
                            .FirstOrDefault(r => r.CheckInDate.Date <= DateTime.Today && r.CheckOutDate.Date >= DateTime.Today)
                            ?? activeReservations.First();

                        // Definir data inicial
                        if (validReservation.CheckInDate.Date <= DateTime.Today &&
                            validReservation.CheckOutDate.Date >= DateTime.Today)
                        {
                            // Estamos dentro da reserva - usa hoje
                            ScheduledDate = DateTime.Today;
                            MinimumDate = DateTime.Today;
                        }
                        else
                        {
                            // Reserva no futuro - usa check-in
                            ScheduledDate = validReservation.CheckInDate.Date;
                            MinimumDate = validReservation.CheckInDate.Date;
                        }

                        OnPropertyChanged(nameof(MinimumDate));
                    }
                }
            }
            catch
            {
                // Se falhar, mantém defaults
                ScheduledDate = DateTime.Today;
                MinimumDate = DateTime.Today;
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
            var token = await SecureStorage.GetAsync("auth_token");

            if (string.IsNullOrEmpty(token))
            {
                var result = await Shell.Current.DisplayAlert(
                    "Login Required",
                    "Please login or create an account to book this activity",
                    "Login",
                    "Cancel");

                if (result)
                {
                    await Shell.Current.GoToAsync(nameof(LoginPage));
                }
                return;
            }

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
                var reservationService = Application.Current.Handler.MauiContext.Services.GetService<ReservationService>();
                var reservationsResult = await reservationService.GetMyReservationsAsync();

                if (reservationsResult == null || !reservationsResult.Success || reservationsResult.Data == null)
                {
                    ErrorMessage = "Unable to verify your reservations. Please try again.";
                    await Shell.Current.DisplayAlert("Connection Error", ErrorMessage, "OK");
                    return;
                }

                var activeReservations = reservationsResult.Data
                    .Where(r => r.Status == "Confirmed" || r.Status == "CheckedIn")
                    .ToList();

                if (!activeReservations.Any())
                {
                    ErrorMessage = "You need an active room reservation to book activities";
                    await Shell.Current.DisplayAlert(
                        "No Active Reservation",
                        "You don't have any active room reservations.\n\nPlease book a room first, then you can reserve activities during your stay.",
                        "OK");
                    return;
                }

                // Encontrar automaticamente a reserva que contém a data escolhida
                var validReservation = activeReservations
                    .FirstOrDefault(r => ScheduledDate.Date >= r.CheckInDate.Date
                                      && ScheduledDate.Date <= r.CheckOutDate.Date);

                if (validReservation == null)
                {
                    var availablePeriods = string.Join("\n", activeReservations.Select(r =>
                        $"• {r.CheckInDate:dd MMM yyyy} - {r.CheckOutDate:dd MMM yyyy}"));

                    ErrorMessage = $"The selected date ({ScheduledDate:dd MMM yyyy}) is not within any of your active reservations";

                    await Shell.Current.DisplayAlert(
                        "Invalid Date",
                        $"Activities can only be booked during your reservation period.\n\nYour active reservations:\n{availablePeriods}\n\nPlease select a date within these periods.",
                        "OK");
                    return;
                }

                // Criar data sem timezone
                var scheduledDateOnly = new DateTime(
                    ScheduledDate.Year,
                    ScheduledDate.Month,
                    ScheduledDate.Day,
                    0, 0, 0,
                    DateTimeKind.Unspecified
                );

                var request = new CreateActivityBookingRequest
                {
                    ActivityId = ActivityId,
                    NumberOfPeople = NumberOfParticipants,
                    ScheduledDate = scheduledDateOnly,
                    ReservationId = validReservation.Id
                };

                var result = await _activityService.BookActivityAsync(request);

                if (result.Success)
                {
                    // Notificação local
                    await _notificationService.ShowActivityBookedAsync(
                        Activity.Name,
                        ScheduledDate
                    );

                    SuccessMessage = "Activity booked successfully!";

                    // Enviar mensagem para outros ViewModels
                    WeakReferenceMessenger.Default.Send(new ActivityBookedMessage
                    {
                        ActivityBookingId = result.Data?.ActivityBookingId ?? 0,
                        ActivityName = Activity.Name,
                        ScheduledDate = ScheduledDate,
                        NumberOfPeople = NumberOfParticipants,
                        TotalPrice = TotalPrice
                    });

                    await Shell.Current.DisplayAlert(
                        "Success",
                        $"{Activity.Name} booked for {ScheduledDate:dd MMM yyyy}!",
                        "OK"
                    );

                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    ErrorMessage = result.Message ?? "Failed to book activity";
                    await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                await Shell.Current.DisplayAlert("Exception", ErrorMessage, "OK");
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