using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelWebApp.Mobile.Services;
using HotelWebApp.Mobile.Views;
using HotelWebApp.Mobile.Helpers;

namespace HotelWebApp.Mobile.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private readonly ReservationService _reservationService;

        [ObservableProperty]
        private string welcomeMessage = "Welcome!";

        [ObservableProperty]
        private string userFullName = "Guest";

        [ObservableProperty]
        private string profilePictureUrl = "https://via.placeholder.com/80/6366F1/FFFFFF?text=User";

        [ObservableProperty]
        private int upcomingReservations = 0;

        [ObservableProperty]
        private int completedStays = 0;

        [ObservableProperty]
        private bool isLoading = true;

        public HomeViewModel(
            AuthService authService,
            UserService userService,
            ReservationService reservationService)
        {
            _authService = authService;
            _userService = userService;
            _reservationService = reservationService;
        }

        public async Task InitializeAsync()
        {
            await LoadUserDataAsync();
        }

        private async Task LoadUserDataAsync()
        {
            IsLoading = true;

            try
            {
                var profileResponse = await _userService.GetProfileAsync();
                if (profileResponse.Success && profileResponse.Data != null)
                {
                    var profile = profileResponse.Data;
                    UserFullName = profile.FullName;
                    WelcomeMessage = $"Welcome back, {profile.FullName.Split(' ')[0]}!";

                    if (!string.IsNullOrWhiteSpace(profile.ProfilePictureUrl))
                    {
                        if (!profile.ProfilePictureUrl.StartsWith("http"))
                        {
                            var relativePath = profile.ProfilePictureUrl.TrimStart('/');
                            ProfilePictureUrl = $"{Constants.ApiBaseUrl}/{relativePath}";
                        }
                        else
                        {
                            ProfilePictureUrl = profile.ProfilePictureUrl;
                        }
                    }
                }

                var reservationsResponse = await _reservationService.GetMyReservationsAsync();
                if (reservationsResponse.Success && reservationsResponse.Data != null)
                {
                    var reservations = reservationsResponse.Data;

                    UpcomingReservations = reservations.Count(r =>
                        (r.Status.Equals("Confirmed", StringComparison.OrdinalIgnoreCase) ||
                         r.Status.Equals("CheckedIn", StringComparison.OrdinalIgnoreCase)) &&
                        r.CheckInDate >= DateTime.Today);

                    CompletedStays = reservations.Count(r =>
                        r.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading user data: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task NavigateToProfile()
        {
            await Shell.Current.GoToAsync(nameof(ProfilePage));
        }

        [RelayCommand]
        private async Task NavigateToReservations()
        {
            await Shell.Current.GoToAsync(nameof(ReservationsPage));
        }

        [RelayCommand]
        private async Task NavigateToInvoices()
        {
            await Shell.Current.GoToAsync(nameof(InvoicesPage));
        }

        [RelayCommand]
        private async Task NavigateToActivities()
        {
            await Shell.Current.GoToAsync(nameof(ActivitiesPage));
        }

        [RelayCommand]
        private async Task NavigateToMyActivityBookings()
        {
            await Shell.Current.GoToAsync(nameof(MyActivityBookingsPage));
        }

        [RelayCommand]
        private async Task NavigateToPromotions()
        {
            await Shell.Current.GoToAsync(nameof(PromotionsPage));
        }

        [RelayCommand]
        private async Task Logout()
        {
            bool confirm = await Shell.Current.DisplayAlert(
                "Logout",
                "Are you sure you want to logout?",
                "Yes",
                "No");

            if (confirm)
            {
                await _authService.LogoutAsync();

                // Navega para o root e depois para LoginPage
                Application.Current.MainPage = new NavigationPage(new LoginPage(
                    Application.Current.Handler.MauiContext.Services.GetService<LoginViewModel>()));
            }
        }

        [RelayCommand]
        private async Task NavigateToAbout()
        {
            await Shell.Current.GoToAsync(nameof(AboutPage));
        }
    }
}