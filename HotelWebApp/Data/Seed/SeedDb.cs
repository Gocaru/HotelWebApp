using HotelWebApp.Data;
using HotelWebApp.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HotelWebApp.Data.Seed
{
    /// <summary>
    /// Responsible for seeding the database with initial data required for the application to function.
    /// This includes creating user roles and a default administrator account.
    /// This class is designed to be run at application startup.
    /// </summary>
    public class SeedDb
    {
        private readonly HotelWebAppContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedDb(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, HotelWebAppContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        /// <summary>
        /// Executes the database seeding process.
        /// It ensures that the necessary roles and the default admin user exist.
        /// </summary>
        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync();

            // Ensure all required roles are present in the database
            await CheckRoleAsync("Admin");
            await CheckRoleAsync("Employee");
            await CheckRoleAsync("Guest");

            // Then, create the default administrator user if it does not already exist.
            var adminUser = await _userManager.FindByEmailAsync("admin@hotel.com");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    FullName = "Administrator",
                    UserName = "admin@hotel.com", // UserName and Email are often the same.
                    Email = "admin@hotel.com",
                    EmailConfirmed = true // Admin/Employee accounts are confirmed by default.
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    // If user creation is successful, assign them to the 'Admin' role.
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed Activities and Promotions
            await SeedActivitiesAsync();
            await SeedPromotionsAsync();
        }

        /// <summary>
        /// A private helper method to check if a role exists, and create it if it doesn't.
        /// This makes the seeding process idempotent (it can be run multiple times without causing errors).
        /// </summary>
        /// <param name="roleName">The name of the role to check/create.</param>
        private async Task CheckRoleAsync(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if(!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = roleName });    
            }
        }

        private async Task SeedActivitiesAsync()
        {
            if (!_context.Activities.Any())
            {
                var activities = new List<Activity>
        {
            new Activity
            {
                Name = "Spa & Massage",
                Description = "Relaxing massage and spa treatment",
                Price = 50m,
                Duration = "1 hour",
                Schedule = "9:00 - 20:00",
                Capacity = 4,
                IsActive = true
            },
            new Activity
            {
                Name = "Guided City Tour",
                Description = "Explore the city with a professional guide",
                Price = 30m,
                Duration = "3 hours",
                Schedule = "10:00, 14:00",
                Capacity = 20,
                IsActive = true
            },
            new Activity
            {
                Name = "Airport Transfer",
                Description = "Private transfer to/from airport",
                Price = 25m,
                Duration = "Variable",
                Schedule = "24/7",
                Capacity = 4,
                IsActive = true
            },
            new Activity
            {
                Name = "Gym Access",
                Description = "Full day access to hotel gym facilities",
                Price = 15m,
                Duration = "Full day",
                Schedule = "6:00 - 22:00",
                Capacity = 30,
                IsActive = true
            },
            new Activity
            {
                Name = "Wine Tasting",
                Description = "Premium wine tasting experience",
                Price = 40m,
                Duration = "2 hours",
                Schedule = "18:00 - 20:00",
                Capacity = 15,
                IsActive = true
            }
        };

                await _context.Activities.AddRangeAsync(activities);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedPromotionsAsync()
        {
            if (!_context.Promotions.Any())
            {
                var promotions = new List<Promotion>
        {
            new Promotion
            {
                Title = "Autumn Special - 20% Off",
                Description = "Book 3 nights or more and get 20% discount on your stay",
                StartDate = new DateTime(2025, 9, 30),
                EndDate = new DateTime(2025, 11, 30),
                DiscountPercentage = 20,
                IsActive = true,
                Type = PromotionType.LongStay,         
                MinimumNights = 3,                   
                Terms = "Valid for bookings made between September and November. Cannot be combined with other offers."
            },
            new Promotion
            {
                Title = "Weekend Getaway",
                Description = "Special weekend rates for Friday and Saturday nights",
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 12, 31),
                DiscountPercentage = 15,
                IsActive = true,
                Type = PromotionType.Weekend,        
                Terms = "Valid only for weekend stays (Friday-Sunday). Subject to availability."
            },
            new Promotion
            {
                Title = "Early Bird Discount",
                Description = "Book 30 days in advance and save 10%",
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 12, 31),
                DiscountPercentage = 10,
                IsActive = true,
                Type = PromotionType.EarlyBird,         
                MinimumDaysInAdvance = 30,           
                Terms = "Must be booked at least 30 days before check-in date. Non-refundable."
            }
        };

                await _context.Promotions.AddRangeAsync(promotions);
                await _context.SaveChangesAsync();
            }
        }
    }
}
