using HotelWebApp.Mobile.ViewModels;

namespace HotelWebApp.Mobile.Views;

public partial class MyActivityBookingsPage : ContentPage
{
    private readonly MyActivityBookingsViewModel _viewModel;

    public MyActivityBookingsPage(MyActivityBookingsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}
