namespace HotelWebApp.Models
{
    /// <summary>
    /// ViewModel representing a single event to be displayed on the Syncfusion Scheduler.
    /// This class acts as a Data Transfer Object (DTO), mapping properties from the
    /// Reservation entity to the fields expected by the scheduler component.
    /// </summary>
    public class SchedulerEventViewModel
    {
        /// <summary>
        // / Binds to the 'Id' field of the scheduler event. Corresponds to the Reservation ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Binds to the 'Subject' field. This is the main text displayed on the event block (e.g., the guest's name).
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Binds to the 'StartTime' field. Corresponds to the reservation's CheckInDate.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Binds to the 'EndTime' field. Corresponds to the reservation's CheckOutDate.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Binds to the 'IsAllDay' field. For hotel reservations, this is typically false.
        /// </summary>
        public bool IsAllDay { get; set; }

        /// <summary>
        /// Binds to the 'Location' field, displayed in the event's tooltip or editor.
        /// In this system, it can represent the room number and type.
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// Binds to the 'Description' field, providing extra details in the tooltip.
        /// In this system, it shows the status and number of guests.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Binds to the 'RecurrenceRule' field. Not used in this application but included for completeness.
        /// </summary>
        public string? RecurrenceRule { get; set; }

        /// <summary>
        // / A custom field used to dynamically set the background color of the event.
        /// This is bound using the 'colorField' property in the scheduler's eventSettings.
        /// </summary>
        public string? CategoryColor { get; set; }

        /// <summary>
        /// Binds to the resource ID ('resourceId' field). This is the foreign key that links the
        /// event to a specific resource (in this case, a Room). Crucial for timeline/group views.
        /// </summary>
        public int RoomId { get; set; }

    }
}
