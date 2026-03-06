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

        public Quantity(double value, U unit)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Invalid value");

            this.value = value;
            this.unit = unit;
        }
        // Subtract method to perform subtraction between two quantities of the same unit type
        public Quantity<U> Subtract(Quantity<U> other)
        {
            if (other == null)
                throw new ArgumentException("Other quantity cannot be null");

            if (unit.GetType() != other.unit.GetType())
                throw new ArgumentException("Cross-category subtraction not allowed");

            dynamic u1 = unit;
            dynamic u2 = other.unit;

            double base1 = u1.ConvertToBaseUnit(value);
            double base2 = u2.ConvertToBaseUnit(other.value);

            double result = base1 - base2;

            double converted = u1.ConvertFromBaseUnit(result);

            converted = Math.Round(converted, 2);

            return new Quantity<U>(converted, unit);
        }
        // Overloaded Subtract method to allow specifying a target unit for the result
        public Quantity<U> Subtract(Quantity<U> other, U targetUnit)
        {
            if (other == null)
                throw new ArgumentException("Other quantity cannot be null");

            dynamic u1 = unit;
            dynamic u2 = other.unit;
            dynamic t = targetUnit;

            double base1 = u1.ConvertToBaseUnit(value);
            double base2 = u2.ConvertToBaseUnit(other.value);

            double result = base1 - base2;

            double converted = t.ConvertFromBaseUnit(result);

            converted = Math.Round(converted, 2);

            return new Quantity<U>(converted, targetUnit);
        }
        // Divide method to perform division between two quantities of the same unit type, returning a double result
        public double Divide(Quantity<U> other)
        {
            if (other == null)
                throw new ArgumentException("Other quantity cannot be null");

            dynamic u1 = unit;
            dynamic u2 = other.unit;

            double base1 = u1.ConvertToBaseUnit(value);
            double base2 = u2.ConvertToBaseUnit(other.value);

            if (base2 == 0)
                throw new ArithmeticException("Division by zero");

            return base1 / base2;
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
            return Add(other, unit);
        }
        public Quantity<U> Add(Quantity<U> other, U targetUnit)
        {
            double base1 = ConvertToBase(this.value, this.unit);
            double base2 = ConvertToBase(other.value, other.unit);

            double sum = base1 + base2;

            dynamic t = targetUnit;
            double result = t.ConvertFromBaseUnit(sum);

            return new Quantity<U>(result, targetUnit);
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