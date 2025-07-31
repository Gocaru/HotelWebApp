namespace HotelWebApp.Data.Entities
{
    /// <summary>
    /// Defines the possible operational statuses of a hotel room.
    /// </summary>
    public enum RoomStatus
    {
        /// <summary>
        /// The room is clean, ready, and available for a new guest to book or check-in.
        /// </summary>
        Available = 0,

        /// <summary>
        /// A guest is currently staying in the room. It cannot be booked.
        /// </summary>
        Occupied = 1,


        /// <summary>
        /// The room has been booked for a future date but is not yet occupied.
        /// It is not available for booking during the reserved period.
        /// </summary>
        Reserved = 2,

        /// <summary>
        /// The room is temporarily out of service for cleaning, repairs, or other maintenance.
        /// It cannot be booked.
        /// </summary>
        Maintenance = 3
    }
}
