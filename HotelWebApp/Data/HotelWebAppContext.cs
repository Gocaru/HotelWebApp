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

    public DbSet<Invoice> Invoices { get; set; }

    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Dizemos ao EF para não apagar em cascata as reservas quando um utilizador é apagado.
        // Em vez disso, a operação será restringida (não permitirá apagar o utilizador se ele tiver reservas).
        builder.Entity<Reservation>()
            .HasOne(r => r.ApplicationUser)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.GuestId)
            .OnDelete(DeleteBehavior.Restrict);

        // Fazemos o mesmo para a relação entre a Fatura e o Utilizador.
        builder.Entity<Invoice>()
            .HasOne(i => i.Guest)
            .WithMany(u => u.Invoices)
            .HasForeignKey(i => i.GuestId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
