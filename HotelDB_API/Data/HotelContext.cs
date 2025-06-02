using HotelManagement.Core;
using Microsoft.EntityFrameworkCore;

namespace HotelDB_API.Data
{
    public class HotelContext : DbContext
    {
        /// <summary>
        /// Construtor do contexto da base de dados.
        /// É chamado automaticamente quando a aplicação precisa de aceder à base de dados.
        /// Recebe opções de configuração, como a string de ligação ao SQL Server.
        /// </summary>
        /// <param name="options">Opções de configuração da ligação à base de dados</param>
        public HotelContext(DbContextOptions<HotelContext> options) : base(options)
        {
        }

        public DbSet<Guest> Guests { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<ExtraService> ExtraServices { get; set; }
    }
}
