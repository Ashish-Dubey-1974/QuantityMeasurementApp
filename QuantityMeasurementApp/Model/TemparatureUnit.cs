namespace QuantityMeasurementApp.Model
{
    public enum TemperatureUnit : int
    {
        CELSIUS,
        FAHRENHEIT,
        KELVIN
    }

    public static class TemperatureUnitExtensions
    {
        public static double ConvertToBaseUnit(this TemperatureUnit unit, double value)
        {
            return unit switch
            {
                TemperatureUnit.CELSIUS => value,
                TemperatureUnit.FAHRENHEIT => (value - 32) * 5 / 9,
                TemperatureUnit.KELVIN => value - 273.15,
                _ => throw new ArgumentException("Invalid temperature unit")
            };
        }

        public static double ConvertFromBaseUnit(this TemperatureUnit unit, double baseValue)
        {
            return unit switch
            {
                TemperatureUnit.CELSIUS => baseValue,
                TemperatureUnit.FAHRENHEIT => baseValue * 9 / 5 + 32,
                TemperatureUnit.KELVIN => baseValue + 273.15,
                _ => throw new ArgumentException("Invalid temperature unit")
            };
        }

        public static bool SupportsArithmetic(this TemperatureUnit unit)
        {
            return false;
        }

        public static void ValidateOperationSupport(this TemperatureUnit unit, string operation)
        {
            throw new NotSupportedException(
                $"Temperature does not support {operation} operation.");
        }

        public static string GetUnitName(this TemperatureUnit unit)
        {
            return unit.ToString();
        }
    }
}