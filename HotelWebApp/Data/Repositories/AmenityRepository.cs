using HotelWebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Data.Repositories
{
    /// <summary>
    /// Repository for performing CRUD operations on the Amenity entity.
    /// It abstracts the data access logic from the rest of the application.
    /// </summary>
    public class AmenityRepository : IAmenityRepository
    {
        private readonly HotelWebAppContext _context;   

        public AmenityRepository(HotelWebAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all amenities from the database, ordered by name.
        /// </summary>
        /// <returns>A collection of all Amenity entities.</returns>
        public async Task<IEnumerable<Amenity>> GetAllAsync()
        {
            return await _context.Amenities.OrderBy(a => a.Name).ToListAsync();
        }

        /// <summary>
        /// Retrieves a single amenity by its unique identifier.
        /// </summary>
        /// <param name="id">The primary key of the amenity.</param>
        /// <returns>The Amenity entity, or null if not found.</returns>
        public async Task<Amenity?> GetByIdAsync(int id)
        {
            return await _context.Amenities.FindAsync(id); ;
        }

        /// <summary>
        /// Adds a new amenity to the database.
        /// </summary>
        /// <param name="amenity">The Amenity entity to add.</param>
        public async Task CreateAsync(Amenity amenity)
        {
            await _context.Amenities.AddAsync(amenity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing amenity in the database.
        /// </summary>
        /// <param name="amenity">The Amenity entity with updated values.</param>
        public async Task UpdateAsync(Amenity amenity)
        {
            _context.Amenities.Update(amenity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes an amenity from the database.
        /// </summary>
        /// <param name="amenity">The Amenity entity to delete.</param>
        public async Task DeleteAsync(Amenity amenity)
        {
            _context.Amenities.Remove(amenity);
            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Checks if an amenity with the specified ID exists in the database.
        /// </summary>
        /// <param name="id">The ID of the amenity to check.</param>
        /// <returns>True if the amenity exists, otherwise false.</returns>
        public async Task<bool> AmenityExistsAsync(int id)
        {
            return await _context.Amenities.AnyAsync(e => e.Id == id);
        }

        /// <summary>
        /// Checks if an amenity is currently associated with any existing reservations.
        /// This is used to prevent the deletion of in-use amenities.
        /// </summary>
        /// <param name="id">The ID of the amenity to check.</param>
        /// <returns>True if the amenity is in use, otherwise false.</returns>
        public async Task<bool> IsInUseAsync(int id)
        {
            return await _context.ReservationAmenities.AnyAsync(ra => ra.AmenityId == id);
        }

    }
}
