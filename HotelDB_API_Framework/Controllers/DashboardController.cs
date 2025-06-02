using HotelDB_API_Framework.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HotelDB_API_Framework.Controllers
{
    /// <summary>
    /// Controlador responsável por fornecer estatísticas gerais para o dashboard da aplicação.
    /// </summary>
    public class DashboardController : ApiController
    {
        private HotelDBDataClassesDataContext db;

        /// <summary>
        /// Inicializa o controlador e estabelece ligação à base de dados.
        /// </summary>
        public DashboardController()
        {
            db = new HotelDBDataClassesDataContext(
                ConfigurationManager.ConnectionStrings["HotelDBConnectionString"].ConnectionString);
        }

        /// <summary>
        /// Obtém estatísticas agregadas para o dashboard.
        /// </summary>
        /// <returns>Objeto contendo totais de hóspedes, quartos, reservas, faturas e receita.</returns>
        public IHttpActionResult Get()
        {
            var stats = new DashboardStats
            {
                TotalGuests = db.Guests.Count(),
                TotalRooms = db.Rooms.Count(),
                TotalBookings = db.Bookings.Count(),
                TotalInvoices = db.Invoices.Count(),
                TotalRevenue = db.Invoices.Sum(i => (decimal?)i.Total) ?? 0
            };

            return Ok(stats);
        }
    }
}
