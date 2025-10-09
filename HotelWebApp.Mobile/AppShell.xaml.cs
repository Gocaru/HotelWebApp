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
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(ConfirmEmailPage), typeof(ConfirmEmailPage));
            Routing.RegisterRoute(nameof(ChangePasswordPage), typeof(ChangePasswordPage));
            Routing.RegisterRoute(nameof(ForgotPasswordPage), typeof(ForgotPasswordPage));
            Routing.RegisterRoute(nameof(ResetPasswordPage), typeof(ResetPasswordPage));
            Routing.RegisterRoute(nameof(InvoiceDetailPage), typeof(InvoiceDetailPage));
            Routing.RegisterRoute(nameof(PaymentPage), typeof(PaymentPage));
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

            // Home Tab
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

            // Reservations Tab
            Items.Add(new Tab
            {
                Title = "Reservations",
                Items =
        {
            new ShellContent
            {
                Title = "My Reservations",
                Route = "Reservations",
                ContentTemplate = new DataTemplate(() => sp.GetRequiredService<ReservationsPage>())
            }
        }
            });

            // Invoices Tab
            Items.Add(new Tab
            {
                Title = "Invoices",
                Items =
        {
            new ShellContent
            {
                Title = "My Invoices",
                Route = "Invoices",
                ContentTemplate = new DataTemplate(() => sp.GetRequiredService<InvoicesPage>())
            }
        }
            });

            // Activities Tab
            Items.Add(new Tab
            {
                Title = "Activities",
                Items =
        {
            new ShellContent
            {
                Title = "Activities",
                Route = "Activities",
                ContentTemplate = new DataTemplate(() => sp.GetRequiredService<ActivitiesPage>())
            }
        }
            });

            // Profile Tab
            Items.Add(new Tab
            {
                Title = "Profile",
                Items =
        {
            new ShellContent
            {
                Title = "Profile",
                Route = "Profile",
                ContentTemplate = new DataTemplate(() => sp.GetRequiredService<ProfilePage>())
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
