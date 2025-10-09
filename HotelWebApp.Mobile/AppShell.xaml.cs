using HotelWebApp.Mobile.Views;

namespace HotelWebApp.Mobile
{
    public partial class AppShell : Shell
    {
        public static IServiceProvider Services { get; private set; }

        public AppShell(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            Services = serviceProvider;

            // Registar rotas
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(ReservationsPage), typeof(ReservationsPage));
            Routing.RegisterRoute(nameof(ReservationDetailPage), typeof(ReservationDetailPage));
            Routing.RegisterRoute(nameof(ActivitiesPage), typeof(ActivitiesPage));
            Routing.RegisterRoute(nameof(ActivityDetailPage), typeof(ActivityDetailPage));
            Routing.RegisterRoute(nameof(MyActivityBookingsPage), typeof(MyActivityBookingsPage));
            Routing.RegisterRoute(nameof(PromotionsPage), typeof(PromotionsPage));
            Routing.RegisterRoute(nameof(PromotionDetailPage), typeof(PromotionDetailPage));
            Routing.RegisterRoute(nameof(ChangePasswordPage), typeof(ChangePasswordPage));
            Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));

            // Inicializar estado do Shell baseado em autenticação
            InitializeShellState(serviceProvider);
        }

        private void InitializeShellState(IServiceProvider serviceProvider)
        {
            var token = SecureStorage.GetAsync("auth_token").GetAwaiter().GetResult();

            if (string.IsNullOrEmpty(token))
                ConfigureShellForUnauthenticatedUser(serviceProvider);
            else
                ConfigureShellForAuthenticatedUser(serviceProvider);
        }

        public void ConfigureShellForAuthenticatedUser(IServiceProvider sp)
        {
            Items.Clear();

            Items.Add(new Tab
            {
                Title = "Home",
                Items =
                {
                    new ShellContent
                    {
                        Title = "Home",
                        Route = "Home",
                        ContentTemplate = new DataTemplate(() => sp.GetRequiredService<HomePage>())
                    }
                }
            });
        }

        public void ConfigureShellForUnauthenticatedUser(IServiceProvider sp)
        {
            Items.Clear();

            var loginShell = new ShellContent
            {
                Title = "Login",
                Route = nameof(LoginPage),
                ContentTemplate = new DataTemplate(() => sp.GetRequiredService<LoginPage>())
            };

            Items.Add(loginShell);
            CurrentItem = loginShell;
        }
    }
}
