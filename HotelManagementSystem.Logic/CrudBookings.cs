using HotelManagement.Core;

namespace HotelManagementSystem.Logic
{
    /// <summary>
    /// Classe responsável pelas operações CRUD relacionadas com reservas.
    /// </summary>
    public class CrudBookings
    {
        private string _filePath = @"Data\bookings.txt";

        public CrudBookings()
        {
            FileStorageHelper.EnsureFileExists(_filePath);
        }

        /// <summary>
        /// Lê todas as reservas existentes do ficheiro.
        /// </summary>
        public List<Booking> GetAll()
        {
            var bookings = new List<Booking>();
            var linhas = FileStorageHelper.ReadAllLines(_filePath);

            foreach (var linha in linhas)
            {
                var dados = linha.Split(';');

                if (dados.Length == 8 &&
                    int.TryParse(dados[0], out int id) &&
                    int.TryParse(dados[1], out int guestId) &&
                    int.TryParse(dados[2], out int roomNumber) &&
                    DateTime.TryParse(dados[3], out DateTime reservationDate) &&
                    DateTime.TryParse(dados[4], out DateTime checkInDate) &&
                    DateTime.TryParse(dados[5], out DateTime checkOutDate) &&
                    int.TryParse(dados[6], out int numberOfGuests) &&
                    Enum.TryParse(dados[7], out BookingStatus status))
                {
                    bookings.Add(new Booking
                    {
                        BookingId = id,
                        GuestId = guestId,
                        RoomId = roomNumber,
                        ReservationDate = reservationDate,
                        CheckInDate = checkInDate,
                        CheckOutDate = checkOutDate,
                        NumberOfGuests = numberOfGuests,
                        Status = status
                    });
                }
            }

            return bookings;
        }

        /// <summary>
        /// Adiciona uma nova reserva ao ficheiro.
        /// </summary>
        /// <param name="booking">Objeto Booking a adicionar.</param>
        public void Add(Booking booking)
        {
            string linha = $"{booking.BookingId};{booking.GuestId};{booking.RoomId};{booking.ReservationDate};{booking.CheckInDate};{booking.CheckOutDate};{booking.NumberOfGuests};{booking.Status}";
            FileStorageHelper.AppendLine(_filePath, linha);
        }

        /// <summary>
        /// Atualiza uma reserva existente com base no ID.
        /// </summary>
        public void Update(Booking updatedBooking)
        {
            var allBookings = GetAll();
            var index = allBookings.FindIndex(b => b.BookingId == updatedBooking.BookingId);

            if (index >= 0)
            {
                allBookings[index] = updatedBooking;
                SaveAll(allBookings);
            }
        }

        /// <summary>
        /// Remove uma reserva com base no ID.
        /// </summary>
        public void Delete(int bookingId)
        {
            var allBookings = GetAll();
            allBookings = allBookings.Where(b => b.BookingId != bookingId).ToList();
            SaveAll(allBookings);
        }

        /// <summary>
        /// Guarda todas as reservas no ficheiro.
        /// </summary>
        private void SaveAll(List<Booking> bookings)
        {
            var linhas = bookings.Select(b =>
                $"{b.BookingId};{b.GuestId};{b.RoomId};{b.ReservationDate};{b.CheckInDate};{b.CheckOutDate};{b.NumberOfGuests};{b.Status}");

            FileStorageHelper.WriteAllLines(_filePath, linhas);
        }

    }
}
