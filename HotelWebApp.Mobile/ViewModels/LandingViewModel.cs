using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelWebApp.Mobile.Views;

namespace HotelWebApp.Mobile.ViewModels
{
    public partial class LandingViewModel : ObservableObject
    {
        [RelayCommand]
        private async Task NavigateToLoginAsync()
        {
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }

        [RelayCommand]
        private async Task NavigateToRegisterAsync()
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }

        [RelayCommand]
        private async Task BrowseActivitiesAsync()
        {
            await Shell.Current.GoToAsync(nameof(ActivitiesPage));
        }

        [RelayCommand]
        private async Task ViewPromotionsAsync()
        {
            await Shell.Current.GoToAsync(nameof(PromotionsPage));
        }

        [RelayCommand]
        private async Task ViewAboutAsync()
        {
            await Shell.Current.GoToAsync(nameof(AboutPage));
        }
    }
}