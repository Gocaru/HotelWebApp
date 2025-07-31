namespace HotelWebApp.Data.Entities
{
    /// <summary>
    /// Defines the possible statuses for a guest's change request.
    /// </summary>
    public enum RequestStatus
    {
        /// <summary>
        /// The guest has submitted the request, and it is awaiting review by an employee.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// An employee has reviewed and approved the request.
        /// Any necessary changes to the reservation have been (or should be) made.
        /// </summary>
        Approved = 1,

        /// <summary>
        /// An employee has reviewed and rejected the request.
        /// No changes were made to the reservation.
        /// </summary>
        Rejected = 2
    }
}
