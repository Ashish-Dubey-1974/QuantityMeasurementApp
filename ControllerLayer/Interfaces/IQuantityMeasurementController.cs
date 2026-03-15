using ModelLayer.DTOs;

namespace ControllerLayer.Interfaces
{
    /// <summary>
    /// Contract for the controller layer.
    ///
    /// The controller sits between the presentation layer (Menu / REST endpoints)
    /// and the business layer (Service).  It accepts QuantityDTO input, delegates
    /// to the service, and returns formatted QuantityDTO results.
    ///
    /// UC15 Facade Pattern: the controller hides service complexity behind simple
    /// perform-style methods that map naturally to REST verbs in a future web app.
    /// </summary>
    public interface IQuantityMeasurementController
    {
        /// <summary>Converts a quantity to the requested target unit.</summary>
        QuantityDTO PerformConversion(QuantityDTO input, string targetUnitName);

        /// <summary>Compares two quantities of the same category.</summary>
        QuantityDTO PerformComparison(QuantityDTO first, QuantityDTO second);

        /// <summary>Adds two quantities and returns the sum.</summary>
        QuantityDTO PerformAddition(QuantityDTO first, QuantityDTO second);

        /// <summary>Subtracts the second quantity from the first.</summary>
        QuantityDTO PerformSubtraction(QuantityDTO first, QuantityDTO second);

        /// <summary>Divides the first quantity by the second, returning the dimensionless ratio.</summary>
        QuantityDTO PerformDivision(QuantityDTO first, QuantityDTO second);
    }
}
