using HotelWebApp.Data.Repositories;
using HotelWebApp.Models.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelWebApp.Controllers.Api
{
    /// <summary>
    /// Manages hotel room information for browsing and availability checking
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomRepository _roomRepo;

        public RoomsController(IRoomRepository roomRepo)
        {
            _roomRepo = roomRepo;
        }

        /// <summary>
        /// Searches for available rooms within a specific date range
        /// </summary>
        /// <param name="checkInDate">Desired check-in date</param>
        /// <param name="checkOutDate">Desired check-out date</param>
        /// <returns>List of rooms available for the specified dates</returns>
        // GET: api/rooms/available
        [HttpGet("available")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<RoomDto>>>> GetAvailableRooms(
            [FromQuery] DateTime checkInDate,
            [FromQuery] DateTime checkOutDate)
        {
            try
            {
                if (checkInDate < DateTime.Today)
                {
                    return BadRequest(new ApiResponse<List<RoomDto>>
                    {
                        Success = false,
                        Message = "Check-in date cannot be in the past"
                    });
                }

                if (checkOutDate <= checkInDate)
                {
                    return BadRequest(new ApiResponse<List<RoomDto>>
                    {
                        Success = false,
                        Message = "Check-out date must be after check-in date"
                    });
                }

                var availableRooms = await _roomRepo.GetAvailableRoomsAsync(checkInDate, checkOutDate);

                var roomDtos = availableRooms.Select(r => new RoomDto
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    Type = r.Type.ToString(),
                    PricePerNight = r.PricePerNight,
                    Capacity = r.Capacity,
                    ImageUrl = r.ImageUrl,
                    Status = r.Status.ToString()
                }).ToList();

                return Ok(new ApiResponse<List<RoomDto>>
                {
                    Success = true,
                    Data = roomDtos,
                    Message = $"Found {roomDtos.Count} available rooms"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<RoomDto>>
                {
                    Success = false,
                    Message = "Error retrieving available rooms",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Retrieves detailed information about a specific room
        /// </summary>
        /// <param name="id">The room ID</param>
        /// <returns>Complete room details including type, capacity, and pricing</returns>
        // GET: api/rooms/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<RoomDto>>> GetRoom(int id)
        {
            try
            {
                var room = await _roomRepo.GetByIdAsync(id);
                if (room == null)
                {
                    return NotFound(new ApiResponse<RoomDto>
                    {
                        Success = false,
                        Message = "Room not found"
                    });
                }

                var roomDto = new RoomDto
                {
                    Id = room.Id,
                    RoomNumber = room.RoomNumber,
                    Type = room.Type.ToString(),
                    PricePerNight = room.PricePerNight,
                    Capacity = room.Capacity,
                    ImageUrl = room.ImageUrl,
                    Status = room.Status.ToString()
                };

                return Ok(new ApiResponse<RoomDto>
                {
                    Success = true,
                    Data = roomDto,
                    Message = "Room retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<RoomDto>
                {
                    Success = false,
                    Message = "Error retrieving room",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Retrieves all rooms in the hotel
        /// </summary>
        /// <returns>Complete list of all rooms regardless of availability</returns>
        // GET: api/rooms - Lista todos os quartos
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<RoomDto>>>> GetAllRooms()
        {
            try
            {
                var rooms = await _roomRepo.GetAllAsync();

                var roomDtos = rooms.Select(r => new RoomDto
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    Type = r.Type.ToString(),
                    PricePerNight = r.PricePerNight,
                    Capacity = r.Capacity,
                    ImageUrl = r.ImageUrl,
                    Status = r.Status.ToString()
                }).ToList();

                return Ok(new ApiResponse<List<RoomDto>>
                {
                    Success = true,
                    Data = roomDtos,
                    Message = $"Retrieved {roomDtos.Count} rooms"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<RoomDto>>
                {
                    Success = false,
                    Message = "Error retrieving rooms",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
