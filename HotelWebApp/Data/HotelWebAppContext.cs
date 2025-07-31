using HotelWebApp.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Data;

/// <summary>
/// The main database context for the application, powered by Entity Framework Core.
/// It inherits from IdentityDbContext to include all the necessary tables for user and role management.
/// </summary>
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

    public DbSet<Invoice> Invoices { get; set; }

    public DbSet<Payment> Payments { get; set; }

    public DbSet<ChangeRequest> ChangeRequests { get; set; }

    /// <summary>
    /// Configures the model and its relationships using the Fluent API.
    /// This method is called by Entity Framework when the model is being created.
    /// </summary>
    /// <param name="builder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Custom configuration to override default cascade delete behavior.
        // This ensures data integrity by preventing a user from being deleted
        // if they have existing reservations or invoices.
        builder.Entity<Reservation>()
            .HasOne(r => r.ApplicationUser)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.GuestId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete.

        builder.Entity<Invoice>()
            .HasOne(i => i.Guest)
            .WithMany(u => u.Invoices)
            .HasForeignKey(i => i.GuestId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete.
    }
}


