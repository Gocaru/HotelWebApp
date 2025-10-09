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

            // Configurar UserService
            builder.Services.AddHttpClient<UserService>(client =>
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

            // Configurar ReservationService
            builder.Services.AddHttpClient<ReservationService>(client =>
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

            // Configurar ActivityService
            builder.Services.AddHttpClient<ActivityService>(client =>
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

            // Configurar PromotionService
            builder.Services.AddHttpClient<PromotionService>(client =>
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
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<ReservationsViewModel>();
            builder.Services.AddTransient<ReservationDetailViewModel>();
            builder.Services.AddTransient<ActivitiesViewModel>();
            builder.Services.AddTransient<ActivityDetailViewModel>();
            builder.Services.AddTransient<MyActivityBookingsViewModel>();
            builder.Services.AddTransient<PromotionsViewModel>();
            builder.Services.AddTransient<PromotionDetailViewModel>();
            builder.Services.AddTransient<ChangePasswordViewModel>();
            builder.Services.AddTransient<AboutViewModel>();

            // Registar Views
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<ReservationsPage>();
            builder.Services.AddTransient<ReservationDetailPage>();
            builder.Services.AddTransient<ActivitiesPage>();
            builder.Services.AddTransient<ActivityDetailPage>();
            builder.Services.AddTransient<MyActivityBookingsPage>();
            builder.Services.AddTransient<PromotionsPage>();
            builder.Services.AddTransient<PromotionDetailPage>();
            builder.Services.AddTransient<ChangePasswordPage>();
            builder.Services.AddTransient<AboutPage>();

            // AppShell
            builder.Services.AddSingleton<AppShell>(sp => new AppShell(sp));


            #if DEBUG
            builder.Logging.AddDebug();
            #endif

            return builder.Build();
        }
    }
}
