using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Data.Entities
{
    public class ChangeRequest : IEntity
    {
        public int Id { get; set; }

        public int ReservationId { get; set; }
        
        public virtual Reservation Reservation { get; set; }

        public string RequestDetails { get; set; }

        public DateTime RequestedOn { get; set; }

        public RequestStatus Status { get; set; } 

        public string? EmployeeNotes { get; set; } 
        
        public DateTime? ProcessedOn { get; set; }

        public string? ProcessedByUserId { get; set; }

    }
}
