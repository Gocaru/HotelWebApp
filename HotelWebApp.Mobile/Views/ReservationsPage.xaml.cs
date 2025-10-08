using HotelWebApp.Mobile.ViewModels;

namespace HotelWebApp.Mobile.Views;

public partial class ReservationsPage : ContentPage
{
    public ReservationsPage(ReservationsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        System.Diagnostics.Debug.WriteLine("ReservationsPage: Constructor called");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        System.Diagnostics.Debug.WriteLine("ReservationsPage: OnAppearing called");

        if (BindingContext is ReservationsViewModel viewModel)
        {
            System.Diagnostics.Debug.WriteLine("ReservationsPage: Calling InitializeAsync");
            await viewModel.InitializeAsync();
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("ReservationsPage: BindingContext is NOT ReservationsViewModel!");
        }
    }
}