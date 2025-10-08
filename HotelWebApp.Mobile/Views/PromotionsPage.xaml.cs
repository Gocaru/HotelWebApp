using HotelWebApp.Mobile.ViewModels;

namespace HotelWebApp.Mobile.Views;

public partial class PromotionsPage : ContentPage
{
    private readonly PromotionsViewModel _viewModel;

    public PromotionsPage(PromotionsViewModel viewModel)
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
