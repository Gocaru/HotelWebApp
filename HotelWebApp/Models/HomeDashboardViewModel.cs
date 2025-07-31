using HotelWebApp.Data.Entities;

namespace HotelWebApp.Models
{
    /// <summary>
    /// ViewModel for the employee dashboard (HomePage).
    /// It aggregates all the key operational data needed for a daily summary.
    /// </summary>
    public class HomeDashboardViewModel
    {
        /// <summary>
        /// A collection of reservations with a check-in date scheduled for today.
        /// </summary>
        public IEnumerable<Reservation> CheckInsToday { get; set; } = new List<Reservation>();

        /// <summary>
        /// A collection of reservations with a check-out date scheduled for today.
        /// </summary>
        public IEnumerable<Reservation> CheckOutsToday { get; set; } = new List<Reservation>();

        /// <summary>
        /// A collection of all reservation change requests that are currently in 'Pending' status
        /// and require action from an employee.
        /// </summary>
        public IEnumerable<ChangeRequest> PendingChangeRequests { get; set; } = new List<ChangeRequest>();
    }
}
