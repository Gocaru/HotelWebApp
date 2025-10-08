using HotelWebApp.Data;
using HotelWebApp.Data.Entities;
using HotelWebApp.Models.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HotelWebApp.Controllers.Api
{
    /// <summary>
    /// Manages hotel activities that guests can book, such as spa, tours, and gym access
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {
        private readonly HotelWebAppContext _context;

        public ActivitiesController(HotelWebAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all active activities available at the hotel
        /// </summary>
        /// <returns>List of active activities with pricing and schedule information</returns>
        // GET: api/activities
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<ActivityDto>>>> GetActivities()
        {
            try
            {
                var activities = await _context.Activities
                    .Where(a => a.IsActive)
                    .ToListAsync();

                var activityDtos = activities.Select(a => new ActivityDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Price = a.Price,
                    Duration = a.Duration,
                    Schedule = a.Schedule,
                    Capacity = a.Capacity,
                    ImageUrl = a.ImageUrl,
                    CurrentParticipants = 0
                }).ToList();

                return Ok(new ApiResponse<List<ActivityDto>>
                {
                    Success = true,
                    Data = activityDtos,
                    Message = $"Retrieved {activityDtos.Count} activities"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<ActivityDto>>
                {
                    Success = false,
                    Message = "Error retrieving activities",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Retrieves details of a specific activity by ID
        /// </summary>
        /// <param name="id">The activity ID</param>
        /// <returns>Detailed information about the activity</returns>
        // GET: api/activities/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<ActivityDto>>> GetActivity(int id)
        {
            try
            {
                var activity = await _context.Activities
                    .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);

                if (activity == null)
                {
                    return NotFound(new ApiResponse<ActivityDto>
                    {
                        Success = false,
                        Message = "Activity not found"
                    });
                }

                var currentParticipants = await _context.ActivityBookings
                    .Where(ab => ab.ActivityId == id
                        && (ab.Status == ActivityBookingStatus.Pending
                            || ab.Status == ActivityBookingStatus.Confirmed))
                    .SumAsync(ab => ab.NumberOfPeople);

                var activityDto = new ActivityDto
                {
                    Id = activity.Id,
                    Name = activity.Name,
                    Description = activity.Description,
                    Price = activity.Price,
                    Duration = activity.Duration,
                    Schedule = activity.Schedule,
                    Capacity = activity.Capacity,
                    ImageUrl = activity.ImageUrl,
                    CurrentParticipants = currentParticipants 
                };

                return Ok(new ApiResponse<ActivityDto>
                {
                    Success = true,
                    Data = activityDto,
                    Message = "Activity retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ActivityDto>
                {
                    Success = false,
                    Message = "Error retrieving activity",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Check activity availability for a specific date
        /// </summary>
        /// <param name="id">Activity ID</param>
        /// <param name="date">Date to check (format: yyyy-MM-dd)</param>
        /// <returns>Availability information for the specified date</returns>
        // GET: api/activities/{id}/availability?date=2025-01-15
        [HttpGet("{id}/availability")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<ActivityAvailabilityDto>>> GetActivityAvailability(
            int id,
            [FromQuery] DateTime date)
        {
            try
            {
                var activity = await _context.Activities
                    .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);

                if (activity == null)
                {
                    return NotFound(new ApiResponse<ActivityAvailabilityDto>
                    {
                        Success = false,
                        Message = "Activity not found"
                    });
                }

                // Validar que a data não é no passado
                if (date.Date < DateTime.Today)
                {
                    return BadRequest(new ApiResponse<ActivityAvailabilityDto>
                    {
                        Success = false,
                        Message = "Cannot check availability for past dates"
                    });
                }

                var currentParticipants = await _context.ActivityBookings
                    .Where(ab => ab.ActivityId == id
                        && ab.ScheduledDate.Date == date.Date  // ✅ DATA ESPECÍFICA
                        && (ab.Status == ActivityBookingStatus.Pending
                            || ab.Status == ActivityBookingStatus.Confirmed))
                    .SumAsync(ab => ab.NumberOfPeople);

                var availableSpots = activity.Capacity - currentParticipants;

                var availabilityDto = new ActivityAvailabilityDto
                {
                    ActivityId = activity.Id,
                    ActivityName = activity.Name,
                    Date = date.Date,
                    Capacity = activity.Capacity,
                    CurrentParticipants = currentParticipants,
                    AvailableSpots = Math.Max(0, availableSpots),
                    IsFull = currentParticipants >= activity.Capacity
                };

                return Ok(new ApiResponse<ActivityAvailabilityDto>
                {
                    Success = true,
                    Data = availabilityDto,
                    Message = $"Availability checked for {date:yyyy-MM-dd}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ActivityAvailabilityDto>
                {
                    Success = false,
                    Message = "Error checking availability",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Books an activity for the authenticated guest
        /// </summary>
        /// <param name="id">The activity ID to book</param>
        /// <param name="request">Booking details including scheduled date and number of people</param>
        /// <returns>Confirmation of the activity booking</returns>
        // POST: api/activities/{id}/book
        [HttpPost("{id}/book")]
        [Authorize(AuthenticationSchemes = "ApiScheme")]
        public async Task<ActionResult<ApiResponse<ActivityBookingDto>>> BookActivity(
            int id,
            [FromBody] BookActivityRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<ActivityBookingDto>
                    {
                        Success = false,
                        Message = "User not found in token"
                    });
                }

                var activity = await _context.Activities
                    .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);

                if (activity == null)
                {
                    return NotFound(new ApiResponse<ActivityBookingDto>
                    {
                        Success = false,
                        Message = "Activity not found"
                    });
                }

                if (request.ScheduledDate < DateTime.Today)
                {
                    return BadRequest(new ApiResponse<ActivityBookingDto>
                    {
                        Success = false,
                        Message = "Scheduled date cannot be in the past"
                    });
                }

                if (request.NumberOfPeople < 1 || request.NumberOfPeople > activity.Capacity)
                {
                    return BadRequest(new ApiResponse<ActivityBookingDto>
                    {
                        Success = false,
                        Message = $"Number of people must be between 1 and {activity.Capacity}"
                    });
                }

                // Verificar se o utilizador tem uma reserva ativa
                Reservation? activeReservation = null;
                if (request.ReservationId.HasValue)
                {
                    activeReservation = await _context.Reservations
                        .FirstOrDefaultAsync(r => r.Id == request.ReservationId.Value
                            && r.GuestId == userId
                            && (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.CheckedIn));

                    if (activeReservation == null)
                    {
                        return BadRequest(new ApiResponse<ActivityBookingDto>
                        {
                            Success = false,
                            Message = "Invalid or inactive reservation"
                        });
                    }
                }

                var totalPrice = activity.Price * request.NumberOfPeople;

                var booking = new ActivityBooking
                {
                    ActivityId = id,
                    GuestId = userId,
                    ReservationId = request.ReservationId,
                    BookingDate = DateTime.UtcNow,
                    ScheduledDate = request.ScheduledDate,
                    NumberOfPeople = request.NumberOfPeople,
                    Status = ActivityBookingStatus.Pending,
                    TotalPrice = totalPrice
                };

                _context.ActivityBookings.Add(booking);
                await _context.SaveChangesAsync();

                var bookingDto = new ActivityBookingDto
                {
                    Id = booking.Id,
                    ActivityName = activity.Name,
                    BookingDate = booking.BookingDate,
                    ScheduledDate = booking.ScheduledDate,
                    NumberOfPeople = booking.NumberOfPeople,
                    Status = booking.Status.ToString(),
                    TotalPrice = booking.TotalPrice
                };

                return Ok(new ApiResponse<ActivityBookingDto>
                {
                    Success = true,
                    Data = bookingDto,
                    Message = "Activity booked successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ActivityBookingDto>
                {
                    Success = false,
                    Message = "Error booking activity",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Retrieves all activity bookings for the authenticated guest
        /// </summary>
        /// <returns>List of activity bookings with status and details</returns>
        // GET: api/activities/my-bookings
        [HttpGet("my-bookings")]
        [Authorize(AuthenticationSchemes = "ApiScheme")]
        public async Task<ActionResult<ApiResponse<List<ActivityBookingDto>>>> GetMyBookings()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<List<ActivityBookingDto>>
                    {
                        Success = false,
                        Message = "User not found in token"
                    });
                }

                var bookings = await _context.ActivityBookings
                    .Include(ab => ab.Activity) 
                    .Where(ab => ab.GuestId == userId)
                    .OrderByDescending(ab => ab.BookingDate)
                    .ToListAsync();

                var bookingDtos = bookings.Select(b => new ActivityBookingDto
                {
                    Id = b.Id,
                    ActivityName = b.Activity.Name,
                    BookingDate = b.BookingDate,
                    ScheduledDate = b.ScheduledDate,
                    NumberOfPeople = b.NumberOfPeople,
                    Status = b.Status.ToString(),
                    TotalPrice = b.TotalPrice,
                    ActivityLocation = b.Activity.Schedule, 
                    ActivityDuration = b.Activity.Duration,
                    ActivitySchedule = b.Activity.Schedule,
                    ActivityPrice = b.Activity.Price
                }).ToList();

                return Ok(new ApiResponse<List<ActivityBookingDto>>
                {
                    Success = true,
                    Data = bookingDtos,
                    Message = "Bookings retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<ActivityBookingDto>>
                {
                    Success = false,
                    Message = "Error retrieving bookings",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Cancels an activity booking for the authenticated guest
        /// </summary>
        /// <param name="id">The booking ID to cancel</param>
        /// <returns>Confirmation of cancellation</returns>
        // DELETE: api/activities/bookings/{id}
        [HttpDelete("bookings/{id}")]
        [Authorize(AuthenticationSchemes = "ApiScheme")]
        public async Task<ActionResult<ApiResponse<bool>>> CancelBooking(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "User not found in token"
                    });
                }

                var booking = await _context.ActivityBookings
                    .FirstOrDefaultAsync(ab => ab.Id == id && ab.GuestId == userId);

                if (booking == null)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Booking not found"
                    });
                }

                if (booking.Status == ActivityBookingStatus.Cancelled)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Booking is already cancelled"
                    });
                }

                if (booking.Status == ActivityBookingStatus.Completed)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Cannot cancel a completed booking"
                    });
                }

                booking.Status = ActivityBookingStatus.Cancelled;
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Booking cancelled successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error cancelling booking",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
