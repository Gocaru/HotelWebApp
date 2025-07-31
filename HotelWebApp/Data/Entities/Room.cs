using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelWebApp.Data.Entities
{
    /// <summary>
    /// Represents a physical room in the hotel that can be booked.
    /// </summary>
    public class Room : IEntity
    {
        /// <summary>
        /// The unique identifier for the room.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The number used to identify the room (e.g., "101", "205A").
        /// </summary>
        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Room Number")]
        [MaxLength(10, ErrorMessage ="{0} cannot exceed {1} characters.")]
        public string RoomNumber { get; set; }

        /// <summary>
        /// The category or type of the room (e.g., Standard, Suite, Luxury).
        /// </summary>
        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Room Type")]
        public RoomType Type { get; set; }

        /// <summary>
        /// The maximum number of guests the room can accommodate.
        /// </summary>
        [Required(ErrorMessage = "{0} is required")]
        [Range(1, 10, ErrorMessage = "{0} must be between {1} and {2}.")]
        public int Capacity { get; set; }

        /// <summary>
        /// The base price for booking this room for one night.
        /// </summary>
        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Price per Night")]
        [Range(0.01, 10000, ErrorMessage = "{0} must be between {1} and {2}.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePerNight { get; set; }

        /// <summary>
        /// The current operational status of the room (e.g., Available, Occupied, InMaintenance).
        /// </summary>
        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Room Status")]
        public RoomStatus Status { get; set; }

        /// <summary>
        /// The file name of the room's representative image.
        /// The file is stored in the wwwroot/images/rooms directory. Can be null.
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Navigation property to the collection of all reservations associated with this room.
        /// </summary>
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
