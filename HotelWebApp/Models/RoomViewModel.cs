using HotelWebApp.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Models
{
    public class RoomViewModel
    {

        public int Id {  get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Room Number")]
        [MaxLength(10, ErrorMessage = "{0} cannot exceed {1} characters.")]
        public string RoomNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a room type.")]
        [Display(Name = "Room Type")]
        public RoomType? Type { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Range(1, 10, ErrorMessage = "{0} must be between {1} and {2}.")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Price per Night")]
        [Range(0, 1000, ErrorMessage = "{0} must be between {1} and {2}.")]
        public decimal PricePerNight { get; set; }

        [Required(ErrorMessage = "Please select a room status.")]
        [Display(Name = "Room Status")]
        public RoomStatus? Status { get; set; }

        // Propriedade para mpstrar a imagem atual
        public string? ImageUrl { get; set; }

        // Propriedade para receber o novo ficheiro de imagem
        [Display(Name = "Room Image")]
        public IFormFile? ImageFile { get; set; }

    }
}
