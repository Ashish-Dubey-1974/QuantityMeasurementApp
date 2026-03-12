using RepoLayer.Interfaces;
using ModelLayer.Models;

namespace RepoLayer.Repositories
{
    /// <summary>
    /// Provides a repository implementation for quantity storage operations.
    /// </summary>
    public class QuantityRepository : IQuantityRepository
    {
        /// <summary>
        /// Saves the given quantity instance.
        /// </summary>
        public Quantity<T> Save<T>(Quantity<T> quantity) where T : struct, Enum
        {
            return quantity;
        }
    }
}