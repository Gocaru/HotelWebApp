using HotelDB_WPF_Framework.Models;
using System;

namespace HotelDB_WPF_Framework.Services
{
    public static class InvoiceService
    {
        /// <summary>
        /// Calcula o total da estadia e o total geral da fatura com base na reserva e no preço do quarto.
        /// </summary>
        public static void CalcularTotais(Invoice invoice, Booking booking, Room room)
        {
            if (invoice == null)
                throw new ArgumentNullException(nameof(invoice), "Invoice must not be null.");
            if (booking == null)
                throw new ArgumentNullException(nameof(booking), "Booking must not be null.");
            if (room == null)
                throw new ArgumentNullException(nameof(room), "Room must not be null.");

            int numeroNoites = (booking.CheckOutDate - booking.CheckInDate).Days;
            if (numeroNoites <= 0)
                throw new ArgumentException("Check-out date must be after check-in date.");

            decimal stayTotal = numeroNoites * room.PricePerNight;

            invoice.StayTotal = stayTotal;
            invoice.Total = stayTotal + invoice.ExtrasTotal;
        }
    }
}

