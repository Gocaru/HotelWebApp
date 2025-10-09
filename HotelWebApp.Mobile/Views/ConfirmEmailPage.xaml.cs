using HotelWebApp.Mobile.ViewModels;

namespace HotelWebApp.Mobile.Views;

public partial class ConfirmEmailPage : ContentPage
{
    public ConfirmEmailPage(ConfirmEmailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}