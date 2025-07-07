using HotelWebApp.Data;
using HotelWebApp.Data.Entities;
using HotelWebApp.Data.Repositories;
using HotelWebApp.Data.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HotelWebApp.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace HotelWebApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<HotelWebAppContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                // Pode adicionar outras configurações de password, etc., aqui.
                // options.Password.RequireDigit = true;
            })
            .AddEntityFrameworkStores<HotelWebAppContext>()
            .AddDefaultUI() // Adiciona a UI padrão do Identity
            .AddDefaultTokenProviders(); // Adiciona os geradores de token

            builder.Services.AddScoped<IRoomRepository, RoomRepository>();
            builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
            builder.Services.AddScoped<IReservationService, ReservationService>();
            builder.Services.AddScoped<IAmenityRepository, AmenityRepository>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();


            builder.Services.AddTransient<SeedDb>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddRazorPages();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var seeder = services.GetRequiredService<SeedDb>();
                await seeder.SeedAsync();
            }

            app.Run();
        }
    }
}
