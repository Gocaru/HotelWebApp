using HotelWebApp.Data.Entities;

namespace HotelWebApp.Data.Repositories
{
    /// <summary>
    /// Defines the contract for the room repository, specifying the data access operations
    /// available for the Room entity.
    /// </summary>
    public interface IRoomRepository
    {
        /// <summary>
        /// Asynchronously retrieves a list of all rooms from the data store.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of all rooms.</returns>
        Task<List<Room>> GetAllAsync();


        /// <summary>
        /// Asynchronously retrieves a single room by its unique identifier.
        /// </summary>
        /// <param name="id">The primary key of the room.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the Room entity, or null if not found.</returns>
        Task<Room?> GetByIdAsync(int id);

        /// <summary>
        /// Asynchronously adds a new room to the data store.
        /// </summary>
        /// <param name="room">The Room entity to create.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task AddAsync(Room room);

        /// <summary>
        /// Asynchronously updates an existing room in the data store.
        /// </summary>
        /// <param name="room">The Room entity with updated values.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task UpdateAsync(Room room);

        /// <summary>
        /// Asynchronously deletes a room from the data store by its ID.
        /// </summary>
        /// <param name="id">The ID of the room to delete.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// Asynchronously checks if a room with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the room to check.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is true if the room exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(int id);


        /// <summary>
        /// Asynchronously retrieves a collection of rooms that are available for booking within a specific date range.
        /// This method checks for reservation overlaps and rooms that are out of service.
        /// </summary>
        /// <param name="checkIn">The desired check-in date.</param>
        /// <param name="checkOut">The desired check-out date.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of available rooms.</returns>
        Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut);

        Task<bool> RoomNumberExistsAsync(string roomNumber, int? excludeId = null);
    }
}
