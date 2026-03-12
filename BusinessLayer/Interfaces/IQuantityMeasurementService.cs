using ModelLayer.Models;

namespace BusinessLayer.Interfaces
{
    /// <summary>
    /// Defines operations for comparing quantities.
    /// </summary>
    public interface IQuantityMeasurementService
    {
        /// <summary>
        /// Checks whether two quantities represent the same value.
        /// </summary>
        bool Compare<U>(Quantity<U> first, Quantity<U> second) where U : struct, Enum;
    }
}