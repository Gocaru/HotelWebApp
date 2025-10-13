using HotelWebApp.Mobile.ViewModels;

namespace HotelWebApp.Mobile.Views;

public partial class MyActivityBookingsPage : ContentPage
{
    private readonly MyActivityBookingsViewModel _viewModel;

    public MyActivityBookingsPage(MyActivityBookingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Carregar dados quando a página aparecer
        await _viewModel.InitializeAsync();
    }
}