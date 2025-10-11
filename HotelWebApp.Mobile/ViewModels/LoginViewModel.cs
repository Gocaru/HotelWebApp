using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelWebApp.Mobile.Services;
using HotelWebApp.Mobile.Views;

namespace HotelWebApp.Mobile.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
        }

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
        public bool IsNotBusy => !IsBusy;

        [RelayCommand]
        private async Task LoginAsync()
        {
            System.Diagnostics.Debug.WriteLine("=== LOGIN STARTED ===");

            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Please enter your email";
                System.Diagnostics.Debug.WriteLine($"Validation failed: Email empty");
                OnPropertyChanged(nameof(HasError));
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter your password";
                System.Diagnostics.Debug.WriteLine($"Validation failed: Password empty");
                OnPropertyChanged(nameof(HasError));
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;
            OnPropertyChanged(nameof(HasError));
            System.Diagnostics.Debug.WriteLine($"Calling API with Email: {Email}");

            try
            {
                var result = await _authService.LoginAsync(Email, Password);
                System.Diagnostics.Debug.WriteLine($"API Response - Success: {result.Success}");
                System.Diagnostics.Debug.WriteLine($"API Response - Message: {result.Message}");

                if (result.Success && result.Data != null)
                {
                    System.Diagnostics.Debug.WriteLine("Login SUCCESS - Reconfiguring Shell");

                    // Garantir que Shell existe
                    if (Shell.Current == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Creating new AppShell instance");
                        Application.Current.MainPage = new AppShell(AppShell.Services);
                    }

                    // Reconfigurar Shell para utilizador autenticado
                    if (Shell.Current is AppShell appShell)
                    {
                        System.Diagnostics.Debug.WriteLine("Reconfiguring AppShell for authenticated user");
                        appShell.ConfigureShellForAuthenticatedUser(AppShell.Services);
                    }

                    // Navegar para Home
                    await Shell.Current.GoToAsync("///Home");

                    System.Diagnostics.Debug.WriteLine("Navigation completed");

                    Email = string.Empty;
                    Password = string.Empty;
                }
                else
                {
                    // Verifica se é erro de email não confirmado
                    if (result.Message?.Contains("Email not confirmed") == true)
                    {
                        var confirm = await Shell.Current.DisplayAlert(
                            "Email Not Confirmed",
                            "Your email address has not been confirmed yet. Would you like to confirm it now?",
                            "Yes",
                            "Cancel");

                        if (confirm)
                        {
                            await Shell.Current.GoToAsync($"{nameof(ConfirmEmailPage)}?email={Uri.EscapeDataString(Email)}");
                        }
                    }
                    else
                    {
                        ErrorMessage = result.Message ?? "Login failed. Please try again.";
                        System.Diagnostics.Debug.WriteLine($"Login FAILED - ErrorMessage: {ErrorMessage}");
                        OnPropertyChanged(nameof(HasError));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Connection error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"EXCEPTION: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");
                OnPropertyChanged(nameof(HasError));
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ForgotPasswordAsync()
        {
            await Shell.Current.GoToAsync(nameof(ForgotPasswordPage));
        }

        [RelayCommand]
        private async Task NavigateToRegisterAsync()
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }
    }
}
