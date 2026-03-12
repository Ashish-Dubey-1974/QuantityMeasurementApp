using System;

namespace ModelLayer.Enums
{
    /// <summary>
    /// Represents temperature units used in the system.
    /// Celsius is treated as the reference unit for conversions.
    /// </summary>
    public enum TemperatureUnit
    {
        Celsius,
        Fahrenheit,
        Kelvin
    }

    public static class TemperatureUnitExtension
    {
        /// <summary>
        /// Converts a temperature value into the base unit (Celsius).
        /// </summary>
        public static double ConvertToBase(this TemperatureUnit unitType, double inputTemp)
        {
            switch (unitType)
            {
                case TemperatureUnit.Celsius:
                    return inputTemp;

                case TemperatureUnit.Fahrenheit:
                    return (inputTemp - 32.0) * 5.0 / 9.0;

                case TemperatureUnit.Kelvin:
                    return inputTemp - 273.15;

                default:
                    throw new ArgumentException("Unsupported temperature unit");
            }
        }

        /// <summary>
        /// Converts a Celsius value into the requested temperature unit.
        /// </summary>
        public static double ConvertFromBase(this TemperatureUnit unitType, double celsiusValue)
        {
            switch (unitType)
            {
                case TemperatureUnit.Celsius:
                    return celsiusValue;

                case TemperatureUnit.Fahrenheit:
                    return (celsiusValue * 9.0 / 5.0) + 32.0;

                case TemperatureUnit.Kelvin:
                    return celsiusValue + 273.15;

                default:
                    throw new ArgumentException("Unsupported temperature unit");
            }
        }

        /// <summary>
        /// Returns the display symbol associated with each temperature unit.
        /// </summary>
        public static string GetSymbol(this TemperatureUnit unitType)
        {
            switch (unitType)
            {
                case TemperatureUnit.Celsius:
                    return "°C";

                case TemperatureUnit.Fahrenheit:
                    return "°F";

                case TemperatureUnit.Kelvin:
                    return "K";

                default:
                    return unitType.ToString();
            }
        }
    }
}