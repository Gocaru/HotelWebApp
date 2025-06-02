using HotelManagement.Core;

namespace HotelManagementSystem.Logic
{
    /// <summary>
    /// Classe auxiliar que fornece métodos de validação para diferentes entidades do sistema.
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Valida os dados de um objeto do tipo Guest.
        /// </summary>
        /// <param name="guest">Objeto Guest a validar.</param>
        /// <param name="mensagemErro">Mensagem de erro gerada em caso de validação falhada.</param>
        /// <returns>True se os dados forem válidos; False caso contrário.</returns>
        public static bool ValidateGuest(Guest guest, out string mensagemErro)
        {
            mensagemErro = "";

            if (!ValidateName(guest.Name))
            {
                mensagemErro = "Invalid name.";
            }
            else if (!ValidateContact(guest.Contact))
            {
                mensagemErro = "Invalid contact. It must contain only digits and be at least 9 characters long.";
            }
            else if (!ValidateEmail(guest.Email))
            {
                mensagemErro = "Invalid email address.";
            }
            else if (!ValidateDocument(guest.IdentificationDocument))
            {
                mensagemErro = "Invalid identification document. It must be at least 6 characters long.";
            }

            return string.IsNullOrEmpty(mensagemErro);
        }

        /// <summary>
        /// Valida os dados de um objeto do tipo Room.
        /// </summary>
        /// <param name="room">Objeto Room a validar.</param>
        /// <param name="mensagemErro">Mensagem de erro gerada em caso de validação falhada.</param>
        /// <returns>True se os dados forem válidos; False caso contrário.</returns>
        public static bool ValidateRoom(Room room, out string mensagemErro)
        {
            mensagemErro = "";

            if (room.Number <= 0)
            {
                mensagemErro = "Room number must be greater than zero.";
            }
            else if (room.Capacity <= 0)
            {
                mensagemErro = "Room capacity must be greater than zero.";
            }
            else if (room.PricePerNight < 0)
            {
                mensagemErro = "Price per night cannot be negative.";
            }
            return string.IsNullOrEmpty(mensagemErro);
        }

        /// <summary>
        /// Valida se um nome é válido (não está vazio ou só com espaços).
        /// </summary>
        /// <param name="name">Nome a validar.</param>
        /// <returns>True se o nome for válido; False caso contrário.</returns>
        public static bool ValidateName(string name)
        {
            return !string.IsNullOrWhiteSpace(name);
        }

        /// <summary>
        /// Valida se o contacto é composto apenas por dígitos e tem pelo menos 9 caracteres.
        /// </summary>
        /// <param name="contact">Contacto a validar.</param>
        /// <returns>True se o contacto for válido; False caso contrário.</returns>
        public static bool ValidateContact(string contact)
        {
            return !string.IsNullOrWhiteSpace(contact) &&
                   contact.All(char.IsDigit) &&
                   contact.Length >= 9;
        }

        /// <summary>
        /// Valida se um endereço de email é válido (não está vazio e contém os caracteres necessários).
        /// </summary>
        /// <param name="email">Email a validar.</param>
        /// <returns>True se o email for válido; False caso contrário.</returns>
        public static bool ValidateEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email) &&
                   email.Contains("@") &&
                   email.Contains(".");
        }

        /// <summary>
        /// Valida se o documento de identificação tem pelo menos 6 caracteres.
        /// </summary>
        /// <param name="document">Documento a validar.</param>
        /// <returns>True se o documento for válido; False caso contrário.</returns>
        public static bool ValidateDocument(string document)
        {
            return !string.IsNullOrWhiteSpace(document) &&
                   document.Length >= 6;
        }

        /// <summary>
        /// Valida os dados de uma reserva.
        /// </summary>
        /// <param name="booking">Reserva a validar.</param>
        /// <param name="mensagemErro">Mensagem de erro caso a validação falhe.</param>
        /// <returns>True se a reserva for válida, False caso contrário.</returns>
        public static bool ValidateBooking(Booking booking, out string mensagemErro)
        {
            mensagemErro = string.Empty;

            if (booking.GuestId <= 0)
            {
                mensagemErro = "Invalid Guest ID.";
            }
            else if (booking.RoomId <= 0)
            {
                mensagemErro = "Invalid Room Number.";
            }
            else if (booking.CheckInDate >= booking.CheckOutDate)
            {
                mensagemErro = "Check-out date must be after check-in date.";
            }
            else if (booking.NumberOfGuests <= 0)
            {
                mensagemErro = "Number of guests must be greater than zero.";
            }
            else if (booking.ReservationDate.Date > booking.CheckInDate.Date)
            {
                mensagemErro = "Reservation date must be on or before check-in date.";
            }

            return string.IsNullOrEmpty(mensagemErro);
        }

        /// <summary>
        /// Valida os dados de um serviço extra.
        /// </summary>
        /// <param name="service">Objeto ExtraService a validar.</param>
        /// <param name="mensagemErro">Mensagem de erro gerada em caso de falha.</param>
        /// <returns>True se o serviço for válido; False caso contrário.</returns>
        public static bool ValidateExtraService(ExtraService service, out string mensagemErro)
        {
            mensagemErro = string.Empty;

            if (service.BookingId <= 0)
            {
                mensagemErro = "Invalid booking ID.";
            }
            else if (!Enum.IsDefined(typeof(ExtraServiceType), service.ServiceType))
            {
                mensagemErro = "Invalid service type.";
            }
            else if (service.Price < 0)
            {
                mensagemErro = "Price cannot be negative.";
            }

            return string.IsNullOrEmpty(mensagemErro);
        }

        /// <summary>
        /// Valida os dados de uma fatura (Invoice).
        /// </summary>
        /// <param name="invoice">Fatura a validar.</param>
        /// <param name="mensagemErro">Mensagem de erro, se a validação falhar.</param>
        /// <returns>True se a fatura for válida; False caso contrário.</returns>
        public static bool ValidateInvoice(Invoice invoice, out string mensagemErro)
        {
            mensagemErro = string.Empty;

            if (invoice.BookingId <= 0)
            {
                mensagemErro = "Invalid Booking ID.";
            }
            else if (string.IsNullOrWhiteSpace(invoice.GuestName))
            {
                mensagemErro = "Guest name cannot be empty.";
            }
            else if (invoice.StayTotal < 0)
            {
                mensagemErro = "Stay total cannot be negative.";
            }
            else if (invoice.ExtrasTotal < 0)
            {
                mensagemErro = "Extras total cannot be negative.";
            }
            else if (invoice.IssueDate > DateTime.Now)
            {
                mensagemErro = "Issue date cannot be in the future.";
            }
            else if (!Enum.IsDefined(typeof(PaymentMethod), invoice.PaymentMethod))
            {
                mensagemErro = "Invalid payment method.";
            }

            return string.IsNullOrEmpty(mensagemErro);
        }

    }
}
