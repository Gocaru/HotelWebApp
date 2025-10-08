using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelWebApp.Mobile.Models;
using HotelWebApp.Mobile.Services;

namespace HotelWebApp.Mobile.ViewModels
{
    public partial class ChangePasswordViewModel : ObservableObject
    {
        private readonly UserService _userService;

        [ObservableProperty]
        private string currentPassword = string.Empty;

        [ObservableProperty]
        private string newPassword = string.Empty;

        [ObservableProperty]
        private string confirmPassword = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool showCurrentPassword;

        [ObservableProperty]
        private bool showNewPassword;

        [ObservableProperty]
        private bool showConfirmPassword;

        public ChangePasswordViewModel(UserService userService)
        {
            _userService = userService;
        }

        [RelayCommand]
        private async Task ChangePasswordAsync()
        {
            if (IsBusy) return;

            // Validações
            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                await Shell.Current.DisplayAlert("Validation Error", "Current password is required", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                await Shell.Current.DisplayAlert("Validation Error", "New password is required", "OK");
                return;
            }

            if (NewPassword.Length < 6)
            {
                await Shell.Current.DisplayAlert("Validation Error", "Password must be at least 6 characters", "OK");
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                await Shell.Current.DisplayAlert("Validation Error", "Passwords do not match", "OK");
                return;
            }

            if (CurrentPassword == NewPassword)
            {
                await Shell.Current.DisplayAlert("Validation Error", "New password must be different from current password", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                var request = new ChangePasswordRequest
                {
                    CurrentPassword = CurrentPassword,
                    NewPassword = NewPassword,
                    ConfirmPassword = ConfirmPassword
                };

                var response = await _userService.ChangePasswordAsync(request);

                if (response.Success)
                {
                    // Limpar campos
                    CurrentPassword = string.Empty;
                    NewPassword = string.Empty;
                    ConfirmPassword = string.Empty;

                    await Shell.Current.DisplayAlert("Success", "Password changed successfully", "OK");

                    // Voltar para a página anterior
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", response.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to change password: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void ToggleCurrentPasswordVisibility()
        {
            ShowCurrentPassword = !ShowCurrentPassword;
        }

        [RelayCommand]
        private void ToggleNewPasswordVisibility()
        {
            ShowNewPassword = !ShowNewPassword;
        }

        [RelayCommand]
        private void ToggleConfirmPasswordVisibility()
        {
            ShowConfirmPassword = !ShowConfirmPassword;
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}