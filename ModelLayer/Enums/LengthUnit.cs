namespace ModelLayer.Enums
{
    /// <summary>
    /// Units supported for length measurement.
    /// The base unit used internally is inches.
    /// Conversion logic has been moved to ModelLayer.Helpers.UnitConverter.
    /// </summary>
    public enum LengthUnit
    {
        Inches,
        Feet,
        Yards,
        Centimeters
    }
}
