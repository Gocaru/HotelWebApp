namespace HotelDB_API_Framework.Models
{
    /// <summary>
    /// Representa um conjunto de estatísticas agregadas para o dashboard da aplicação.
    /// </summary>
    public class DashboardStats
    {
        /// <summary>
        /// Total de hóspedes registados.
        /// </summary>
        public int TotalGuests { get; set; }

        /// <summary>
        /// Total de quartos disponíveis no sistema.
        /// </summary>
        public int TotalRooms { get; set; }

        /// <summary>
        /// Total de reservas efetuadas.
        /// </summary>
        public int TotalBookings { get; set; }

        /// <summary>
        /// Total de faturas emitidas.
        /// </summary>
        public int TotalInvoices { get; set; }

        /// <summary>
        /// Receita total calculada a partir das faturas.
        /// </summary>
        public decimal TotalRevenue { get; set; }
    }
}