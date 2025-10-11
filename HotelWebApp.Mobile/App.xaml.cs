using HotelWebApp.Mobile.Services;

namespace HotelWebApp.Mobile
{
    public partial class App : Application
    {
        private readonly IServiceProvider _services;

        public App(IServiceProvider services)
        {
            InitializeComponent();
            _services = services;
            MainPage = _services.GetRequiredService<AppShell>();
        }
    }
}