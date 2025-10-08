using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelWebApp.Mobile.Services;
using HotelWebApp.Mobile.Views;

namespace HotelWebApp.Mobile.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string welcomeMessage = "Welcome!";

        public HomeViewModel(AuthService authService)
        {
            _authService = authService;
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
                await Shell.Current.GoToAsync(nameof(LoginPage));
            }
        }
    }
}