namespace BusinessLayer.Exceptions
{
    /// <summary>
    /// Custom unchecked exception for all quantity measurement failures.
    ///
    /// Thrown by QuantityMeasurementService when:
    ///  - Incompatible measurement categories are mixed (e.g. Length + Weight).
    ///  - A unit name supplied in a QuantityDTO is not recognised.
    ///  - An unsupported arithmetic operation is attempted (e.g. Temperature divide).
    ///  - Division by zero is encountered.
    ///
    /// Extends <see cref="Exception"/> (unchecked) so callers can choose to catch
    /// it without being forced to by the compiler, while still getting rich context.
    /// </summary>
    public class QuantityMeasurementException : Exception
    {
        public QuantityMeasurementException(string message)
            : base(message) { }

        public QuantityMeasurementException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
