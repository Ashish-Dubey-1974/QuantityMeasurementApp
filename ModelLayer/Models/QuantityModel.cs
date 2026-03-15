using System;

namespace ModelLayer.Models
{
    /// <summary>
    /// A generic POJO (Plain Old CLR Object) used internally within the service layer
    /// to represent a quantity alongside its typed unit of measurement.
    ///
    /// Unlike <see cref="Quantity{TUnit}"/> (which contains arithmetic behaviour),
    /// QuantityModel is a pure data carrier — it has no logic of its own.
    /// The service layer constructs QuantityModel objects from incoming QuantityDTO data,
    /// uses them for type-safe operations, and converts results back to QuantityDTO
    /// before returning to the caller.
    ///
    /// Generic constraint: T must be one of LengthUnit, WeightUnit, VolumeUnit,
    /// or TemperatureUnit — any enum that represents a measurable unit.
    /// </summary>
    public class QuantityModel<T> where T : struct, Enum
    {
        /// <summary>The numeric value of this quantity.</summary>
        public double Value { get; }

        /// <summary>The typed unit associated with this value.</summary>
        public T Unit { get; }

        public QuantityModel(double value, T unit)
        {
            Value = value;
            Unit  = unit;
        }

        public override string ToString() => $"{Value} {Unit}";
    }
}
