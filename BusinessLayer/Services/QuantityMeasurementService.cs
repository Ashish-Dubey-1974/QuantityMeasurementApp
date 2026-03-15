using System;
using BusinessLayer.Exceptions;
using BusinessLayer.Interfaces;
using ModelLayer.DTOs;
using ModelLayer.Entities;
using ModelLayer.Enums;
using ModelLayer.Helpers;
using ModelLayer.Models;
using RepoLayer.Interfaces;

namespace BusinessLayer.Services
{
    /// <summary>
    /// Core business logic for all quantity measurement operations.
    ///
    /// UC15 responsibilities:
    ///  1. Accept QuantityDTO input from the controller layer.
    ///  2. Map DTOs → Quantity&lt;T&gt; via UnitConverter.ParseUnit helpers.
    ///  3. Validate category compatibility (cross-category operations rejected).
    ///  4. Execute the operation using the Quantity model.
    ///  5. Persist a QuantityMeasurementEntity to the repository.
    ///  6. Return a standardised QuantityDTO result (or error DTO on failure).
    ///
    /// The generic backward-compatible methods (DemonstrateConversion, etc.)
    /// are preserved so all UC1-UC14 test cases continue to pass unchanged.
    ///
    /// Dependency Injection: the repository is injected via constructor, enabling
    /// easy substitution and isolated unit testing.
    /// </summary>
    public class QuantityMeasurementService : IQuantityMeasurementService
    {
        private readonly IQuantityRepository _repository;

        public QuantityMeasurementService(IQuantityRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        // ── DTO-based operations ──────────────────────────────────────────────

        public QuantityDTO Convert(QuantityDTO input, string targetUnitName)
        {
            try
            {
                ValidateDto(input);
                var result = DispatchConvert(input, targetUnitName);
                SaveEntity(new QuantityMeasurementEntity(input, "Convert", result));
                return result;
            }
            catch (QuantityMeasurementException) { throw; }
            catch (Exception ex)
            {
                var err = QuantityDTO.Error(ex.Message);
                SaveEntity(new QuantityMeasurementEntity(input, null, "Convert", ex.Message));
                return err;
            }
        }

        public QuantityDTO Compare(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                ValidateDto(first); ValidateDto(second);
                EnsureSameCategory(first, second);
                var result = DispatchCompare(first, second);
                SaveEntity(new QuantityMeasurementEntity(first, second, "Compare", result));
                return result;
            }
            catch (QuantityMeasurementException) { throw; }
            catch (Exception ex)
            {
                var err = QuantityDTO.Error(ex.Message);
                SaveEntity(new QuantityMeasurementEntity(first, second, "Compare", ex.Message));
                return err;
            }
        }

        public QuantityDTO Add(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                ValidateDto(first); ValidateDto(second);
                EnsureSameCategory(first, second);
                var result = DispatchArithmetic(first, second, "Add");
                SaveEntity(new QuantityMeasurementEntity(first, second, "Add", result));
                return result;
            }
            catch (QuantityMeasurementException) { throw; }
            catch (Exception ex)
            {
                var err = QuantityDTO.Error(ex.Message);
                SaveEntity(new QuantityMeasurementEntity(first, second, "Add", ex.Message));
                return err;
            }
        }

        public QuantityDTO Subtract(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                ValidateDto(first); ValidateDto(second);
                EnsureSameCategory(first, second);
                var result = DispatchArithmetic(first, second, "Subtract");
                SaveEntity(new QuantityMeasurementEntity(first, second, "Subtract", result));
                return result;
            }
            catch (QuantityMeasurementException) { throw; }
            catch (Exception ex)
            {
                var err = QuantityDTO.Error(ex.Message);
                SaveEntity(new QuantityMeasurementEntity(first, second, "Subtract", ex.Message));
                return err;
            }
        }

        public QuantityDTO Divide(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                ValidateDto(first); ValidateDto(second);
                EnsureSameCategory(first, second);
                var result = DispatchArithmetic(first, second, "Divide");
                SaveEntity(new QuantityMeasurementEntity(first, second, "Divide", result));
                return result;
            }
            catch (QuantityMeasurementException) { throw; }
            catch (Exception ex)
            {
                var err = QuantityDTO.Error(ex.Message);
                SaveEntity(new QuantityMeasurementEntity(first, second, "Divide", ex.Message));
                return err;
            }
        }

        // ── DTO dispatch helpers ──────────────────────────────────────────────

        private QuantityDTO DispatchConvert(QuantityDTO input, string targetUnitName)
        {
            return input.Category.ToLower() switch
            {
                "length" => DoConvert<LengthUnit>(input, targetUnitName),
                "weight" => DoConvert<WeightUnit>(input, targetUnitName),
                "volume" => DoConvert<VolumeUnit>(input, targetUnitName),
                "temperature" => DoConvert<TemperatureUnit>(input, targetUnitName),
                _ => throw new QuantityMeasurementException($"Unknown category: {input.Category}")
            };
        }

        private QuantityDTO DoConvert<T>(QuantityDTO input, string targetUnitName) where T : struct, Enum
        {
            T sourceUnit = UnitConverter.ParseUnit<T>(input.UnitName);
            T targetUnit = UnitConverter.ParseUnit<T>(targetUnitName);
            var q = new Quantity<T>(input.Value, sourceUnit);
            var result = q.ConvertTo(targetUnit);
            return new QuantityDTO(result.Value, targetUnitName, input.Category)
            {
                ResultDescription = $"{input} converted to {targetUnitName}"
            };
        }

        private QuantityDTO DispatchCompare(QuantityDTO first, QuantityDTO second)
        {
            return first.Category.ToLower() switch
            {
                "length"      => DoCompare<LengthUnit>(first, second),
                "weight"      => DoCompare<WeightUnit>(first, second),
                "volume"      => DoCompare<VolumeUnit>(first, second),
                "temperature" => DoCompare<TemperatureUnit>(first, second),
                _ => throw new QuantityMeasurementException($"Unknown category: {first.Category}")
            };
        }

        private QuantityDTO DoCompare<T>(QuantityDTO first, QuantityDTO second) where T : struct, Enum
        {
            T u1 = UnitConverter.ParseUnit<T>(first.UnitName);
            T u2 = UnitConverter.ParseUnit<T>(second.UnitName);
            bool equal = new Quantity<T>(first.Value, u1).Equals(new Quantity<T>(second.Value, u2));
            return new QuantityDTO(equal ? 1 : 0, "Boolean", first.Category)
            {
                ResultDescription = equal ? "Equal" : "Not Equal"
            };
        }

        private QuantityDTO DispatchArithmetic(QuantityDTO first, QuantityDTO second, string op)
        {
            return first.Category.ToLower() switch
            {
                "length"      => DoArithmetic<LengthUnit>(first, second, op),
                "weight"      => DoArithmetic<WeightUnit>(first, second, op),
                "volume"      => DoArithmetic<VolumeUnit>(first, second, op),
                "temperature" => DoArithmetic<TemperatureUnit>(first, second, op),
                _ => throw new QuantityMeasurementException($"Unknown category: {first.Category}")
            };
        }

        private QuantityDTO DoArithmetic<T>(QuantityDTO first, QuantityDTO second, string op) where T : struct, Enum
        {
            T u1 = UnitConverter.ParseUnit<T>(first.UnitName);
            T u2 = UnitConverter.ParseUnit<T>(second.UnitName);
            var q1 = new Quantity<T>(first.Value, u1);
            var q2 = new Quantity<T>(second.Value, u2);

            if (op == "Add")
            {
                var r = q1.Add(q2);
                return new QuantityDTO(r.Value, first.UnitName, first.Category);
            }
            if (op == "Subtract")
            {
                var r = q1.Subtract(q2);
                return new QuantityDTO(r.Value, first.UnitName, first.Category);
            }
            if (op == "Divide")
            {
                double ratio = q1.Divide(q2);
                return new QuantityDTO(ratio, "Dimensionless", first.Category)
                {
                    ResultDescription = "Ratio"
                };
            }
            throw new QuantityMeasurementException($"Unknown operation: {op}");
        }

        // ── Validation helpers ────────────────────────────────────────────────

        private static void ValidateDto(QuantityDTO dto)
        {
            if (dto == null)            throw new QuantityMeasurementException("QuantityDTO cannot be null.");
            if (string.IsNullOrWhiteSpace(dto.Category))
                throw new QuantityMeasurementException("QuantityDTO category is required.");
            if (string.IsNullOrWhiteSpace(dto.UnitName))
                throw new QuantityMeasurementException("QuantityDTO unit name is required.");
        }

        private static void EnsureSameCategory(QuantityDTO a, QuantityDTO b)
        {
            if (!string.Equals(a.Category, b.Category, StringComparison.OrdinalIgnoreCase))
                throw new QuantityMeasurementException(
                    $"Cross-category operation not allowed: '{a.Category}' vs '{b.Category}'.");
        }

        private void SaveEntity(QuantityMeasurementEntity entity)
        {
            try { _repository.Save(entity); }
            catch { /* repository errors must never propagate to the caller */ }
        }

        // ── Backward-compatible generic API (UC1–UC14 tests unchanged) ────────

        public bool Compare<U>(Quantity<U> first, Quantity<U> second) where U : struct, Enum
            => first != null && second != null && first.Equals(second);

        public Quantity<U> DemonstrateConversion<U>(Quantity<U> source, U target) where U : struct, Enum
            => source.ConvertTo(target);

        public double DemonstrateConversion<U>(double value, U sourceUnit, U targetUnit) where U : struct, Enum
            => new Quantity<U>(value, sourceUnit).ConvertTo(targetUnit).Value;

        public Quantity<U> DemonstrateAddition<U>(Quantity<U> a, Quantity<U> b) where U : struct, Enum
            => a.Add(b);

        public Quantity<U> DemonstrateAddition<U>(Quantity<U> a, Quantity<U> b, U resultUnit) where U : struct, Enum
            => a.Add(b, resultUnit);

        public Quantity<U> Subtract<U>(Quantity<U> a, Quantity<U> b, U resultUnit) where U : struct, Enum
            => a.Subtract(b, resultUnit);

        public double Divide<U>(double valA, U unitA, double valB, U unitB) where U : struct, Enum
            => new Quantity<U>(valA, unitA).Divide(new Quantity<U>(valB, unitB));
    }
}
