namespace HotelWebApp.Data.Entities
{
    /// <summary>
    /// Defines a common contract for all entities in the data model.
    /// By implementing this interface, entities are guaranteed to have a primary key property named 'Id'.
    /// This is useful for creating generic repositories and services that can operate on any entity type.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// This property serves as the primary key in the database.
        /// </summary>
        public int Id { get; set; }
    }
}
