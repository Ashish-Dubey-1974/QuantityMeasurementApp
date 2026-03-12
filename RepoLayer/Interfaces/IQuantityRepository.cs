using ModelLayer.Models;

namespace RepoLayer.Interfaces
{
    /// <summary>
    /// Defines operations for storing quantities.
    /// </summary>
    public interface IQuantityRepository
    {
        /// <summary>
        /// Saves a quantity instance.
        /// </summary>
        Quantity<T> Save<T>(Quantity<T> quantity) where T : struct, Enum;
    }
}