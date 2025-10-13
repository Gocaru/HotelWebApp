using HotelWebApp.Data.Entities;
using HotelWebApp.Data.Repositories;
using HotelWebApp.Models.Api;
using HotelWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelWebApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "ApiScheme")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationRepository _reservationRepo;
        private readonly IRoomRepository _roomRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IReservationService _reservationService;

        public ReservationsController(
            IReservationRepository reservationRepo,
            IRoomRepository roomRepo,
            UserManager<ApplicationUser> userManager,
            IReservationService reservationService)
        {
            _reservationRepo = reservationRepo;
            _roomRepo = roomRepo;
            _userManager = userManager;
            _reservationService = reservationService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ReservationDto>>>> GetMyReservations()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<List<ReservationDto>>
                    {
                        Success = false,
                        Message = "User not found in token"
                    });
                }

                var reservations = await _reservationRepo.GetReservationsByGuestIdWithDetailsAsync(userId);

                var reservationDtos = reservations.Select(r => new ReservationDto
                {
                    Id = r.Id,
                    CheckInDate = r.CheckInDate,
                    CheckOutDate = r.CheckOutDate,
                    ReservationDate = r.ReservationDate,
                    TotalPrice = r.TotalPrice,
                    Status = r.Status.ToString(),
                    NumberOfGuests = r.NumberOfGuests,
                    OriginalPrice = r.OriginalPrice,
                    DiscountPercentage = r.DiscountPercentage,
                    PromotionTitle = r.Promotion?.Title,
                    Room = r.Room != null ? new RoomDto
                    {
                        Id = r.Room.Id,
                        RoomNumber = r.Room.RoomNumber,
                        Type = r.Room.Type.ToString(),
                        PricePerNight = r.Room.PricePerNight,
                        Capacity = r.Room.Capacity,
                        ImageUrl = r.Room.ImageUrl
                    } : null,
                    Amenities = r.ReservationAmenities?.Select(ra => new ReservationAmenityDto
                    {
                        AmenityName = ra.Amenity?.Name ?? string.Empty,
                        Quantity = ra.Quantity,
                        Price = ra.Amenity?.Price ?? 0
                    }).ToList() ?? new List<ReservationAmenityDto>()
                }).OrderByDescending(r => r.ReservationDate).ToList();

                return Ok(new ApiResponse<List<ReservationDto>>
                {
                    Success = true,
                    Data = reservationDtos,
                    Message = "Reservations retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<ReservationDto>>
                {
                    Success = false,
                    Message = "Error retrieving reservations",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ReservationDto>>> GetReservation(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<ReservationDto>
                    {
                        Success = false,
                        Message = "User not found in token"
                    });
                }

                var reservation = await _reservationRepo.GetByIdWithDetailsAsync(id);
                if (reservation == null)
                {
                    return NotFound(new ApiResponse<ReservationDto>
                    {
                        Success = false,
                        Message = "Reservation not found"
                    });
                }

                if (reservation.GuestId != userId)
                {
                    return Forbid();
                }

                var reservationDto = new ReservationDto
                {
                    Id = reservation.Id,
                    CheckInDate = reservation.CheckInDate,
                    CheckOutDate = reservation.CheckOutDate,
                    ReservationDate = reservation.ReservationDate,
                    TotalPrice = reservation.TotalPrice,
                    Status = reservation.Status.ToString(),
                    NumberOfGuests = reservation.NumberOfGuests,
                    OriginalPrice = reservation.OriginalPrice,
                    DiscountPercentage = reservation.DiscountPercentage,
                    PromotionTitle = reservation.Promotion?.Title,
                    Room = reservation.Room != null ? new RoomDto
                    {
                        Id = reservation.Room.Id,
                        RoomNumber = reservation.Room.RoomNumber,
                        Type = reservation.Room.Type.ToString(),
                        PricePerNight = reservation.Room.PricePerNight,
                        Capacity = reservation.Room.Capacity,
                        ImageUrl = reservation.Room.ImageUrl
                    } : null,
                    Amenities = reservation.ReservationAmenities?.Select(ra => new ReservationAmenityDto
                    {
                        AmenityName = ra.Amenity?.Name ?? string.Empty,
                        Quantity = ra.Quantity,
                        Price = ra.Amenity?.Price ?? 0
                    }).ToList() ?? new List<ReservationAmenityDto>()
                };

                return Ok(new ApiResponse<ReservationDto>
                {
                    Success = true,
                    Data = reservationDto,
                    Message = "Reservation retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ReservationDto>
                {
                    Success = false,
                    Message = "Error retrieving reservation",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<ApiResponse<bool>>> CancelReservation(int id)
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

                var reservation = await _reservationRepo.GetByIdAsync(id);
                if (reservation == null)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Reservation not found"
                    });
                }

                if (reservation.GuestId != userId)
                {
                    return Forbid();
                }

                if (reservation.Status != ReservationStatus.Confirmed)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Only confirmed reservations can be cancelled"
                    });
                }

                if (reservation.CheckInDate <= DateTime.Today)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Cannot cancel reservations that have already started or are for today"
                    });
                }

                reservation.Status = ReservationStatus.Cancelled;
                await _reservationRepo.UpdateAsync(reservation);

                var room = await _roomRepo.GetByIdAsync(reservation.RoomId);
                if (room != null)
                {
                    room.Status = RoomStatus.Available;
                    await _roomRepo.UpdateAsync(room);
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Reservation cancelled successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error cancelling reservation",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("{id}/check-in")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckIn(int id)
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

                var reservation = await _reservationRepo.GetByIdWithDetailsAsync(id);
                if (reservation == null)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Reservation not found"
                    });
                }

                if (reservation.GuestId != userId)
                {
                    return Forbid();
                }

                if (reservation.Status != ReservationStatus.Confirmed)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Only confirmed reservations can be checked in"
                    });
                }

                if (reservation.CheckInDate.Date != DateTime.Today.Date)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Check-in is only available on the reservation date"
                    });
                }

                reservation.Status = ReservationStatus.CheckedIn;
                if (reservation.Room != null)
                {
                    reservation.Room.Status = RoomStatus.Occupied;
                }

                await _reservationRepo.UpdateAsync(reservation);

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Check-in completed successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error during check-in",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ReservationDto>>> CreateReservation([FromBody] CreateReservationRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<ReservationDto>
                    {
                        Success = false,
                        Message = "User not found in token"
                    });
                }

                if (request.CheckInDate < DateTime.Today)
                {
                    return BadRequest(new ApiResponse<ReservationDto>
                    {
                        Success = false,
                        Message = "Check-in date cannot be in the past"
                    });
                }

                if (request.CheckOutDate <= request.CheckInDate)
                {
                    return BadRequest(new ApiResponse<ReservationDto>
                    {
                        Success = false,
                        Message = "Check-out date must be after check-in date"
                    });
                }

                var checkInWithTime = new DateTime(request.CheckInDate.Year, request.CheckInDate.Month, request.CheckInDate.Day, 12, 0, 0);
                var checkOutWithTime = new DateTime(request.CheckOutDate.Year, request.CheckOutDate.Month, request.CheckOutDate.Day, 11, 0, 0);

                var isAvailable = await _reservationRepo.IsRoomAvailableAsync(
                    request.RoomId,
                    checkInWithTime,
                    checkOutWithTime
                );

                if (!isAvailable)
                {
                    return BadRequest(new ApiResponse<ReservationDto>
                    {
                        Success = false,
                        Message = "Room is not available for the selected dates"
                    });
                }

                var room = await _roomRepo.GetByIdAsync(request.RoomId);
                if (room == null)
                {
                    return NotFound(new ApiResponse<ReservationDto>
                    {
                        Success = false,
                        Message = "Room not found"
                    });
                }

                if (request.NumberOfGuests > room.Capacity)
                {
                    return BadRequest(new ApiResponse<ReservationDto>
                    {
                        Success = false,
                        Message = $"Room capacity is {room.Capacity} guests"
                    });
                }

                var viewModel = new HotelWebApp.Models.ReservationViewModel
                {
                    GuestId = userId,
                    RoomId = request.RoomId,
                    CheckInDate = request.CheckInDate,
                    CheckOutDate = request.CheckOutDate,
                    NumberOfGuests = request.NumberOfGuests
                };

                var result = await _reservationService.CreateReservationAsync(viewModel, userId);

                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<ReservationDto>
                    {
                        Success = false,
                        Message = result.Error
                    });
                }

                var userReservations = await _reservationRepo.GetReservationsByGuestIdWithDetailsAsync(userId);
                var reservation = userReservations
                    .OrderByDescending(r => r.ReservationDate)
                    .FirstOrDefault();

                if (reservation == null)
                {
                    return StatusCode(500, new ApiResponse<ReservationDto>
                    {
                        Success = false,
                        Message = "Reservation created but could not retrieve details"
                    });
                }

                var reservationDto = new ReservationDto
                {
                    Id = reservation.Id,
                    CheckInDate = reservation.CheckInDate,
                    CheckOutDate = reservation.CheckOutDate,
                    ReservationDate = reservation.ReservationDate,
                    TotalPrice = reservation.TotalPrice,
                    Status = reservation.Status.ToString(),
                    NumberOfGuests = reservation.NumberOfGuests,
                    Room = new RoomDto
                    {
                        Id = reservation.Room.Id,
                        RoomNumber = reservation.Room.RoomNumber,
                        Type = reservation.Room.Type.ToString(),
                        PricePerNight = reservation.Room.PricePerNight,
                        Capacity = reservation.Room.Capacity,
                        ImageUrl = reservation.Room.ImageUrl
                    },
                    Amenities = new List<ReservationAmenityDto>()
                };

                return Ok(new ApiResponse<ReservationDto>
                {
                    Success = true,
                    Data = reservationDto,
                    Message = "Reservation created successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ReservationDto>
                {
                    Success = false,
                    Message = "Error creating reservation",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}