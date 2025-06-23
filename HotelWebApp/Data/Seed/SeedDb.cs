using HotelWebApp.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace HotelWebApp.Data.Seed
{
    public class SeedDb
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedDb(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            await CheckRoleAsync("Admin");
            await CheckRoleAsync("Employee");
            await CheckRoleAsync("Guest");

            var adminUser = await _userManager.FindByEmailAsync("admin@hotel.com");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    FullName = "Administrator",
                    UserName = "admin@hotel.com",
                    Email = "admin@hotel.com",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

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
