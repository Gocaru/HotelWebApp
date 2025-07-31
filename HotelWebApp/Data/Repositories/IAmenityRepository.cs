using HotelWebApp.Data.Entities;

namespace HotelWebApp.Data.Repositories
{
    /// <summary>
    /// Defines the contract for the amenity repository, specifying the data access operations
    /// available for the Amenity entity.
    /// </summary>
    public interface IAmenityRepository
    {
        /// <summary>
        /// Asynchronously retrieves all amenities from the data store.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all amenities.</returns>
        Task<IEnumerable<Amenity>> GetAllAsync();

        /// <summary>
        /// Asynchronously retrieves a single amenity by its unique identifier.
        /// </summary>
        /// <param name="id">The primary key of the amenity.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the Amenity entity, or null if not found.</returns>
        Task<Amenity?> GetByIdAsync(int id);

        /// <summary>
        /// Asynchronously adds a new amenity to the data store.
        /// </summary>
        /// <param name="amenity">The Amenity entity to create.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task CreateAsync(Amenity amenity);

        /// <summary>
        /// Asynchronously updates an existing amenity in the data store.
        /// </summary>
        /// <param name="amenity">The Amenity entity with updated values.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task UpdateAsync(Amenity amenity);

        /// <summary>
        /// Asynchronously deletes an amenity from the data store.
        /// </summary>
        /// <param name="amenity">The Amenity entity to delete.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task DeleteAsync(Amenity amenity);

        /// <summary>
        /// Asynchronously checks if an amenity with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the amenity to check.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is true if the amenity exists; otherwise, false.</returns>
        Task<bool> AmenityExistsAsync(int id);

        /// <summary>
        /// Asynchronously checks if an amenity is currently being used in any reservation.
        /// </summary>
        /// <param name="id">The ID of the amenity to check.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is true if the amenity is in use; otherwise, false.</returns>
        Task<bool> IsInUseAsync(int id);
    }
}
