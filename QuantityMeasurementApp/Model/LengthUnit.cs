using System;

namespace QuantityMeasurementApp.Model
{
    public enum LengthUnit
    {
        Feet = 1,               // Base Unit
        Inch = 2,
        Yard = 3,
        Centimeter = 4
    }

    public static class LengthUnitExtensions
    {
        private const double INCH_TO_FEET = 1.0 / 12.0;
        private const double YARD_TO_FEET = 3.0;
        private const double CM_TO_FEET = 1.0 / 30.48;

        // ===============================
        // Convert value in this unit → FEET (Base Unit)
        // ===============================
        public static double ConvertToBaseUnit(this LengthUnit unit, double value)
        {
            return unit switch
            {
                LengthUnit.Feet => value,
                LengthUnit.Inch => value * INCH_TO_FEET,
                LengthUnit.Yard => value * YARD_TO_FEET,
                LengthUnit.Centimeter => value * CM_TO_FEET,
                _ => throw new ArgumentException("Invalid unit")
            };
        }

        // ===============================
        // Convert FEET → This Unit
        // ===============================
        public static double ConvertFromBaseUnit(this LengthUnit unit, double baseValue)
        {
            return unit switch
            {
                LengthUnit.Feet => baseValue,
                LengthUnit.Inch => baseValue / INCH_TO_FEET,
                LengthUnit.Yard => baseValue / YARD_TO_FEET,
                LengthUnit.Centimeter => baseValue / CM_TO_FEET,
                _ => throw new ArgumentException("Invalid unit")
            };
        }

        // ===============================
        // Optional: Get Conversion Factor (for testing)
        // ===============================
        public static double GetConversionFactor(this LengthUnit unit)
        {
            return unit switch
            {
                LengthUnit.Feet => 1.0,
                LengthUnit.Inch => INCH_TO_FEET,
                LengthUnit.Yard => YARD_TO_FEET,
                LengthUnit.Centimeter => CM_TO_FEET,
                _ => throw new ArgumentException("Invalid unit")
            };
        }
    }
}