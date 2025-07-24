using HotelWebApp.Data;
using HotelWebApp.Data.Entities;
using HotelWebApp.Data.Repositories;
using HotelWebApp.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Services
{
    public class ReservationService : IReservationService
    {
        private readonly HotelWebAppContext _context;
        private readonly IReservationRepository _reservationRepo;
        private readonly IAmenityRepository _amenityRepo;
        private readonly IRoomRepository _roomRepo;
        private readonly IEmailSender _emailSender;

        public ReservationService(HotelWebAppContext context, IReservationRepository reservationRepo, IRoomRepository roomRepo, IEmailSender emailSender, IAmenityRepository amenityRepo)
        {
            _context = context;
            _reservationRepo = reservationRepo;
            _roomRepo = roomRepo;
            _emailSender = emailSender;
            _amenityRepo = amenityRepo;
        }

        public async Task<Result> CreateReservationAsync(ReservationViewModel model, string guestId)
        {
            if (model.CheckOutDate <= model.CheckInDate)
                return Result.Failure("Check-out date must be after the check-in date.");

            var checkInWithTime = new DateTime(model.CheckInDate.Year, model.CheckInDate.Month, model.CheckInDate.Day, 12, 0, 0);
            var checkOutWithTime = new DateTime(model.CheckOutDate.Year, model.CheckOutDate.Month, model.CheckOutDate.Day, 11, 0, 0);

            if (!await _reservationRepo.IsRoomAvailableAsync(model.RoomId, checkInWithTime, checkOutWithTime, null))
                return Result.Failure("This room is not available for the selected dates.");

            var room = await _roomRepo.GetByIdAsync(model.RoomId);
            if (room == null) return Result.Failure("Room not found.");

            if (model.NumberOfGuests > room.Capacity)
                return Result.Failure($"The selected room only accommodates up to {room.Capacity} guests.");

            var numberOfNights = (checkOutWithTime.Date - checkInWithTime.Date).Days;

            var reservation = new Reservation
            {
                GuestId = guestId,
                RoomId = model.RoomId,
                CheckInDate = checkInWithTime,
                CheckOutDate = checkOutWithTime,
                NumberOfGuests = model.NumberOfGuests,
                Status = ReservationStatus.Confirmed,
                TotalPrice = numberOfNights * room.PricePerNight
            };

            await _reservationRepo.CreateAsync(reservation);

            room.Status = RoomStatus.Reserved;
            await _roomRepo.UpdateAsync(room);

            return Result.Success();
        }

        public async Task<Result> UpdateReservationAsync(int reservationId, ReservationViewModel model)
        {
            // Carrega a reserva com todos os detalhes necessários (incluindo o quarto)
            var reservation = await _reservationRepo.GetByIdWithDetailsAsync(reservationId);
            if (reservation == null) return Result.Failure("Reservation not found.");

            var oldStatus = reservation.Status;
            var newStatus = model.Status;


            if (oldStatus != newStatus)
            {
                // O utilizador está a tentar mudar o estado da reserva.

                if (newStatus == ReservationStatus.Cancelled)
                {
                    // Cancelar uma Reserva
                    if (oldStatus != ReservationStatus.Confirmed)
                    {
                        return Result.Failure($"Only 'Confirmed' reservations can be cancelled via this form.");
                    }

                    await CancelReservationLogic(reservation);
                    return Result.Success();
                }

                if (oldStatus == ReservationStatus.Cancelled && newStatus == ReservationStatus.Confirmed)
                {
                    // Reativar uma Reserva Cancelada
                    // garantir que o quarto ainda está disponível.
                    if (!await _reservationRepo.IsRoomAvailableAsync(reservation.RoomId, reservation.CheckInDate, reservation.CheckOutDate, reservationId))
                    {
                        return Result.Failure("The room is no longer available to reactivate this reservation.");
                    }

                    reservation.Status = ReservationStatus.Confirmed;
                    if (reservation.Room != null)
                    {
                        reservation.Room.Status = RoomStatus.Reserved;
                    }

                    await _reservationRepo.UpdateAsync(reservation);
                    return Result.Success();
                }

                return Result.Failure($"Status change from '{oldStatus}' to '{newStatus}' is not supported via the edit form. Use the specific Check-in/Check-out actions.");
            }


            // o estado não mudou, então é uma edição dos dados da reserva.
            switch (reservation.Status)
            {
                case ReservationStatus.Confirmed:
                case ReservationStatus.CheckedIn:
                    // Permite a edição nestes estados, com regras específicas abaixo.
                    break;
                default:
                    // Não permite edição em outros estados como 'Cancelled' ou 'CheckedOut'.
                    return Result.Failure($"Cannot edit a reservation with status '{reservation.Status}'.");
            }

            if (model.CheckOutDate <= model.CheckInDate)
            {
                return Result.Failure("Check-out date must be after the check-in date.");
            }

            var checkInWithTime = new DateTime(model.CheckInDate.Year, model.CheckInDate.Month, model.CheckInDate.Day, 12, 0, 0);
            var checkOutWithTime = new DateTime(model.CheckOutDate.Year, model.CheckOutDate.Month, model.CheckOutDate.Day, 11, 0, 0);

            // Validação específica para o estado CheckedIn
            if (reservation.Status == ReservationStatus.CheckedIn && (reservation.CheckInDate.Date != checkInWithTime.Date || reservation.RoomId != model.RoomId))
            {
                return Result.Failure("Cannot change check-in date or room for a reservation that is already checked-in.");
            }

            if (!await _reservationRepo.IsRoomAvailableAsync(model.RoomId, checkInWithTime, checkOutWithTime, reservationId))
            {
                return Result.Failure("This room is not available for the selected dates.");
            }

            // liberta o quarto antigo, se o quarto for alterado.
            if (reservation.RoomId != model.RoomId)
            {
                var oldRoom = await _roomRepo.GetByIdAsync(reservation.RoomId);
                if (oldRoom != null)
                {
                    oldRoom.Status = RoomStatus.Available;
                    await _roomRepo.UpdateAsync(oldRoom);
                }
            }

            var newRoom = await _roomRepo.GetByIdAsync(model.RoomId);
            if (newRoom == null) return Result.Failure("New room not found.");

            // Atualiza os dados da reserva
            var numberOfNights = (checkOutWithTime.Date - checkInWithTime.Date).Days;
            reservation.GuestId = model.GuestId;
            reservation.RoomId = model.RoomId;
            reservation.CheckInDate = checkInWithTime;
            reservation.CheckOutDate = checkOutWithTime;
            reservation.NumberOfGuests = model.NumberOfGuests;
            reservation.TotalPrice = numberOfNights * newRoom.PricePerNight;
            // O Status não é alterado aqui porque estamos no fluxo de "edição normal".

            await _reservationRepo.UpdateAsync(reservation);

            // Se o quarto mudou, o novo quarto deve ser marcado como reservado.
            if (reservation.RoomId != model.RoomId)
            {
                newRoom.Status = RoomStatus.Reserved;
                await _roomRepo.UpdateAsync(newRoom);
            }

            return Result.Success();
        }

        public async Task<Result> CheckInReservationAsync(int reservationId)
        {
            var reservation = await _reservationRepo.GetByIdWithDetailsAsync(reservationId);
            if (reservation == null) return Result.Failure("Reservation not found.");

            if (reservation.Status != ReservationStatus.Confirmed)
                return Result.Failure("Only 'Confirmed' reservations can be checked in.");

            if (reservation.CheckInDate.Date != DateTime.Today.Date)
                return Result.Failure("Check-in is only allowed on the scheduled check-in date.");

            reservation.Status = ReservationStatus.CheckedIn;
            reservation.Room.Status = RoomStatus.Occupied;

            await _reservationRepo.UpdateAsync(reservation);
            return Result.Success();
        }

        public async Task<Result> CheckOutReservationAsync(int reservationId)
        {
            var reservation = await _reservationRepo.GetByIdWithDetailsAsync(reservationId);
            if (reservation == null) return Result.Failure("Reservation not found.");

            if (reservation.Status != ReservationStatus.CheckedIn)
                return Result.Failure("Only 'Checked-in' reservations can be checked out.");

            reservation.Status = ReservationStatus.CheckedOut;
            reservation.Room.Status = RoomStatus.Available;

            await _reservationRepo.UpdateAsync(reservation);
            return Result.Success();
        }

        public async Task<Result> CancelReservationByEmployeeAsync(int reservationId)
        {
            var reservation = await _reservationRepo.GetByIdWithDetailsAsync(reservationId);
            if (reservation == null) return Result.Failure("Reservation not found.");

            // Regra de negócio: um funcionário pode cancelar uma reserva confirmada a qualquer momento antes do check-in.
            if (reservation.Status != ReservationStatus.Confirmed)
                return Result.Failure("Only 'Confirmed' reservations can be cancelled.");

            // Um funcionário pode ter permissão para cancelar no próprio dia.
            if (reservation.CheckInDate.Date <= DateTime.Today.Date)
                return Result.Failure("This reservation cannot be cancelled on or after the check-in date via this system.");

            // Guarda os detalhes do email ANTES de a reserva ser alterada.
            string guestEmail = reservation.ApplicationUser.Email;
            string guestName = reservation.ApplicationUser.FullName;
            string checkInDate = reservation.CheckInDate.ToShortDateString();
            string roomNumber = reservation.Room.RoomNumber;

            await CancelReservationLogic(reservation);

            try
            {
                var subject = "Reservation Cancellation Confirmation";
                var message = $@"
                    <html>
                    <body>
                        <p>Dear {guestName},</p>
                        <p>This email is to confirm that your reservation at HotelWebApp has been cancelled by our staff.</p>
                        <p><strong>Reservation Details:</strong></p>
                        <ul>
                            <li>Room Number: {roomNumber}</li>
                            <li>Check-in Date: {checkInDate}</li>
                        </ul>
                        <p>If you have any questions, please contact our support.</p>
                        <p>Thank you,</p>
                        <p>The HotelWebApp Team</p>
                    </body>
                    </html>";

                await _emailSender.SendEmailAsync(guestEmail, subject, message);
            }
            catch (Exception ex)
            {
                // A reserva foi cancelada, mas o email falhou. Retornamos um SUCESSO com AVISO.
                // A mensagem de aviso informa o funcionário sobre o problema.
                return Result.SuccessWithWarning(
                    $"The reservation was cancelled successfully, but the notification email to '{guestEmail}' could not be sent. Please verify the email address. Error: {ex.Message}");
            }

            return Result.Success();
        }

        public async Task<Result> CancelReservationByGuestAsync(int reservationId, string guestId)
        {
            var reservation = await _reservationRepo.GetByIdWithDetailsAsync(reservationId);
            if (reservation == null) return Result.Failure("Reservation not found.");

            // Segurança: Garante que o hóspede só cancela as suas próprias reservas.
            if (reservation.GuestId != guestId)
                return Result.Failure("Unauthorized to cancel this reservation.");

            if (reservation.Status != ReservationStatus.Confirmed)
                return Result.Failure("Only 'Confirmed' reservations can be cancelled.");

            if (reservation.CheckInDate.Date <= DateTime.Today.Date)
                return Result.Failure("Reservations cannot be cancelled on or after the check-in date.");

            await CancelReservationLogic(reservation);
            return Result.Success();
        }

        /// <summary>
        /// Método privado para conter a lógica de cancelamento partilhada, evitando duplicação de código.
        /// </summary>
        private async Task CancelReservationLogic(Reservation reservation)
        {
            reservation.Status = ReservationStatus.Cancelled;
            if (reservation.Room != null)
            {
                reservation.Room.Status = RoomStatus.Available;
            }
            await _reservationRepo.UpdateAsync(reservation);
        }

        public async Task<Result> AddAmenityToReservationAsync(int reservationId, int amenityId, int quantity)
        {
            if (quantity <= 0)
            {
                return Result.Failure("Quantity must be greater than zero."); 
            }

            var reservation = await _reservationRepo.GetByIdAsync(reservationId);
            if (reservation == null)
            {
                return Result.Failure("Reservation not found."); 
            }

            var amenity = await _amenityRepo.GetByIdAsync(amenityId);
            if (amenity == null)
            {
                return Result.Failure("Amenity not found."); 
            }

            // Criar a nova entidade de junção
            var reservationAmenity = new ReservationAmenity
            {
                ReservationId = reservation.Id,
                AmenityId = amenity.Id,
                Quantity = quantity,
                PriceAtTimeOfBooking = amenity.Price,
                DateAdded = DateTime.UtcNow
            };

            reservation.ReservationAmenities.Add(reservationAmenity);

            // Recalcula o preço total da reserva
            decimal amenityCost = amenity.Price * quantity;
            reservation.TotalPrice += amenityCost;

            try
            {
                await _reservationRepo.UpdateAsync(reservation);
                return Result.Success(); 
            }
            catch (DbUpdateException ex)
            {
                // TODO: Log do erro 'ex'
                return Result.Failure("An error occurred while saving to the database.");
            }
        }

        public async Task<Result> RemoveAmenityFromReservationAsync(int reservationId, int reservationAmenityId)
        {
            var reservation = await _reservationRepo.GetByIdWithDetailsAsync(reservationId);
            if (reservation == null)
            {
                return Result.Failure("Reservation not found.");
            }

            var reservationAmenity = reservation.ReservationAmenities.FirstOrDefault(ra => ra.Id == reservationAmenityId);
            if (reservationAmenity == null)
            {
                return Result.Failure("Amenity link not found for this reservation.");
            }

            // Subtrai o custo da amenity do preço total
            decimal amenityCost = reservationAmenity.PriceAtTimeOfBooking * reservationAmenity.Quantity;
            reservation.TotalPrice -= amenityCost;

            reservation.ReservationAmenities.Remove(reservationAmenity);

            try
            {
                await _reservationRepo.UpdateAsync(reservation);
                return Result.Success();
            }
            catch (DbUpdateException ex)
            {
                return Result.Failure("An error occurred while removing the amenity.");
            }
        }

        public async Task<Result> MarkPastReservationsAsNoShowAsync()
        {
            try
            {
                var today = DateTime.Today;

                // 1. Encontrar todas as reservas 'Confirmed' cuja data de check-in é anterior a hoje
                var reservationsToUpdate = await _context.Reservations
                    .Include(r => r.Room) // Incluir o quarto para podermos atualizá-lo
                    .Where(r => r.Status == ReservationStatus.Confirmed && r.CheckInDate.Date < today)
                    .ToListAsync();

                if (!reservationsToUpdate.Any())
                {
                    return Result.Success("No past-due reservations found to process.");
                }

                foreach (var reservation in reservationsToUpdate)
                {
                    // 2. Mudar o status da reserva para NoShow
                    reservation.Status = ReservationStatus.NoShow;

                    // 3. Libertar o quarto se ele ainda estava como 'Reservado'
                    if (reservation.Room != null && reservation.Room.Status == RoomStatus.Reserved)
                    {
                        reservation.Room.Status = RoomStatus.Available;
                    }
                }

                // 4. Salvar todas as alterações de uma só vez na base de dados
                await _context.SaveChangesAsync();

                string successMessage = reservationsToUpdate.Count == 1
                    ? "1 reservation was successfully marked as No-Show."
                    : $"{reservationsToUpdate.Count} reservations were successfully marked as No-Show.";

                return Result.Success(successMessage);
            }
            catch (Exception ex)
            {
                // Em caso de um erro inesperado na base de dados, retorna uma mensagem de falha
                // Opcional: Logar o erro 'ex' para fins de depuração
                return Result.Failure("An error occurred while processing no-show reservations. Please try again.");
            }
        }
    }
}
