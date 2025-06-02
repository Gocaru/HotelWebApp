using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelDB_API_Framework.Services
{
    /// <summary>
    /// Serviço responsável por aplicar regras de negócio relacionadas com faturas.
    /// </summary>
    public class InvoiceService
    {
        private HotelDBDataClassesDataContext _dc;

        /// <summary>
        /// Construtor do serviço de faturas.
        /// </summary>
        /// <param name="dc">Instância do DataContext usada para acesso à base de dados.</param>
        public InvoiceService(HotelDBDataClassesDataContext dc)
        {
            _dc = dc;
        }

        /// <summary>
        /// Verifica se a reserva com o ID fornecido existe na base de dados.
        /// </summary>
        /// <param name="bookingId">ID da reserva a verificar.</param>
        /// <returns>Verdadeiro se a reserva existir, falso caso contrário.</returns>
        public bool IsValidBooking(int bookingId)
        {
            return _dc.Bookings.Any(b => b.BookingId == bookingId);
        }

        /// <summary>
        /// Verifica se já existe uma fatura emitida para a reserva fornecida.
        /// </summary>
        /// <param name="bookingId">ID da reserva a verificar.</param>
        /// <returns>Verdadeiro se já existir fatura, falso caso contrário.</returns>
        public bool IsBookingAlreadyInvoiced(int bookingId)
        {
            return _dc.Invoices.Any(i => i.BookingId == bookingId);
        }

        /// <summary>
        /// Calcula os totais de uma fatura com base na reserva associada.
        /// Calcula ExtrasTotal e StayTotal (noites × preço por noite + extras).
        /// Define também a data de emissão (IssueDate).
        /// </summary>
        /// <param name="invoice">Fatura a ser atualizada com os valores calculados.</param>
        public void CalculateTotals(Invoice invoice)
        {
            var booking = _dc.Bookings.SingleOrDefault(b => b.BookingId == invoice.BookingId);
            if (booking == null) return;

            var room = _dc.Rooms.SingleOrDefault(r => r.RoomId == booking.RoomId);
            if (room == null) return;

            // Cálculo do número de noites
            int nights = 0;

            if (booking.CheckInDate.HasValue && booking.CheckOutDate.HasValue)
            {
                nights = (int)(booking.CheckOutDate.Value - booking.CheckInDate.Value).TotalDays;
            }


            // Cálculo dos serviços extra
            decimal extrasTotal = _dc.ExtraServices
                .Where(es => es.BookingId == booking.BookingId)
                .Select(es => (decimal?)es.Price)
                .Sum() ?? 0;

            // Atualização dos valores na fatura
            invoice.ExtrasTotal = extrasTotal;
            invoice.StayTotal = (decimal)(room.PricePerNight * nights);
            invoice.Total = invoice.StayTotal + invoice.ExtrasTotal; // Para permitir no futuro aplicar promoções, descontos ou calcular impostos a pagar
            invoice.IssueDate = DateTime.Now;
        }

    }
}