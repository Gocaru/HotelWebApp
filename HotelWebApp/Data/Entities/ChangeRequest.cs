using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Data.Entities
{
    /// <summary>
    /// Represents a request made by a guest to alter an existing reservation.
    /// This allows for an asynchronous communication flow between the guest and hotel staff.
    /// </summary>
    public class ChangeRequest : IEntity
    {
        /// <summary>
        /// The unique identifier for the change request.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The foreign key linking to the reservation that is being requested to change.
        /// </summary>
        public int ReservationId { get; set; }

        /// <summary>
        /// Navigation property to the associated Reservation.
        /// </summary>
        public virtual Reservation Reservation { get; set; }

        /// <summary>
        /// A detailed description of the change requested by the guest.
        /// This can be a combination of a selected service and a free-text message.
        /// </summary>
        public string RequestDetails { get; set; }

        /// <summary>
        /// The date and time when the guest submitted the request.
        /// </summary>
        public DateTime RequestedOn { get; set; }

        /// <summary>
        /// The current status of the request (e.g., Pending, Approved, Rejected).
        /// </summary>
        public RequestStatus Status { get; set; }

        /// <summary>
        /// Optional notes added by the employee who processed the request.
        /// These notes are visible to the guest.
        /// </summary>
        public string? EmployeeNotes { get; set; }

        /// <summary>
        /// The date and time when the request was processed by an employee.
        /// This will be null if the request is still pending.
        /// </summary>
        public DateTime? ProcessedOn { get; set; }

        /// <summary>
        /// The ID of the employee (ApplicationUser) who processed the request.
        /// Used for auditing purposes. This will be null if the request is still pending.
        /// </summary>
        public string? ProcessedByUserId { get; set; }

    }
}
