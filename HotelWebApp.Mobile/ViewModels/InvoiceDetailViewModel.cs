using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelWebApp.Mobile.Models;
using HotelWebApp.Mobile.Services;
using HotelWebApp.Mobile.Views;
using System.Collections.ObjectModel;

namespace HotelWebApp.Mobile.ViewModels
{
    [QueryProperty(nameof(InvoiceId), "InvoiceId")]
    public partial class InvoiceDetailViewModel : ObservableObject
    {
        private readonly InvoiceService _invoiceService;

        [ObservableProperty]
        private int invoiceId;

        [ObservableProperty]
        private InvoiceDto? invoice;

        [ObservableProperty]
        private ObservableCollection<InvoiceItemDto> items = new();

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public InvoiceDetailViewModel(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        partial void OnInvoiceIdChanged(int value)
        {
            LoadInvoiceDetailsAsync().ConfigureAwait(false);
        }

        private async Task LoadInvoiceDetailsAsync()
        {
            if (IsBusy || InvoiceId == 0) return;

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                System.Diagnostics.Debug.WriteLine($"Loading invoice {InvoiceId} details...");

                // Load invoice details
                var invoiceResult = await _invoiceService.GetInvoiceDetailsAsync(InvoiceId);

                if (invoiceResult.Success && invoiceResult.Data != null)
                {
                    Invoice = invoiceResult.Data;
                    System.Diagnostics.Debug.WriteLine("Invoice loaded");
                }
                else
                {
                    ErrorMessage = invoiceResult.Message ?? "Failed to load invoice details";
                    return;
                }

                // Load invoice items
                var itemsResult = await _invoiceService.GetInvoiceItemsAsync(InvoiceId);

                if (itemsResult.Success && itemsResult.Data != null)
                {
                    Items.Clear();
                    foreach (var item in itemsResult.Data)
                    {
                        Items.Add(item);
                    }
                    System.Diagnostics.Debug.WriteLine($"Loaded {Items.Count} items");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task PayInvoiceAsync()
        {
            if (Invoice == null || !Invoice.HasBalance) return;

            await Shell.Current.GoToAsync($"{nameof(PaymentPage)}", new Dictionary<string, object>
            {
                { "Invoice", Invoice }
            });
        }

    }
}