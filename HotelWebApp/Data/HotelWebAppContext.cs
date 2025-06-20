using HotelWebApp.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Data;

public class HotelWebAppContext : IdentityDbContext<ApplicationUser>
{
    public HotelWebAppContext(DbContextOptions<HotelWebAppContext> options)
        : base(options)
    {
    }

    public DbSet<Room> Rooms { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
