using HotelDB_API_Framework.Enums;
using System;
using System.Linq;

namespace HotelDB_API_Framework.Services
{
    /// <summary>
    /// Serviço responsável por aplicar regras de negócio relacionadas com reservas.
    /// </summary>
    public class BookingService
    {
        private HotelDBDataClassesDataContext _dc;

        /// <summary>
        /// Contrutor do serviço de reservas
        /// </summary>
        /// <param name="dc">Instância do DataContext usada para acesso à base de dados.</param>
        public BookingService(HotelDBDataClassesDataContext dc)
        {
            _dc = dc;
        }

        /// <summary>
        /// Verifica se o hóspede com o ID fornecido existe.
        /// </summary>
        /// <param name="guestId">ID do hóspede a validar.</param>
        /// <returns>Verdadeiro se existir, falso caso contrário.</returns>
        public bool IsValidGuest(int guestId)
        {
            return _dc.Guests.Any(g => g.GuestId == guestId);
        }

        /// <summary>
        /// Verifica se o quarto com o ID fornecido existe.
        /// </summary>
        /// <param name="roomId">ID do quarto a validar.</param>
        /// <returns>Verdadeiro se existir, falso caso contrário.</returns>
        public bool IsValidRoom(int roomId)
        {
            return _dc.Rooms.Any(r => r.RoomId == roomId);
        }

        /// <summary>
        /// Verifica se o intervalo de datas da reserva é válido.
        /// </summary>
        /// <param name="checkIn">Data de check-in.</param>
        /// <param name="checkOut">Data de check-out.</param>
        /// <returns>Verdadeiro se check-in for anterior a check-out, falso caso contrário.</returns>
        public bool IsDateRangeValid(DateTime checkIn, DateTime checkOut)
        {
            return checkIn < checkOut;
        }

        /// <summary>
        /// Verifica se já existe uma reserva que se sobrepõe no mesmo quarto e período.
        /// Exclui a própria reserva em caso de atualização.
        /// </summary>
        /// <param name="booking">Reserva a validar</param>
        /// <returns>Verdadeiro se houver conflito, falso caso contrário.</returns>
        public bool HasOverlappingBooking(Booking booking)
        {
            return _dc.Bookings.Any(b =>
                b.RoomId == booking.RoomId &&
                b.BookingId != booking.BookingId &&
                b.CheckInDate < booking.CheckOutDate &&
                booking.CheckInDate < b.CheckOutDate);
        }

        /// <summary>
        /// Verifica se a reserva pode ser cancelada
        /// </summary>
        /// <param name="booking">Reserva a cancelar</param>
        /// <returns>Verdadeiro se puder ser cancelado, falso caso contrário.</returns>
        public bool CanBeCancelled(Booking booking)
        {
            // A reserva só pode ser cancelada se ainda estiver no estado Reserved
            if ((BookingStatus)booking.Status != BookingStatus.Reserved)
                return false;

            // E só pode ser cancelada se a data atual for anterior à data de check-in
            return booking.CheckInDate.HasValue && DateTime.Now.Date < booking.CheckInDate.Value.Date;

        }

        /// <summary>
        /// Permite o concelamento automático, caso o hospede não faça o check in na data e hora definida
        /// </summary>
        /// <param name="horaLimite">Hora até à qual o hospede pode fazer o check in</param>
        public void CancelNoShows(TimeSpan horaLimite)
        {
            var hoje = DateTime.Now.Date;
            var agora = DateTime.Now.TimeOfDay;

            var reservasExpiradas = _dc.Bookings.Where(b =>
                b.CheckInDate.HasValue &&
                b.CheckInDate.Value.Date == hoje &&
                (BookingStatus)b.Status == BookingStatus.Reserved &&
                agora > horaLimite
            ).ToList();

            foreach (var reserva in reservasExpiradas)
            {
                reserva.Status = (int)BookingStatus.Cancelled;
            }

            _dc.SubmitChanges();
        }

    }
}