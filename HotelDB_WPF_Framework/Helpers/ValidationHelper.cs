using HotelDB_WPF_Framework.Enums;
using HotelDB_WPF_Framework.Models;
using System;
using System.Linq;

namespace HotelDB_WPF_Framework.Helpers
{
    /// <summary>
    /// Classe auxiliar que centraliza as validações das entidades principais do sistema já constuidas.
    /// </summary>
    public static class ValidationHelper        //Para poder ser chamado facilmente a partir de qualquer parte do projeto é uma classe public static
    {
        /// <summary>
        /// Valida os dados de um hóspede, verificando se os campos obrigatórios estão preenchidos e válidos.
        /// </summary>
        public static bool ValidateGuest(Guest guest, out string error)
        {
            if (string.IsNullOrWhiteSpace(guest.Name))
            {
                error = "Name is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(guest.Contact) || guest.Contact.Length < 9 || !guest.Contact.All(char.IsDigit))
            {
                error = "Contact must be numeric and at least 9 digits.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(guest.Email) || !guest.Email.Contains("@"))
            {
                error = "A valid email is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(guest.IdentificationDocument) || guest.IdentificationDocument.Length < 6)
            {
                error = "Identification document must have at least 6 characters.";
                return false;
            }

            error = "";
            return true;
        }


        /// <summary>
        /// Valida os dados de um quarto, assegurando que os campos principais estão preenchidos e coerentes.
        /// </summary>
        public static bool ValidateRoom(Room room, out string error)
        {
            if (room.Number <= 0)
            {
                error = "Room number must be greater than 0.";
                return false;
            }

            if (room.Capacity <= 0)
            {
                error = "Room capacity must be at least 1.";
                return false;
            }

            if (room.PricePerNight < 0)
            {
                error = "Room price cannot be negative.";
                return false;
            }

            error = "";
            return true;
        }


        /// <summary>
        /// Valida os dados de uma fatura, verificando se os totais são válidos e o método de pagamento está definido.
        /// </summary>
        public static bool ValidateInvoice(Invoice invoice, out string error)
        {
            if (invoice.BookingId <= 0)
            {
                error = "Invalid booking ID.";
                return false;
            }

            if (invoice.StayTotal < 0 || invoice.ExtrasTotal < 0)
            {
                error = "Totals cannot be negative.";
                return false;
            }

            if (!Enum.IsDefined(typeof(InvoicePaymentMethod), invoice.PaymentMethod))
            {
                error = "Invalid payment method.";
                return false;
            }

            error = "";     //Se não há mensagens de erro
            return true;    //a validação é bem sucedida
        }

        /// <summary>
        /// Valida os dados de um serviço extra associado a uma reserva.
        /// </summary>
        public static bool ValidateExtraService(ExtraService extra, out string error)
        {
            if (extra.BookingId <= 0)
            {
                error = "Invalid booking ID.";
                return false;
            }

            if (!Enum.IsDefined(typeof(ExtraServiceType), extra.ServiceType))
            {
                error = "Invalid service type.";
                return false;
            }

            if (extra.Price < 0)
            {
                error = "Price cannot be negative.";
                return false;
            }

            error = "";
            return true;
        }

        /// <summary>
        /// Valida os dados de uma reserva, verificando datas, número de hóspedes e estado.
        /// </summary>
        public static bool ValidateBooking(Booking booking, out string error)
        {
            if (booking.GuestId <= 0)
            {
                error = "Invalid guest ID.";
                return false;
            }

            if (booking.RoomId <= 0)
            {
                error = "Invalid room ID.";
                return false;
            }

            if (booking.CheckInDate >= booking.CheckOutDate)
            {
                error = "Check-out date must be after check-in date.";
                return false;
            }

            if (booking.NumberOfGuests <= 0)
            {
                error = "Number of guests must be at least 1.";
                return false;
            }

            if (!Enum.IsDefined(typeof(BookingStatus), booking.Status))
            {
                error = "Invalid booking status.";
                return false;
            }

            error = "";
            return true;
        }

    }
}
