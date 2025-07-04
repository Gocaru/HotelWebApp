using HotelWebApp.Data.Entities;
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

    public DbSet<Reservation> Reservations { get; set; }

    public DbSet<Amenity> Amenities { get; set; }
    
    public DbSet<ReservationAmenity> ReservationAmenities { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
