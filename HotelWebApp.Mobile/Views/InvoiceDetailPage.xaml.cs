using HotelWebApp.Mobile.ViewModels;

namespace HotelWebApp.Mobile.Views
{
    public partial class InvoiceDetailPage : ContentPage
    {
        public InvoiceDetailPage(InvoiceDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}