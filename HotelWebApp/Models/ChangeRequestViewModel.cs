using HotelWebApp.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Models
{
    public class ChangeRequestViewModel
    {
        public int ReservationId { get; set; }

        // Para a dropdown de Amenities
        [Display(Name = "Add a Service")]
        public int? SelectedAmenityId { get; set; }

        [Range(1, 10, ErrorMessage = "Quantity must be between 1 and 10.")]
        public int AmenityQuantity { get; set; } = 1; // Valor padrão de 1

        // Para a caixa de texto de pedidos gerais
        [Display(Name = "Other Requests or Details")]
        [MaxLength(500)]
        public string? RequestDetails { get; set; }

        // Para preencher a dropdown na View
        public IEnumerable<SelectListItem>? AvailableAmenities { get; set; }

        public IEnumerable<ChangeRequest> ExistingRequests { get; set; } = new List<ChangeRequest>();

    }
}
