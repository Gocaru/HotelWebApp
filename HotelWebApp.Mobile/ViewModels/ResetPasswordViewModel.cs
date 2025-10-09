using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelWebApp.Mobile.Services;
using HotelWebApp.Mobile.Views;
using System.Linq;

namespace HotelWebApp.Mobile.ViewModels
{
    [QueryProperty(nameof(Email), "email")]
    public partial class ResetPasswordViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string token = string.Empty;

        [ObservableProperty]
        private string newPassword = string.Empty;

        [ObservableProperty]
        private string confirmPassword = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool hasError;

        public bool IsNotBusy => !IsBusy;

        public ResetPasswordViewModel(AuthService authService)
        {
            _authService = authService;
        }

        partial void OnIsBusyChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotBusy));
        }

        [RelayCommand]
        private async Task ResetPasswordAsync()
        {
            if (IsBusy) return;

            HasError = false;
            ErrorMessage = string.Empty;

            // Validações
            if (string.IsNullOrWhiteSpace(Token))
            {
                HasError = true;
                ErrorMessage = "Please enter the reset code from your email";
                return;
            }

            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                HasError = true;
                ErrorMessage = "Please enter a new password";
                return;
            }

            if (NewPassword.Length < 6)
            {
                HasError = true;
                ErrorMessage = "Password must be at least 6 characters";
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                HasError = true;
                ErrorMessage = "Passwords do not match";
                return;
            }

            try
            {
                IsBusy = true;

                var response = await _authService.ResetPasswordAsync(
                    Email.Trim(),
                    Token.Trim(),
                    NewPassword);

                if (response.Success)
                {
                    await Shell.Current.DisplayAlert(
                        "Success",
                        "Your password has been reset successfully. Please login with your new password.",
                        "OK");

                    // Voltar para o login
                    await Shell.Current.GoToAsync($"///{nameof(LoginPage)}");
                }
                else
                {
                    HasError = true;
                    ErrorMessage = response.Message;

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
        private async Task BackToLoginAsync()
        {
            await Shell.Current.GoToAsync($"///{nameof(LoginPage)}");
        }
    }
}