using HotelWebApp.Mobile.ViewModels;

namespace HotelWebApp.Mobile.Views;

public partial class PromotionDetailPage : ContentPage
{
    private readonly PromotionDetailViewModel _viewModel;

    public PromotionDetailPage(PromotionDetailViewModel viewModel)
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
