using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelWebApp.Mobile.Services;
using HotelWebApp.Mobile.Views;
using System.Net.Mail;

namespace HotelWebApp.Mobile.ViewModels
{
    public partial class ForgotPasswordViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool hasError;

        public bool IsNotBusy => !IsBusy;

        public ForgotPasswordViewModel(AuthService authService)
        {
            _authService = authService;
        }

        partial void OnIsBusyChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotBusy));
        }

        [RelayCommand]
        private async Task SendResetLinkAsync()
        {
            if (IsBusy) return;

            HasError = false;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Email))
            {
                HasError = true;
                ErrorMessage = "Please enter your email address";
                return;
            }

            if (!IsValidEmail(Email))
            {
                HasError = true;
                ErrorMessage = "Please enter a valid email address";
                return;
            }

            try
            {
                IsBusy = true;

                var response = await _authService.ForgotPasswordAsync(Email.Trim());

                if (response.Success)
                {
                    await Shell.Current.DisplayAlert(
                        "Email Sent",
                        "If an account exists with this email, you will receive password reset instructions.",
                        "OK");

                    // Navegar para a página de reset password
                    await Shell.Current.GoToAsync($"{nameof(ResetPasswordPage)}?email={Uri.EscapeDataString(Email.Trim())}");
                }
                else
                {
                    HasError = true;
                    ErrorMessage = response.Message;
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
            await Shell.Current.GoToAsync("..");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}