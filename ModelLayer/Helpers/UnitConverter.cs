using System;
using ModelLayer.Enums;

namespace ModelLayer.Helpers
{
    /// <summary>
    /// Centralized utility for all unit conversions across every measurement category.
    ///
    /// UC15 refactoring: all conversion factor logic that previously lived in static
    /// extension classes (LengthUnitExtensions, WeightUnitExtension, etc.) has been
    /// consolidated here, following the Single Responsibility Principle and improving
    /// the separation of concerns between data definitions (enums) and behaviour (conversion).
    ///
    /// Provides:
    ///   - Per-type typed helpers (ToBaseLength, FromBaseWeight, etc.)
    ///   - Generic dispatchers (ToBase&lt;T&gt;, FromBase&lt;T&gt;, GetSymbol&lt;T&gt;)
    ///   - String-based helpers for the DTO / service layer (GetCategory, ParseUnit)
    /// </summary>
    public static class UnitConverter
    {
        // ── Length (base unit: inches) ────────────────────────────────────────
        private static readonly double[] LengthFactors = { 1.0, 12.0, 36.0, 0.393701 };

        public static double ToBaseLength(LengthUnit unit, double value)
            => value * LengthFactors[(int)unit];

        public static double FromBaseLength(LengthUnit unit, double baseValue)
            => baseValue / LengthFactors[(int)unit];

        public static string GetLengthSymbol(LengthUnit unit) => unit switch
        {
            LengthUnit.Inches      => "in",
            LengthUnit.Feet        => "ft",
            LengthUnit.Yards       => "yd",
            LengthUnit.Centimeters => "cm",
            _                      => unit.ToString().ToLower()
        };

        // ── Weight (base unit: kilograms) ─────────────────────────────────────
        private static readonly double[] WeightFactors = { 0.001, 1.0, 0.453592 };

        public static double ToBaseWeight(WeightUnit unit, double value)
            => value * WeightFactors[(int)unit];

        public static double FromBaseWeight(WeightUnit unit, double baseValue)
            => baseValue / WeightFactors[(int)unit];

        public static string GetWeightSymbol(WeightUnit unit) => unit switch
        {
            WeightUnit.Grams     => "g",
            WeightUnit.Kilograms => "Kg",
            WeightUnit.Pound     => "lb",
            _                    => unit.ToString().ToLower()
        };

        // ── Volume (base unit: litre) ─────────────────────────────────────────
        private static readonly double[] VolumeFactors = { 1.0, 0.001, 3.78541 };

        public static double ToBaseVolume(VolumeUnit unit, double value)
            => value * VolumeFactors[(int)unit];

        public static double FromBaseVolume(VolumeUnit unit, double baseValue)
            => baseValue / VolumeFactors[(int)unit];

        public static string GetVolumeSymbol(VolumeUnit unit) => unit switch
        {
            VolumeUnit.Litre      => "L",
            VolumeUnit.MilliLiter => "mL",
            VolumeUnit.Gallon     => "gal",
            _                     => unit.ToString().ToLower()
        };

        // ── Temperature (base unit: Celsius) ──────────────────────────────────
        public static double ToBaseTemperature(TemperatureUnit unit, double value) => unit switch
        {
            TemperatureUnit.Celsius    => value,
            TemperatureUnit.Fahrenheit => (value - 32.0) * 5.0 / 9.0,
            TemperatureUnit.Kelvin     => value - 273.15,
            _                          => throw new ArgumentException("Unsupported temperature unit.")
        };

        public static double FromBaseTemperature(TemperatureUnit unit, double celsiusValue) => unit switch
        {
            TemperatureUnit.Celsius    => celsiusValue,
            TemperatureUnit.Fahrenheit => (celsiusValue * 9.0 / 5.0) + 32.0,
            TemperatureUnit.Kelvin     => celsiusValue + 273.15,
            _                          => throw new ArgumentException("Unsupported temperature unit.")
        };

        public static string GetTemperatureSymbol(TemperatureUnit unit) => unit switch
        {
            TemperatureUnit.Celsius    => "°C",
            TemperatureUnit.Fahrenheit => "°F",
            TemperatureUnit.Kelvin     => "K",
            _                          => unit.ToString()
        };

        // ── Generic dispatchers ───────────────────────────────────────────────

        /// <summary>Converts <paramref name="value"/> to its category base unit.</summary>
        public static double ToBase<T>(T unit, double value) where T : struct, Enum
        {
            if (unit is LengthUnit      l) return ToBaseLength(l, value);
            if (unit is WeightUnit      w) return ToBaseWeight(w, value);
            if (unit is VolumeUnit      v) return ToBaseVolume(v, value);
            if (unit is TemperatureUnit t) return ToBaseTemperature(t, value);
            throw new InvalidOperationException($"Unsupported unit type: {typeof(T).Name}");
        }

        /// <summary>Converts a base-unit value back into the specified unit.</summary>
        public static double FromBase<T>(T unit, double baseValue) where T : struct, Enum
        {
            if (unit is LengthUnit      l) return FromBaseLength(l, baseValue);
            if (unit is WeightUnit      w) return FromBaseWeight(w, baseValue);
            if (unit is VolumeUnit      v) return FromBaseVolume(v, baseValue);
            if (unit is TemperatureUnit t) return FromBaseTemperature(t, baseValue);
            throw new InvalidOperationException($"Unsupported unit type: {typeof(T).Name}");
        }

        /// <summary>Returns the display symbol for the given unit (e.g. "ft", "°C").</summary>
        public static string GetSymbol<T>(T unit) where T : struct, Enum
        {
            if (unit is LengthUnit      l) return GetLengthSymbol(l);
            if (unit is WeightUnit      w) return GetWeightSymbol(w);
            if (unit is VolumeUnit      v) return GetVolumeSymbol(v);
            if (unit is TemperatureUnit t) return GetTemperatureSymbol(t);
            return unit.ToString().ToLower();
        }

        // ── String-based helpers for the DTO / controller layer ───────────────

        /// <summary>Returns the measurement category name for a given unit type.</summary>
        public static string GetCategory<T>(T unit) where T : struct, Enum
        {
            if (unit is LengthUnit)      return "Length";
            if (unit is WeightUnit)      return "Weight";
            if (unit is VolumeUnit)      return "Volume";
            if (unit is TemperatureUnit) return "Temperature";
            return "Unknown";
        }

        /// <summary>
        /// Parses a unit name string (case-insensitive) to the corresponding enum value.
        /// Throws <see cref="ArgumentException"/> when the name is not recognised.
        /// </summary>
        public static T ParseUnit<T>(string unitName) where T : struct, Enum
        {
            if (Enum.TryParse<T>(unitName, ignoreCase: true, out T result))
                return result;
            throw new ArgumentException($"Unknown unit '{unitName}' for type {typeof(T).Name}.");
        }
    }
}
