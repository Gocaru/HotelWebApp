using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelWebApp.Mobile.Models;
using HotelWebApp.Mobile.Services;

namespace HotelWebApp.Mobile.ViewModels
{
    [QueryProperty(nameof(Invoice), "Invoice")]
    public partial class PaymentViewModel : ObservableObject
    {
        private readonly PaymentService _paymentService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private InvoiceDto? invoice;

        [ObservableProperty]
        private decimal paymentAmount;

        [ObservableProperty]
        private string cardNumber = string.Empty;

        [ObservableProperty]
        private string cardHolderName = string.Empty;

        [ObservableProperty]
        private string expiryDate = string.Empty;

        [ObservableProperty]
        private string cvv = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public PaymentViewModel(
            PaymentService paymentService,
            INotificationService notificationService)
        {
            _paymentService = paymentService;
            _notificationService = notificationService;
        }

        partial void OnInvoiceChanged(InvoiceDto? value)
        {
            if (value != null)
            {
                PaymentAmount = value.BalanceDue;
            }
        }

        [RelayCommand]
        private async Task ProcessPaymentAsync()
        {
            if (!ValidateForm())
            {
                return;
            }

            bool confirm = await Shell.Current.DisplayAlert(
                "Confirm Payment",
                $"Process payment of €{PaymentAmount:N2}?",
                "Yes, Pay",
                "Cancel"
            );

            if (!confirm) return;

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                System.Diagnostics.Debug.WriteLine("Processing payment...");

                var request = new MobilePaymentRequest
                {
                    InvoiceId = Invoice!.Id,
                    Amount = PaymentAmount,
                    PaymentMethod = "CreditCard",
                    CardNumber = CardNumber,
                    CardHolderName = CardHolderName,
                    ExpiryDate = ExpiryDate,
                    CVV = Cvv
                };

                var result = await _paymentService.ProcessPaymentAsync(request);

                if (result.Success)
                {
                    System.Diagnostics.Debug.WriteLine("Payment successful");

                    // Mostrar notificação de pagamento bem-sucedido
                    await _notificationService.ShowPaymentSuccessAsync(
                        PaymentAmount,
                        result.Data?.TransactionId ?? "N/A"
                    );

                    await Shell.Current.DisplayAlert(
                        "Payment Successful",
                        $"Your payment of €{PaymentAmount:N2} has been processed successfully.\n\nTransaction ID: {result.Data?.TransactionId}",
                        "OK"
                    );

                    // Navigate back to invoices list
                    await Shell.Current.GoToAsync("../..");
                }
                else
                {
                    ErrorMessage = result.Message ?? "Payment failed";
                    await Shell.Current.DisplayAlert("Payment Failed", ErrorMessage, "OK");
                    System.Diagnostics.Debug.WriteLine($"Payment failed: {ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void PayFullAmount()
        {
            if (Invoice != null)
            {
                PaymentAmount = Invoice.BalanceDue;
            }
        }

        private bool ValidateForm()
        {
            ErrorMessage = string.Empty;

            if (Invoice == null)
            {
                ErrorMessage = "Invalid invoice";
                return false;
            }

            if (PaymentAmount <= 0)
            {
                ErrorMessage = "Payment amount must be greater than 0";
                return false;
            }

            if (PaymentAmount > Invoice.BalanceDue)
            {
                ErrorMessage = $"Payment amount cannot exceed balance due (€{Invoice.BalanceDue:N2})";
                return false;
            }

            if (string.IsNullOrWhiteSpace(CardNumber) || CardNumber.Length < 13)
            {
                ErrorMessage = "Please enter a valid card number";
                return false;
            }

            if (string.IsNullOrWhiteSpace(CardHolderName))
            {
                ErrorMessage = "Please enter card holder name";
                return false;
            }

            if (string.IsNullOrWhiteSpace(ExpiryDate) || ExpiryDate.Length < 4)
            {
                ErrorMessage = "Please enter expiry date in MM/YY format";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Cvv) || Cvv.Length < 3 || Cvv.Length > 4)
            {
                ErrorMessage = "Please enter a valid CVV";
                return false;
            }

            return true;
        }
    }
}