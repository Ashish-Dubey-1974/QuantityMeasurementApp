using System;
using ModelLayer.Helpers;

namespace ModelLayer.Models
{
    public sealed class Quantity<TUnit> where TUnit : struct, Enum
    {
        public double Value { get; }
        public TUnit  Unit  { get; }

        public Quantity(double value, TUnit unit)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Numeric value is not valid.");
            Value = value;
            Unit  = unit;
        }

        public override string ToString()
            => $"{Value} {UnitConverter.GetSymbol(Unit)}";
    }
}
