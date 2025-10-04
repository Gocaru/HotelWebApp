using HotelWebApp.Mobile.Helpers;
using HotelWebApp.Mobile.Services;
using HotelWebApp.Mobile.ViewModels;
using HotelWebApp.Mobile.Views;
using Microsoft.Extensions.Logging;

namespace HotelWebApp.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Configurar HttpClient
            builder.Services.AddHttpClient<AuthService>(client =>
            {
                client.BaseAddress = new Uri(Constants.ApiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
            #if ANDROID
                var handler = new Xamarin.Android.Net.AndroidMessageHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                return handler;
            #else
                return new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
            #endif
            });

            // Registar ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<HomeViewModel>();

            // Registar Views
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<HomePage>();

            // AppShell
            builder.Services.AddSingleton<AppShell>(sp => new AppShell(sp));


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
