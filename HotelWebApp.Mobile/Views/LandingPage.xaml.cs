

using HotelWebApp.Mobile.ViewModels;

namespace HotelWebApp.Mobile.Views;

public partial class LandingPage : ContentPage
{
    public LandingPage(LandingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}