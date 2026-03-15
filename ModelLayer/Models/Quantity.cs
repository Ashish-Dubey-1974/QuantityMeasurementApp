using System;
using ModelLayer.Enums;
using ModelLayer.Helpers;

namespace ModelLayer.Models
{
    /// <summary>
    /// Generic, immutable measurement container for different unit categories
    /// (Length, Weight, Volume, Temperature).
    ///
    /// UC15 refactoring: all conversion calls previously delegated to static
    /// extension classes on the enums are now routed through UnitConverter,
    /// keeping each enum a pure data definition with no embedded behaviour.
    ///
    /// Public API is unchanged — all existing test cases continue to pass.
    /// </summary>
    public sealed class Quantity<TUnit> where TUnit : struct, Enum
    {
        private const double PrecisionLimit = 1e-6;

        public double Value { get; }
        public TUnit Unit  { get; }

        public Quantity(double amount, TUnit unitType)
        {
            if (!double.IsFinite(amount))
                throw new ArgumentException("Numeric value is not valid.");

            Value = amount;
            Unit  = unitType;
        }

        // ── Arithmetic core ───────────────────────────────────────────────────

        private enum OperationType { Add, Subtract, Divide }

        private double ExecuteBaseOperation(Quantity<TUnit> other, OperationType op)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (!Unit.GetType().Equals(other.Unit.GetType()))
                throw new ArgumentException("Measurement types are not compatible.");

            if (typeof(TUnit) == typeof(TemperatureUnit) && op == OperationType.Divide)
                throw new InvalidOperationException("Temperature does not support divide operations.");

            double baseA = UnitConverter.ToBase(Unit,       Value);
            double baseB = UnitConverter.ToBase(other.Unit, other.Value);

            return op switch
            {
                OperationType.Add      => baseA + baseB,
                OperationType.Subtract => baseA - baseB,
                OperationType.Divide when Math.Abs(baseB) < PrecisionLimit
                                       => throw new ArithmeticException("Cannot divide by zero."),
                OperationType.Divide   => baseA / baseB,
                _                      => throw new InvalidOperationException()
            };
        }

        // ── Conversion ────────────────────────────────────────────────────────

        /// <summary>Returns a new Quantity converted to <paramref name="destination"/>.</summary>
        public Quantity<TUnit> ConvertTo(TUnit destination)
        {
            double baseVal   = UnitConverter.ToBase(Unit, Value);
            double converted = UnitConverter.FromBase(destination, baseVal);
            return new Quantity<TUnit>(converted, destination);
        }

        // ── Addition ──────────────────────────────────────────────────────────

        public Quantity<TUnit> Add(Quantity<TUnit> other) => Add(other, Unit);

        public Quantity<TUnit> Add(Quantity<TUnit> other, TUnit? resultUnit = null)
        {
            double baseResult = ExecuteBaseOperation(other, OperationType.Add);
            return CreateFromBase(baseResult, resultUnit ?? Unit);
        }

        // ── Subtraction ───────────────────────────────────────────────────────

        public Quantity<TUnit> Subtract(Quantity<TUnit> other) => Subtract(other, Unit);

        public Quantity<TUnit> Subtract(Quantity<TUnit> other, TUnit? targetUnit = null)
        {
            double baseResult = ExecuteBaseOperation(other, OperationType.Subtract);
            return CreateFromBase(Math.Round(baseResult, 2), targetUnit ?? Unit);
        }

        // ── Division ──────────────────────────────────────────────────────────

        /// <summary>Divides this quantity by <paramref name="other"/> and returns the ratio.</summary>
        public double Divide(Quantity<TUnit> other)
            => ExecuteBaseOperation(other, OperationType.Divide);

        // ── Private helpers ───────────────────────────────────────────────────

        private Quantity<TUnit> CreateFromBase(double baseValue, TUnit destination)
        {
            double converted = UnitConverter.FromBase(destination, baseValue);
            return new Quantity<TUnit>(converted, destination);
        }

        // ── Equality ──────────────────────────────────────────────────────────

        public override bool Equals(object? obj)
        {
            if (obj is not Quantity<TUnit> other) return false;
            return Math.Abs(UnitConverter.ToBase(Unit,       Value)
                           - UnitConverter.ToBase(other.Unit, other.Value)) < PrecisionLimit;
        }

        public override int GetHashCode()
            => UnitConverter.ToBase(Unit, Value).GetHashCode();

        public override string ToString()
            => $"{Value} {UnitConverter.GetSymbol(Unit)}";
    }
}
