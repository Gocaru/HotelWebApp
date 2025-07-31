using HotelWebApp.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Models
{
    /// <summary>
    /// ViewModel for the 'Request Change' page, used by guests.
    /// It contains properties for both submitting a new change request and
    /// displaying the history of previous requests for the same reservation.
    /// </summary>
    public class ChangeRequestViewModel
    {
        /// <summary>
        /// The ID of the reservation to which this change request is linked.
        /// This is typically passed as a hidden field in the form.
        /// </summary>
        public int ReservationId { get; set; }

        /// <summary>
        /// Binds to the selected amenity from the dropdown list.
        /// This is nullable to allow the guest to submit a request without adding a service.
        /// </summary>
        [Display(Name = "Add a Service")]
        public int? SelectedAmenityId { get; set; }

        /// <summary>
        /// Binds to the quantity of the selected amenity. Defaults to 1.
        /// </summary>
        [Range(1, 10, ErrorMessage = "Quantity must be between 1 and 10.")]
        public int AmenityQuantity { get; set; } = 1;

        /// <summary>
        /// Binds to the textarea for any free-text requests from the guest (e.g., date changes).
        /// </summary>
        [Display(Name = "Other Requests or Details")]
        [MaxLength(500)]
        public string? RequestDetails { get; set; }

        /// <summary>
        /// A collection of SelectListItem used to populate the amenities dropdown in the view.
        /// This is prepared by the controller.
        /// </summary>
        public IEnumerable<SelectListItem>? AvailableAmenities { get; set; }

        /// <summary>
        /// A collection of all previous change requests for this reservation.
        /// Used to display the request history on the same page.
        /// </summary>
        public IEnumerable<ChangeRequest> ExistingRequests { get; set; } = new List<ChangeRequest>();

    }
}
