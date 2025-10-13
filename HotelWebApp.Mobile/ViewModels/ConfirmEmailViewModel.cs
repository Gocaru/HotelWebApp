using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelWebApp.Mobile.Services;
using HotelWebApp.Mobile.Views;

namespace HotelWebApp.Mobile.ViewModels
{
    [QueryProperty(nameof(Email), "email")]
    public partial class ConfirmEmailViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string confirmationCode = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool hasError;

        public bool IsNotBusy => !IsBusy;

        public ConfirmEmailViewModel(AuthService authService)
        {
            _authService = authService;
        }

        partial void OnIsBusyChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotBusy));
        }

        [RelayCommand]
        private async Task ConfirmEmailAsync()
        {
            if (IsBusy) return;

            HasError = false;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(ConfirmationCode))
            {
                HasError = true;
                ErrorMessage = "Please enter the confirmation code";
                return;
            }

            try
            {
                IsBusy = true;

                var response = await _authService.ConfirmEmailAsync(Email.Trim(), ConfirmationCode.Trim());

                if (response.Success)
                {
                    await Shell.Current.DisplayAlert(
                        "Success",
                        "Your email has been confirmed! You can now login.",
                        "OK");

                    // Navegar para login
                    Application.Current.MainPage = new NavigationPage(
                        new LoginPage(Application.Current.Handler.MauiContext.Services.GetService<LoginViewModel>()));
                }
                else
                {
                    HasError = true;
                    ErrorMessage = response.Message ?? "Invalid confirmation code";

                    if (response.Errors?.Any() == true)
                    {
                        ErrorMessage = string.Join("\n", response.Errors);
                    }
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ResendCodeAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                HasError = false;
                ErrorMessage = string.Empty;

                var response = await _authService.ResendConfirmationEmailAsync(Email.Trim());

                if (response.Success)
                {
                    await Shell.Current.DisplayAlert(
                        "Email Sent",
                        "A new confirmation code has been sent to your email.",
                        "OK");
                }
                else
                {
                    HasError = true;
                    ErrorMessage = response.Message ?? "Failed to resend code";
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task BackToLoginAsync()
        {
            Application.Current.MainPage = new NavigationPage(
                new LoginPage(Application.Current.Handler.MauiContext.Services.GetService<LoginViewModel>()));
        }
    }
}