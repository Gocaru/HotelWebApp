using HotelWebApp.Data;
using HotelWebApp.Data.Entities;
using HotelWebApp.Data.Repositories;
using HotelWebApp.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Services
{
    /// <summary>
    /// Implements the IReservationService interface, providing the core business logic
    /// for managing the entire lifecycle of a reservation.
    /// </summary>
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepo;
        private readonly IAmenityRepository _amenityRepo;
        private readonly IRoomRepository _roomRepo;
        private readonly IEmailSender _emailSender;
        private readonly IPaymentService _paymentService;
        private readonly IPromotionRepository _promotionRepo;

        public ReservationService(
            IReservationRepository reservationRepo, 
            IRoomRepository roomRepo, 
            IEmailSender emailSender, 
            IAmenityRepository amenityRepo, 
            IPaymentService paymentService, 
            IPromotionRepository promotionRepo)
        {
            _reservationRepo = reservationRepo;
            _roomRepo = roomRepo;
            _emailSender = emailSender;
            _amenityRepo = amenityRepo;
            _paymentService = paymentService;
            _promotionRepo = promotionRepo;
        }

        public async Task<Result> CreateReservationAsync(ReservationViewModel model, string guestId)
        {
            // --- Business Rule Validations ---
            if (model.CheckOutDate <= model.CheckInDate)
                return Result.Failure("Check-out date must be after the check-in date.");

            // Define standard check-in/out times for consistency.
            var checkInWithTime = new DateTime(model.CheckInDate.Year, model.CheckInDate.Month, model.CheckInDate.Day, 12, 0, 0); // Check-in at 12:00 PM
            var checkOutWithTime = new DateTime(model.CheckOutDate.Year, model.CheckOutDate.Month, model.CheckOutDate.Day, 11, 0, 0); // Check-out at 11:00 AM

            if (!await _reservationRepo.IsRoomAvailableAsync(model.RoomId, checkInWithTime, checkOutWithTime, null))
                return Result.Failure("This room is not available for the selected dates.");

            var room = await _roomRepo.GetByIdAsync(model.RoomId);
            if (room == null) return Result.Failure("Room not found.");

            if (model.NumberOfGuests > room.Capacity)
                return Result.Failure($"The selected room only accommodates up to {room.Capacity} guests.");
            // --- End of Validations ---

            var numberOfNights = (checkOutWithTime.Date - checkInWithTime.Date).Days;

            decimal originalPrice = numberOfNights * room.PricePerNight;
            decimal finalPrice = originalPrice;
            decimal? discountPercentage = null;
            int? promotionId = null;

            if (model.PromotionId.HasValue && model.PromotionId.Value > 0)
            {
                var promotion = await _promotionRepo.GetActiveByIdAsync(model.PromotionId.Value);  // ✅

                if (promotion != null)
                {
                    // Validar se a promoção está válida para as datas da reserva
                    if (checkInWithTime.Date >= promotion.StartDate.Date &&
                        checkInWithTime.Date <= promotion.EndDate.Date)
                    {
                        promotionId = promotion.Id;
                        discountPercentage = promotion.DiscountPercentage;

                        decimal discountAmount = (originalPrice * discountPercentage.Value) / 100;
                        finalPrice = originalPrice - discountAmount;

                        System.Diagnostics.Debug.WriteLine($"🎁 Promotion Applied: {promotion.Title}");
                        System.Diagnostics.Debug.WriteLine($"   Original Price: {originalPrice:C}");
                        System.Diagnostics.Debug.WriteLine($"   Discount: {discountPercentage}% = {discountAmount:C}");
                        System.Diagnostics.Debug.WriteLine($"   Final Price: {finalPrice:C}");
                    }
                }
            }

            var reservation = new Reservation
            {
                GuestId = guestId,
                RoomId = model.RoomId,
                CheckInDate = checkInWithTime,
                CheckOutDate = checkOutWithTime,
                NumberOfGuests = model.NumberOfGuests,
                Status = ReservationStatus.Confirmed,
                PromotionId = promotionId,
                OriginalPrice = originalPrice,
                DiscountPercentage = discountPercentage,
                TotalPrice = finalPrice  // Preço final com desconto aplicado
            };

            System.Diagnostics.Debug.WriteLine($"   Creating reservation for Employee:");
            System.Diagnostics.Debug.WriteLine($"   GuestId from parameter: {guestId}");
            System.Diagnostics.Debug.WriteLine($"   GuestId in model: {model.GuestId}");
            System.Diagnostics.Debug.WriteLine($"   GuestId in reservation object: {reservation.GuestId}");
            System.Diagnostics.Debug.WriteLine($"   RoomId: {model.RoomId}");
            System.Diagnostics.Debug.WriteLine($"   Dates: {checkInWithTime:yyyy-MM-dd} to {checkOutWithTime:yyyy-MM-dd}");

            await _reservationRepo.CreateAsync(reservation);

            System.Diagnostics.Debug.WriteLine($"✅ Reservation created with ID: {reservation.Id}");

            // Update the room's status to Reserved to block it for other bookings.
            room.Status = RoomStatus.Reserved;
            await _roomRepo.UpdateAsync(room);

            return Result.Success();
        }

        public async Task<Result> UpdateReservationAsync(int reservationId, ReservationViewModel model)
        {
            var reservation = await _reservationRepo.GetByIdWithDetailsAsync(reservationId);
            if (reservation == null) return Result.Failure("Reservation not found.");

            var oldStatus = reservation.Status;
            var newStatus = model.Status;


            if (oldStatus != newStatus)
            {
                if (newStatus == ReservationStatus.Cancelled)
                {
                    if (oldStatus != ReservationStatus.Confirmed)
                    {
                        return Result.Failure($"Only 'Confirmed' reservations can be cancelled via this form.");
                    }

                    await CancelReservationLogic(reservation);
                    return Result.Success();
                }

                if (oldStatus == ReservationStatus.Cancelled && newStatus == ReservationStatus.Confirmed)
                {

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


            switch (reservation.Status)
            {
                case ReservationStatus.Confirmed:
                case ReservationStatus.CheckedIn:

                    break;
                default:

                    return Result.Failure($"Cannot edit a reservation with status '{reservation.Status}'.");
            }

            if (model.CheckOutDate <= model.CheckInDate)
            {
                return Result.Failure("Check-out date must be after the check-in date.");
            }

            var checkInWithTime = new DateTime(model.CheckInDate.Year, model.CheckInDate.Month, model.CheckInDate.Day, 12, 0, 0);
            var checkOutWithTime = new DateTime(model.CheckOutDate.Year, model.CheckOutDate.Month, model.CheckOutDate.Day, 11, 0, 0);

            if (reservation.Status == ReservationStatus.CheckedIn && (reservation.CheckInDate.Date != checkInWithTime.Date || reservation.RoomId != model.RoomId))
            {
                return Result.Failure("Cannot change check-in date or room for a reservation that is already checked-in.");
            }

            if (!await _reservationRepo.IsRoomAvailableAsync(model.RoomId, checkInWithTime, checkOutWithTime, reservationId))
            {
                return Result.Failure("This room is not available for the selected dates.");
            }


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


            var numberOfNights = (checkOutWithTime.Date - checkInWithTime.Date).Days;
            reservation.GuestId = model.GuestId;
            reservation.RoomId = model.RoomId;
            reservation.CheckInDate = checkInWithTime;
            reservation.CheckOutDate = checkOutWithTime;
            reservation.NumberOfGuests = model.NumberOfGuests;
            reservation.TotalPrice = numberOfNights * newRoom.PricePerNight;


            await _reservationRepo.UpdateAsync(reservation);


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


            if (reservation.Status != ReservationStatus.Confirmed)
                return Result.Failure("Only 'Confirmed' reservations can be cancelled.");


            if (reservation.CheckInDate.Date <= DateTime.Today.Date)
                return Result.Failure("This reservation cannot be cancelled on or after the check-in date via this system.");


            string guestEmail = reservation.ApplicationUser.Email;
            string guestName = reservation.ApplicationUser.FullName;
            string checkInDate = reservation.CheckInDate.ToShortDateString();
            string roomNumber = reservation.Room.RoomNumber;

            // Encapsulate the actual cancellation logic in a private method to avoid code duplication.
            await CancelReservationLogic(reservation);

            // Try to send a notification email to the guest.
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
                // If the email fails, the operation is still a success, but with a warning.
                return Result.SuccessWithWarning(
                    $"The reservation was cancelled successfully, but the notification email to '{guestEmail}' could not be sent. Please verify the email address. Error: {ex.Message}");
            }

            return Result.Success();
        }

        public async Task<Result> CancelReservationByGuestAsync(int reservationId, string guestId)
        {
            var reservation = await _reservationRepo.GetByIdWithDetailsAsync(reservationId);
            if (reservation == null) return Result.Failure("Reservation not found.");

            if (reservation.GuestId != guestId)
                return Result.Failure("Unauthorized to cancel this reservation.");

            if (reservation.Status != ReservationStatus.Confirmed)
                return Result.Failure("Only 'Confirmed' reservations can be cancelled.");

            if (reservation.CheckInDate.Date <= DateTime.Today.Date)
                return Result.Failure("Reservations cannot be cancelled on or after the check-in date.");

            // Re-use the same cancellation logic.
            await CancelReservationLogic(reservation);
            return Result.Success();
        }

        /// <summary>
        /// Private helper method to contain the shared logic for cancelling a reservation.
        /// This avoids code duplication between guest-initiated and employee-initiated cancellations.
        /// </summary>
        private async Task CancelReservationLogic(Reservation reservation)
        {
            reservation.Status = ReservationStatus.Cancelled;
            if (reservation.Room != null)
            {
                reservation.Room.Status = RoomStatus.Available;
            }
            await _reservationRepo.CancelActivityBookingsForReservationAsync(reservation.Id);

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

            // Create the join entity instance.
            var reservationAmenity = new ReservationAmenity
            {
                ReservationId = reservation.Id,
                AmenityId = amenity.Id,
                Quantity = quantity,
                // Store the price at the time of booking for historical accuracy.
                PriceAtTimeOfBooking = amenity.Price,
                DateAdded = DateTime.UtcNow
            };

            reservation.ReservationAmenities.Add(reservationAmenity);

            decimal amenityCost = amenity.Price * quantity;

            // Recalculate the total price of the reservation.
            reservation.TotalPrice += amenityCost;

            try
            {
                await _reservationRepo.UpdateAsync(reservation);
                return Result.Success(); 
            }
            catch (DbUpdateException ex)
            {
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
                var reservationsToUpdate = await _reservationRepo.GetPastConfirmedReservationsAsync();  // ✅

                if (!reservationsToUpdate.Any())
                {
                    return Result.Success("No past-due reservations found to process.");
                }

                foreach (var reservation in reservationsToUpdate)
                {
                    reservation.Status = ReservationStatus.NoShow;

                    if (reservation.Room != null && reservation.Room.Status == RoomStatus.Reserved)
                    {
                        reservation.Room.Status = RoomStatus.Available;
                    }

                    await _paymentService.CreateInvoiceForNoShowAsync(reservation.Id);
                }

                string successMessage = reservationsToUpdate.Count() == 1
                    ? "1 reservation was successfully marked as No-Show."
                    : $"{reservationsToUpdate.Count()} reservations were successfully marked as No-Show.";

                return Result.Success(successMessage);
            }
            catch (Exception ex)
            {
                return Result.Failure("An error occurred while processing no-show reservations. Please try again.");
            }
        }
    }
    
}
