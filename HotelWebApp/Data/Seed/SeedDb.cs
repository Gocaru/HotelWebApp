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
    }
}
