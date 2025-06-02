using HotelDB_WPF_Framework.Enums;
using HotelDB_WPF_Framework.Models;
using System;

namespace HotelDB_WPF_Framework.Helpers.InputBuilders
{
    /// <summary>
    /// Responsável por construir e validar um objeto Booking a partir dos inputs da interface.
    /// </summary>
    public static class BookingInputBuilder
    {
        /// <summary>
        /// Tenta construir um objeto Booking válido a partir dos dados fornecidos pela interface.
        /// </summary>
        /// <param name="selectedGuest">Objeto Guest selecionado.</param>
        /// <param name="txtNumGuests">Texto com o número de hóspedes.</param>
        /// <param name="selectedRoomType">Tipo de quarto selecionado.</param>
        /// <param name="checkInDate">Data de entrada selecionada.</param>
        /// <param name="checkOutDate">Data de saída selecionada.</param>
        /// <param name="booking">Objeto Booking resultante.</param>
        /// <param name="error">Mensagem de erro, caso a construção falhe.</param>
        public static bool TryBuildBooking(Guest selectedGuest,
                                           string txtNumGuests,
                                           RoomType selectedRoomType,
                                           DateTime? checkInDate,
                                           DateTime? checkOutDate,
                                           out Booking booking,
                                           out string error)
        {
            booking = null;
            error = "";

            if (selectedGuest == null)
            {
                error = "Please select a guest.";
                return false;
            }

            if (!int.TryParse(txtNumGuests.Trim(), out int numGuests) || numGuests <= 0)
            {
                error = "Please enter a valid number of guests.";
                return false;
            }

            if (!checkInDate.HasValue || !checkOutDate.HasValue)
            {
                error = "Please select both check-in and check-out dates.";
                return false;
            }

            if (checkInDate >= checkOutDate)
            {
                error = "Check-out date must be after check-in date.";
                return false;
            }

            booking = new Booking
            {
                GuestId = selectedGuest.GuestId,
                CheckInDate = checkInDate.Value,
                CheckOutDate = checkOutDate.Value,
                ReservationDate = DateTime.Now,
                NumberOfGuests = numGuests,
                Status = BookingStatus.Reserved
            };

            return true;
        }
    }
}
