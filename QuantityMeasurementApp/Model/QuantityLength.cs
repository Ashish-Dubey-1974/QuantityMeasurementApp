using System;

namespace QuantityMeasurementApp.Model
{
    public class QuantityLength
    {
        private readonly double value;
        private readonly LengthUnit unit;
        private const double EPSILON = 0.0001;

        public QuantityLength(double value, LengthUnit unit)
        {
            if (Double.IsNaN(value) || Double.IsInfinity(value))
                throw new ArgumentException("Value must be finite number.");

            if (!Enum.IsDefined(typeof(LengthUnit), unit))
                throw new ArgumentException("Invalid unit.");

            this.value = value;
            this.unit = unit;
        }

        // ===============================
        // Convert current value to FEET (Base)
        // ===============================
        private double ConvertToBase()
        {
            return unit.ConvertToBaseUnit(value);
        }

        // ===============================
        // UC5: Static Conversion API (Backward Compatible)
        // ===============================
        public static double Convert(double value, LengthUnit source, LengthUnit target)
        {
            double baseValue = source.ConvertToBaseUnit(value);
            return target.ConvertFromBaseUnit(baseValue);
        }

        // ===============================
        // UC6: Add (Result in First Operand Unit)
        // ===============================
        public QuantityLength Add(QuantityLength other)
        {
            if (other == null)
                throw new ArgumentException("Second operand cannot be null");

            double sumInBase = this.ConvertToBase() + other.ConvertToBase();

            double resultValue = this.unit.ConvertFromBaseUnit(sumInBase);

            return new QuantityLength(resultValue, this.unit);
        }

        // ===============================
        // UC6 Static Version
        // ===============================
        public static QuantityLength AddTwoUnits(QuantityLength l1, QuantityLength l2)
        {
            if (l1 == null || l2 == null)
                throw new ArgumentException("Operands cannot be null");

            return l1.Add(l2);
        }

        // ===============================
        // UC7: Add With Explicit Target Unit
        // ===============================
        public static QuantityLength AddTwoUnits_TargetUnit(
            QuantityLength l1,
            QuantityLength l2,
            LengthUnit targetUnit)
        {
            if (l1 == null || l2 == null)
                throw new ArgumentException("Operands cannot be null");

            double sumInBase = l1.ConvertToBase() + l2.ConvertToBase();

            double resultValue = targetUnit.ConvertFromBaseUnit(sumInBase);

            return new QuantityLength(resultValue, targetUnit);
        }

        // ===============================
        // Equality Override (Uses Base Unit)
        // ===============================
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (this == obj) return true;
            if (!(obj is QuantityLength)) return false;

            QuantityLength other = (QuantityLength)obj;

            return Math.Abs(this.ConvertToBase() - other.ConvertToBase()) < EPSILON;
        }

        public override int GetHashCode()
        {
            return ConvertToBase().GetHashCode();
        }

        public override string ToString()
        {
            return value + " " + unit;
        }
    }
}