using HotelWebApp.Mobile.ViewModels;

namespace HotelWebApp.Mobile.Views
{
    public partial class PaymentPage : ContentPage
    {
        public PaymentPage(PaymentViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}