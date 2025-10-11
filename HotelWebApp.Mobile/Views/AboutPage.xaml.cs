using HotelWebApp.Mobile.ViewModels;
namespace HotelWebApp.Mobile.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage(AboutViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}