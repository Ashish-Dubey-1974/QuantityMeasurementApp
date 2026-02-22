using System;

namespace QuantityMeasurementApp.Model
{
    public enum WeightUnit
    {
        Gram,
        Kilogram,
        Pound
    }

    public static class WeightUnitExtensions
    {
        private const double GRAM_TO_KG = 1.0 / 1000.0;
        private const double POUND_TO_KG = 0.453592;

        // Base Unit = Kilogram
        public static double ConvertToBaseUnit(this WeightUnit unit, double value)
        {
            return unit switch
            {
                WeightUnit.Kilogram => value,
                WeightUnit.Gram => value * GRAM_TO_KG,
                WeightUnit.Pound => value * POUND_TO_KG,
                _ => throw new ArgumentException("Invalid unit")
            };
        }

        public static double ConvertFromBaseUnit(this WeightUnit unit, double baseValue)
        {
            return unit switch
            {
                WeightUnit.Kilogram => baseValue,
                WeightUnit.Gram => baseValue / GRAM_TO_KG,
                WeightUnit.Pound => baseValue / POUND_TO_KG,
                _ => throw new ArgumentException("Invalid unit")
            };
        }
    }
}