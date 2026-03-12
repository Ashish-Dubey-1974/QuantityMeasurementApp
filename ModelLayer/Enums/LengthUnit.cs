namespace ModelLayer.Enums
{
    /// <summary>
    /// Units supported for length measurement.
    /// The base unit used internally is inches.
    /// </summary>
    public enum LengthUnit
    {
        Inches,
        Feet,
        Yards,
        Centimeters
    }

    public static class LengthUnitExtensions
    {
        private static readonly double[] BaseFactors =
        {
            1.0,
            12.0,
            36.0,
            0.393701
        };

        public static double GetConversionFactor(this LengthUnit unit)
        {
            return BaseFactors[(int)unit];
        }

        public static double ConvertToBase(this LengthUnit unit, double amount)
        {
            return amount * unit.GetConversionFactor();
        }

        public static double ConvertFromBase(this LengthUnit unit, double baseValue)
        {
            return baseValue / unit.GetConversionFactor();
        }

        public static string GetSymbol(this LengthUnit unit)
        {
            switch (unit)
            {
                case LengthUnit.Inches: return "in";
                case LengthUnit.Feet: return "ft";
                case LengthUnit.Yards: return "yd";
                case LengthUnit.Centimeters: return "cm";
                default: return unit.ToString().ToLower();
            }
        }
    }
}