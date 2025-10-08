using HotelWebApp.Mobile.ViewModels;

namespace HotelWebApp.Mobile.Views
{
    public partial class ChangePasswordPage : ContentPage
    {
        public ChangePasswordPage(ChangePasswordViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}