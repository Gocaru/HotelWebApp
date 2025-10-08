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
        private string welcomeMessage = "Welcome to Hotel App!";

        public HomeViewModel(AuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task NavigateToReservationsAsync()
        {
            await Shell.Current.GoToAsync(nameof(ReservationsPage));
        }

        [RelayCommand]
        private async Task NavigateToActivitiesAsync()
        {
            await Shell.Current.GoToAsync(nameof(ActivitiesPage));
        }

        [RelayCommand]
        private async Task NavigateToMyActivityBookingsAsync()
        {
            await Shell.Current.GoToAsync(nameof(MyActivityBookingsPage));
        }

        [RelayCommand]
        private async Task NavigateToPromotionsAsync()
        {
            await Shell.Current.GoToAsync(nameof(PromotionsPage));
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            await _authService.LogoutAsync();

            // Reconfigurar Shell para não autenticado
            if (Shell.Current is AppShell appShell)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    appShell.ConfigureShellForUnauthenticatedUser(AppShell.Services);
                    Shell.Current.GoToAsync($"///{nameof(LoginPage)}");
                });
            }
        }
    }
}
