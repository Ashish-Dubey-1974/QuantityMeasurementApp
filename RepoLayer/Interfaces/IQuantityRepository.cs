using ModelLayer.Entities;

namespace RepoLayer.Interfaces
{
    /// <summary>
    /// Defines the data-access contract for quantity measurement persistence.
    ///
    /// UC15: Updated to accept <see cref="QuantityMeasurementEntity"/> so that every
    /// operation (convert, compare, add, subtract, divide) can be saved to the
    /// repository as a complete audit record rather than a raw Quantity object.
    ///
    /// Interface Segregation Principle: a single focused interface makes it easy
    /// to swap in a database-backed implementation without touching any other layer.
    /// </summary>
    public interface IQuantityRepository
    {
        /// <summary>Persists a measurement operation entity.</summary>
        void Save(QuantityMeasurementEntity entity);

        /// <summary>Returns all persisted entities in insertion order.</summary>
        IReadOnlyList<QuantityMeasurementEntity> GetAll();
    }
}
