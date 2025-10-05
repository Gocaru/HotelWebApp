using HotelWebApp.Mobile.ViewModels;

namespace HotelWebApp.Mobile.Views;

public partial class ReservationDetailPage : ContentPage
{
    public ReservationDetailPage(ReservationDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}