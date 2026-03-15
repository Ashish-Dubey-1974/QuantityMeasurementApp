using System;
using ModelLayer.Enums;
using ModelLayer.Helpers;
using ModelLayer.Models;

namespace BusinessLayer.Helpers
{
    public static class QuantityCalculator
    {
        private const double Epsilon = 1e-6;

        public static Quantity<T> ConvertTo<T>(Quantity<T> source, T target) where T : struct, Enum
        {
            double baseVal  = UnitConverter.ToBase(source.Unit, source.Value);
            double result   = UnitConverter.FromBase(target, baseVal);
            return new Quantity<T>(result, target);
        }

        public static bool AreEqual<T>(Quantity<T> a, Quantity<T> b) where T : struct, Enum
        {
            if (a == null || b == null) return false;
            return Math.Abs(UnitConverter.ToBase(a.Unit, a.Value)
                          - UnitConverter.ToBase(b.Unit, b.Value)) < Epsilon;
        }

        public static Quantity<T> Add<T>(Quantity<T> a, Quantity<T> b, T? resultUnit = null) where T : struct, Enum
        {
            EnsureCompatible(a, b);
            double sum = UnitConverter.ToBase(a.Unit, a.Value)
                       + UnitConverter.ToBase(b.Unit, b.Value);
            T target = resultUnit ?? a.Unit;
            return new Quantity<T>(UnitConverter.FromBase(target, sum), target);
        }

        public static Quantity<T> Subtract<T>(Quantity<T> a, Quantity<T> b, T? resultUnit = null) where T : struct, Enum
        {
            EnsureCompatible(a, b);
            double diff = UnitConverter.ToBase(a.Unit, a.Value)
                        - UnitConverter.ToBase(b.Unit, b.Value);
            T target = resultUnit ?? a.Unit;
            return new Quantity<T>(Math.Round(UnitConverter.FromBase(target, diff), 2), target);
        }

        public static double Divide<T>(Quantity<T> a, Quantity<T> b) where T : struct, Enum
        {
            if (typeof(T) == typeof(TemperatureUnit))
                throw new InvalidOperationException("Temperature does not support divide operations.");

            EnsureCompatible(a, b);
            double baseB = UnitConverter.ToBase(b.Unit, b.Value);
            if (Math.Abs(baseB) < Epsilon)
                throw new ArithmeticException("Cannot divide by zero.");

            return UnitConverter.ToBase(a.Unit, a.Value) / baseB;
        }

        private static void EnsureCompatible<T>(Quantity<T> a, Quantity<T> b) where T : struct, Enum
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));
        }
    }
}
