using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelWebApp.Mobile.Models;
using HotelWebApp.Mobile.Services;
using HotelWebApp.Mobile.Views;
using System.Collections.ObjectModel;

namespace HotelWebApp.Mobile.ViewModels
{
    public partial class InvoicesViewModel : ObservableObject
    {
        private readonly InvoiceService _invoiceService;

        [ObservableProperty]
        private ObservableCollection<InvoiceDto> invoices = new();

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private bool hasInvoices;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public InvoicesViewModel(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        public async Task InitializeAsync()
        {
            await LoadInvoicesAsync();
        }

        [RelayCommand]
        private async Task LoadInvoicesAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            IsRefreshing = true;
            ErrorMessage = string.Empty;

            try
            {
                System.Diagnostics.Debug.WriteLine("InvoicesViewModel: Loading invoices...");

                var result = await _invoiceService.GetMyInvoicesAsync();

                if (result.Success && result.Data != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Loaded {result.Data.Count} invoices");

                    Invoices.Clear();
                    foreach (var invoice in result.Data.OrderByDescending(i => i.InvoiceDate))
                    {
                        Invoices.Add(invoice);
                    }
                    HasInvoices = Invoices.Any();
                }
                else
                {
                    ErrorMessage = result.Message ?? "Failed to load invoices";
                    HasInvoices = false;
                    System.Diagnostics.Debug.WriteLine($"Failed: {ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                HasInvoices = false;
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task ViewInvoiceDetailsAsync(InvoiceDto invoice)
        {
            if (invoice == null) return;

            await Shell.Current.GoToAsync($"{nameof(InvoiceDetailPage)}", new Dictionary<string, object>
            {
                { "InvoiceId", invoice.Id }
            });
        }
    }
}