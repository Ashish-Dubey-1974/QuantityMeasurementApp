using System;
using QuantityMeasurementApp.Model;
using QuantityMeasurementApp.Business.Interfaces;
namespace QuantityMeasurementApp.Business.Services
{
    public class Quantity<U> where U : struct
    {
        private readonly double value;
        private readonly U unit;
        private const double EPSILON = 0.000001;
        public double Value => value;
        public U Unit => unit;

        private enum ArithmeticOperation
        {
            ADD,
            SUBTRACT,
            DIVIDE
        }
        private void ValidateArithmeticOperands(Quantity<U> other, U? targetUnit, bool targetUnitRequired)
        {
            if (other == null)
                throw new ArgumentException("Operand cannot be null");

            if (targetUnitRequired && targetUnit == null)
                throw new ArgumentException("Target unit cannot be null");

            if (this.Unit.GetType() != other.Unit.GetType())
                throw new ArgumentException("Quantities must belong to the same measurement category");

            if (double.IsNaN(this.Value) || double.IsInfinity(this.Value) ||
                double.IsNaN(other.Value) || double.IsInfinity(other.Value))
                throw new ArgumentException("Values must be finite numbers");
        }

        private double PerformBaseArithmetic(Quantity<U> other, ArithmeticOperation operation)
        {
            dynamic u1 = unit;
            dynamic u2 = other.unit;
            double base1 = u1.ConvertToBaseUnit(Value);
            double base2 = u2.ConvertToBaseUnit(other.Value);

            return operation switch
            {
                ArithmeticOperation.ADD => base1 + base2,
                ArithmeticOperation.SUBTRACT => base1 - base2,
                ArithmeticOperation.DIVIDE => base2 == 0
                    ? throw new ArithmeticException("Division by zero")
                    : base1 / base2,
                _ => throw new InvalidOperationException("Unsupported operation")
            };
        }

        public Quantity(double value, U unit)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Invalid value");

            this.value = value;
            this.unit = unit;
        }
        public Quantity<U> Subtract(Quantity<U> other)
        {
            return Subtract(other, unit);
        }

        public Quantity<U> Subtract(Quantity<U> other, U targetUnit)
        {
            ValidateArithmeticOperands(other, targetUnit, true);

            double baseResult = PerformBaseArithmetic(other, ArithmeticOperation.SUBTRACT);

            dynamic t = targetUnit;

            double converted = t.ConvertFromBaseUnit(baseResult);

            converted = Math.Round(converted, 2);

            return new Quantity<U>(converted, targetUnit);
        }
        // Divide method to perform division between two quantities of the same unit type, returning a double result
        public double Divide(Quantity<U> other)
        {
            ValidateArithmeticOperands(other, default, false);

            return PerformBaseArithmetic(other, ArithmeticOperation.DIVIDE);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is Quantity<U>))
                return false;

            Quantity<U> other = (Quantity<U>)obj;

            double base1 = ConvertToBase(this.value, this.unit);
            double base2 = ConvertToBase(other.value, other.unit);

            return Math.Abs(base1 - base2) < EPSILON;
        }

        private double ConvertToBase(double value, U unit)
        {
            dynamic u = unit;
            return u.ConvertToBaseUnit(value);
        }

        public Quantity<U> ConvertTo(U targetUnit)
        {
            double baseValue = ConvertToBase(this.value, this.unit);

            dynamic t = targetUnit;
            double converted = t.ConvertFromBaseUnit(baseValue);

            return new Quantity<U>(converted, targetUnit);
        }



        public Quantity<U> Add(Quantity<U> other)
        {
            return Add(other, Unit);
        }
        public Quantity<U> Add(Quantity<U> other, U targetUnit)
        {
            ValidateArithmeticOperands(other, targetUnit, true);

            double baseResult = PerformBaseArithmetic(other, ArithmeticOperation.ADD);
            dynamic t = targetUnit;

            double converted = t.ConvertFromBaseUnit(baseResult);

            return new Quantity<U>(Math.Round(converted, 2), targetUnit);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return $"Quantity({value}, {unit})";
        }

    }
}