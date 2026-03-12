namespace ModelLayer.Enums
{
    public enum WeightUnit
    {
        Grams,
        Kilograms,
        Pound
    }

    /// <summary>
    /// Helper methods for weight unit conversions.
    /// Base unit used internally is kilogram.
    /// </summary>
    public static class WeightUnitExtension
    {
        private static readonly double[] BaseFactors =
        {
            0.001,
            1.0,
            0.453592
        };

        public static double GetConversionFactor(this WeightUnit unit)
        {
            return BaseFactors[(int)unit];
        }

        public static double ConvertToBase(this WeightUnit unit, double amount)
        {
            return amount * unit.GetConversionFactor();
        }

        public static double ConvertFromBase(this WeightUnit unit, double baseValue)
        {
            return baseValue / unit.GetConversionFactor();
        }

        public static string GetSymbol(this WeightUnit unit)
        {
            switch (unit)
            {
                case WeightUnit.Grams: return "g";
                case WeightUnit.Kilograms: return "Kg";
                case WeightUnit.Pound: return "lb";
                default: return unit.ToString().ToLower();
            }
        }
    }
}