namespace ModelLayer.DTOs
{
    /// <summary>
    /// Data Transfer Object for passing quantity measurement data between application layers.
    ///
    /// Uses string-based <see cref="UnitName"/> and <see cref="Category"/> identifiers so
    /// that this class stays decoupled from the concrete enum types in ModelLayer.Enums,
    /// making it safe to use at layer boundaries and in future REST API contracts.
    ///
    /// Supported categories  : "Length" | "Weight" | "Volume" | "Temperature"
    /// Example unit names    : "Feet", "Inches", "Celsius", "Kilograms", "Litre" …
    ///
    /// Error representation  : set <see cref="IsError"/> = true and populate
    ///                         <see cref="ErrorMessage"/> via the static factory
    ///                         <see cref="Error(string)"/>.
    /// </summary>
    public class QuantityDTO
    {
        /// <summary>The numeric value of this quantity.</summary>
        public double Value { get; set; }

        /// <summary>Unit name, e.g. "Feet", "Celsius", "Kilograms".</summary>
        public string UnitName { get; set; } = string.Empty;

        /// <summary>Measurement category: "Length", "Weight", "Volume", or "Temperature".</summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>True when this DTO represents an operation error rather than a result.</summary>
        public bool IsError { get; set; }

        /// <summary>Human-readable error message; only populated when <see cref="IsError"/> is true.</summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>Optional description of the operation result for display purposes.</summary>
        public string ResultDescription { get; set; } = string.Empty;

        // ── Constructors ──────────────────────────────────────────────────────

        public QuantityDTO() { }

        public QuantityDTO(double value, string unitName, string category)
        {
            Value    = value;
            UnitName = unitName;
            Category = category;
        }

        // ── Factory ───────────────────────────────────────────────────────────

        /// <summary>Creates an error-state DTO carrying no quantity data.</summary>
        public static QuantityDTO Error(string message)
            => new QuantityDTO { IsError = true, ErrorMessage = message };

        // ── Display ───────────────────────────────────────────────────────────

        public override string ToString()
            => IsError ? $"Error: {ErrorMessage}" : $"{Value} {UnitName}";
    }
}
