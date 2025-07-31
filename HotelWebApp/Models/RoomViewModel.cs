using HotelWebApp.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Models
{
    /// <summary>
    /// ViewModel for the Room Create and Edit pages.
    /// It encapsulates all the properties of a Room that can be managed by an administrator.
    /// </summary>
    public class RoomViewModel
    {
        /// <summary>
        /// The ID of the room. This is used when editing an existing room.
        /// </summary>
        public int Id {  get; set; }

        /// <summary>
        /// The number used to identify the room (e.g., "101").
        /// </summary>
        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Room Number")]
        [MaxLength(10, ErrorMessage = "{0} cannot exceed {1} characters.")]
        public string RoomNumber { get; set; } = string.Empty;

        /// <summary>
        /// The category of the room (e.g., Standard, Suite).
        /// </summary>
        [Required(ErrorMessage = "Please select a room type.")]
        [Display(Name = "Room Type")]
        public RoomType? Type { get; set; }

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
        [Range(0, 1000, ErrorMessage = "{0} must be between {1} and {2}.")]
        public decimal PricePerNight { get; set; }

        /// <summary>
        /// The current operational status of the room (e.g., Available, InMaintenance).
        /// </summary>
        [Required(ErrorMessage = "Please select a room status.")]
        [Display(Name = "Room Status")]
        public RoomStatus? Status { get; set; }

        /// <summary>
        /// The file name of the currently associated image for the room.
        /// This property is used to display the existing image in the Edit view.
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Represents the new image file uploaded by the administrator via the form.
        /// This property will be null if no new file is uploaded.
        /// </summary>
        [Display(Name = "Room Image")]
        public IFormFile? ImageFile { get; set; }
    }
}
