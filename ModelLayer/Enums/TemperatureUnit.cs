namespace ModelLayer.Enums
{
    /// <summary>
    /// Temperature units supported in the system.
    /// Celsius is treated as the reference (base) unit for conversions.
    /// Conversion logic has been moved to ModelLayer.Helpers.UnitConverter.
    /// </summary>
    public enum TemperatureUnit
    {
        Celsius,
        Fahrenheit,
        Kelvin
    }
}
