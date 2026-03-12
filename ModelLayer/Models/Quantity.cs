using System;
using ModelLayer.Enums;

namespace ModelLayer.Models
{
    /// <summary>
    /// Generic measurement container used for different unit groups
    /// like length, weight, volume and temperature.
    /// Internally calculations are done using the base unit.
    /// </summary>
    public sealed class Quantity<TUnit> where TUnit : struct, Enum
    {
        private const double PrecisionLimit = 1e-6;

        public double Value { get; }
        public TUnit Unit { get; }

        public Quantity(double amount, TUnit unitType)
        {
            if (!double.IsFinite(amount))
                throw new ArgumentException("Numeric value is not valid.");

            Value = amount;
            Unit = unitType;
        }

        private enum OperationType
        {
            Add,
            Subtract,
            Divide
        }

        private double ExecuteBaseOperation(Quantity<TUnit> otherQuantity, OperationType op)
        {
            if (otherQuantity == null)
                throw new ArgumentNullException(nameof(otherQuantity));

            if (!Unit.GetType().Equals(otherQuantity.Unit.GetType()))
                throw new ArgumentException("Measurement types are not compatible.");

            if (typeof(TUnit) == typeof(TemperatureUnit) && op == OperationType.Divide)
                throw new InvalidOperationException($"Temperature does not support {op.ToString().ToLower()} operations.");

            double baseA = ConvertCurrentToBase();
            double baseB = otherQuantity.ConvertCurrentToBase();

            switch (op)
            {
                case OperationType.Add:
                    return baseA + baseB;

                case OperationType.Subtract:
                    return baseA - baseB;

                case OperationType.Divide:
                    if (Math.Abs(baseB) < PrecisionLimit)
                        throw new ArithmeticException("Cannot divide by zero.");
                    return baseA / baseB;

                default:
                    throw new InvalidOperationException();
            }
        }

        private double ConvertCurrentToBase()
        {
            if (Unit is LengthUnit l) return l.ConvertToBase(Value);
            if (Unit is WeightUnit w) return w.ConvertToBase(Value);
            if (Unit is VolumeUnit v) return v.ConvertToBase(Value);
            if (Unit is TemperatureUnit t) return t.ConvertToBase(Value);

            throw new InvalidOperationException("Unsupported unit category.");
        }

        public Quantity<TUnit> ConvertTo(TUnit destinationUnit)
        {
            double baseVal = ConvertCurrentToBase();
            double newValue = 0;

            if (destinationUnit is LengthUnit l)
                newValue = LengthUnitExtensions.ConvertFromBase(l, baseVal);
            else if (destinationUnit is WeightUnit w)
                newValue = WeightUnitExtension.ConvertFromBase(w, baseVal);
            else if (destinationUnit is VolumeUnit v)
                newValue = VolumeUnitExtension.ConvertFromBase(v, baseVal);
            else if (destinationUnit is TemperatureUnit t)
                newValue = TemperatureUnitExtension.ConvertFromBase(t, baseVal);

            return new Quantity<TUnit>(newValue, destinationUnit);
        }

        public Quantity<TUnit> Subtract(Quantity<TUnit> other)
        {
            return Subtract(other, Unit);
        }

        public Quantity<TUnit> Subtract(Quantity<TUnit> other, TUnit? targetUnit = null)
        {
            double baseResult = ExecuteBaseOperation(other, OperationType.Subtract);
            return CreateFromBase(Math.Round(baseResult, 2), targetUnit ?? Unit);
        }

        public double Divide(Quantity<TUnit> other)
        {
            return ExecuteBaseOperation(other, OperationType.Divide);
        }

        public Quantity<TUnit> Add(Quantity<TUnit> other)
        {
            return Add(other, Unit);
        }

        public Quantity<TUnit> Add(Quantity<TUnit> other, TUnit? resultUnit = null)
        {
            double baseResult = ExecuteBaseOperation(other, OperationType.Add);
            return CreateFromBase(baseResult, resultUnit ?? Unit);
        }

        private Quantity<TUnit> CreateFromBase(double baseValue, TUnit destination)
        {
            double convertedVal = 0;

            if (destination is LengthUnit l)
                convertedVal = l.ConvertFromBase(baseValue);
            else if (destination is WeightUnit w)
                convertedVal = WeightUnitExtension.ConvertFromBase(w, baseValue);
            else if (destination is VolumeUnit v)
                convertedVal = VolumeUnitExtension.ConvertFromBase(v, baseValue);
            else if (destination is TemperatureUnit t)
                convertedVal = TemperatureUnitExtension.ConvertFromBase(t, baseValue);

            return new Quantity<TUnit>(convertedVal, destination);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Quantity<TUnit> otherQuantity)
                return false;

            return Math.Abs(ConvertCurrentToBase() - otherQuantity.ConvertCurrentToBase()) < PrecisionLimit;
        }

        public override int GetHashCode()
        {
            return ConvertCurrentToBase().GetHashCode();
        }

        public override string ToString()
        {
            string symbol = "";

            if (Unit is LengthUnit l) symbol = l.GetSymbol();
            else if (Unit is WeightUnit w) symbol = w.GetSymbol();
            else if (Unit is VolumeUnit v) symbol = v.GetSymbol();
            else if (Unit is TemperatureUnit t) symbol = t.GetSymbol();

            return $"{Value} {symbol}";
        }
    }
}