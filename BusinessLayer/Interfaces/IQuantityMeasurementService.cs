using ModelLayer.DTOs;
using ModelLayer.Models;

namespace BusinessLayer.Interfaces
{
    /// <summary>
    /// Service interface for all quantity measurement operations.
    ///
    /// UC15: Expanded from UC14 (which only exposed Compare) to the full set of
    /// DTO-based operations used by the controller layer.  All methods accept
    /// QuantityDTO input and return QuantityDTO output so that higher layers
    /// remain completely decoupled from concrete Quantity&lt;T&gt; generics.
    ///
    /// A second generic set of methods (suffixed "Internal") keeps backward
    /// compatibility with existing unit tests that work directly with Quantity&lt;T&gt;.
    /// </summary>
    public interface IQuantityMeasurementService
    {
        // ── DTO-based API (used by ControllerLayer) ───────────────────────────

        /// <summary>Converts a quantity from its current unit to the target unit.</summary>
        QuantityDTO Convert(QuantityDTO input, string targetUnitName);

        /// <summary>Compares two quantities; Result.Value == 1 means equal, 0 means not equal.</summary>
        QuantityDTO Compare(QuantityDTO first, QuantityDTO second);

        /// <summary>Adds two quantities; returns the sum expressed in the first operand's unit.</summary>
        QuantityDTO Add(QuantityDTO first, QuantityDTO second);

        /// <summary>Subtracts the second quantity from the first.</summary>
        QuantityDTO Subtract(QuantityDTO first, QuantityDTO second);

        /// <summary>Divides the first by the second; Result.Value is the dimensionless ratio.</summary>
        QuantityDTO Divide(QuantityDTO first, QuantityDTO second);

        // ── Generic / backward-compatible API (used by unit tests) ────────────

        bool Compare<U>(Quantity<U> first, Quantity<U> second) where U : struct, Enum;

        Quantity<U> DemonstrateConversion<U>(Quantity<U> source, U target) where U : struct, Enum;

        double DemonstrateConversion<U>(double value, U sourceUnit, U targetUnit) where U : struct, Enum;

        Quantity<U> DemonstrateAddition<U>(Quantity<U> a, Quantity<U> b) where U : struct, Enum;

        Quantity<U> DemonstrateAddition<U>(Quantity<U> a, Quantity<U> b, U resultUnit) where U : struct, Enum;

        Quantity<U> Subtract<U>(Quantity<U> a, Quantity<U> b, U resultUnit) where U : struct, Enum;

        double Divide<U>(double valA, U unitA, double valB, U unitB) where U : struct, Enum;
    }
}
