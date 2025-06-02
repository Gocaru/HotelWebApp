using HotelManagement.Core;

namespace HotelManagementSystem.Logic
{
    /// <summary>
    /// Classe auxiliar responsável pela criação de faturas baseadas em reservas, quartos e serviços extra.
    /// </summary>
    public static class InvoiceGenerator
    {
        /// <summary>
        /// Gera uma nova fatura com base nos dados fornecidos.
        /// </summary>
        public static Invoice Generate(Booking booking, Room room, List<ExtraService> extras, string guestName, PaymentMethod metodoPagamento, int nextInvoiceId)
        {
            // Calcula o número total de noites da estadia
            int noites = (booking.CheckOutDate - booking.CheckInDate).Days;
            
            // Calcula o valor total da estadia (noites x preço por noite)
            decimal estadia = noites * room.PricePerNight;
            
            // Soma o total dos serviços extra associados à reserva
            decimal extrasTotal = extras.Sum(e => e.Price);

            // Cria e devolve a nova fatura com os dados preenchidos
            return new Invoice
            {
                InvoiceId = nextInvoiceId,
                BookingId = booking.BookingId,
                GuestName = guestName,
                StayTotal = estadia,
                ExtrasTotal = extrasTotal,
                IssueDate = DateTime.Now,
                PaymentMethod = metodoPagamento
            };
        }
    }
}
