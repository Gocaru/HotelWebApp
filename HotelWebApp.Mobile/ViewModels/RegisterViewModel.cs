using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelWebApp.Mobile.Services;
using HotelWebApp.Mobile.Views;

namespace HotelWebApp.Mobile.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        public RegisterViewModel(AuthService authService)
        {
            _authService = authService;
        }

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string confirmPassword = string.Empty;

        [ObservableProperty]
        private string fullName = string.Empty;

        [ObservableProperty]
        private string phoneNumber = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
        public bool IsNotBusy => !IsBusy;

        [RelayCommand]
        private async Task RegisterAsync()
        {
            System.Diagnostics.Debug.WriteLine("=== REGISTER STARTED ===");

            // Validações
            if (string.IsNullOrWhiteSpace(FullName))
            {
                ErrorMessage = "Please enter your full name";
                System.Diagnostics.Debug.WriteLine($"Validation failed: FullName empty");
                OnPropertyChanged(nameof(HasError));
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Please enter your email";
                System.Diagnostics.Debug.WriteLine($"Validation failed: Email empty");
                OnPropertyChanged(nameof(HasError));
                return;
            }

            if (!IsValidEmail(Email))
            {
                ErrorMessage = "Please enter a valid email address";
                System.Diagnostics.Debug.WriteLine($"Validation failed: Invalid email format");
                OnPropertyChanged(nameof(HasError));
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter a password";
                System.Diagnostics.Debug.WriteLine($"Validation failed: Password empty");
                OnPropertyChanged(nameof(HasError));
                return;
            }

            if (Password.Length < 6)
            {
                ErrorMessage = "Password must be at least 6 characters";
                System.Diagnostics.Debug.WriteLine($"Validation failed: Password too short");
                OnPropertyChanged(nameof(HasError));
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match";
                System.Diagnostics.Debug.WriteLine($"Validation failed: Passwords don't match");
                OnPropertyChanged(nameof(HasError));
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;
            OnPropertyChanged(nameof(HasError));
            System.Diagnostics.Debug.WriteLine($"Calling API with Email: {Email}");

            try
            {
                var result = await _authService.RegisterAsync(
                    Email.Trim(),
                    Password,
                    FullName.Trim(),
                    string.IsNullOrWhiteSpace(PhoneNumber) ? null : PhoneNumber.Trim());

                System.Diagnostics.Debug.WriteLine($"API Response - Success: {result.Success}");
                System.Diagnostics.Debug.WriteLine($"API Response - Message: {result.Message}");

                if (result.Success)
                {
                    System.Diagnostics.Debug.WriteLine("Registration SUCCESS - Navigating to email confirmation");

                    await Shell.Current.DisplayAlert(
                        "Success",
                        "Account created! Please check your email for the confirmation code.",
                        "OK");

                    // Navega para a página de confirmação
                    await Shell.Current.GoToAsync($"{nameof(ConfirmEmailPage)}?email={Uri.EscapeDataString(Email.Trim())}");

                    // Limpar campos
                    Password = string.Empty;
                    ConfirmPassword = string.Empty;
                    FullName = string.Empty;
                    PhoneNumber = string.Empty;
                }
                else
                {
                    ErrorMessage = result.Message ?? "Registration failed. Please try again.";

                    if (result.Errors?.Any() == true)
                    {
                        ErrorMessage = string.Join("\n", result.Errors);
                    }

                    System.Diagnostics.Debug.WriteLine($"Registration FAILED - ErrorMessage: {ErrorMessage}");
                    OnPropertyChanged(nameof(HasError));
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
        private async Task BackToLoginAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var trimmedEmail = email.Trim();

            return trimmedEmail.Contains("@") &&
                   trimmedEmail.Contains(".") &&
                   trimmedEmail.IndexOf("@") > 0 &&
                   trimmedEmail.IndexOf("@") < trimmedEmail.LastIndexOf(".");
        }
    }
}