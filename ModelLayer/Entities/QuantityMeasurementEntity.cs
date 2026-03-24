using System;
using ModelLayer.DTOs;

namespace ModelLayer.Entities
{
    /// <summary>
    /// Represents a recorded quantity measurement operation for persistence and audit purposes.
    ///
    /// Design goals:
    ///  - Immutable after construction (all properties are read-only).
    ///  - Different constructors cover single-operand (e.g. conversion) and
    ///    dual-operand (e.g. addition, comparison) scenarios, plus an error state.
    ///  - Implements Serializable so it can be persisted across application restarts.
    ///
    /// Stored in <c>QuantityRepository</c> after every service operation to provide
    /// a full audit trail of measurements that can later be exposed as history,
    /// logs, or REST responses.
    /// </summary>
    [Serializable]
    public class QuantityMeasurementEntity
    {
        // ── Operation details ─────────────────────────────────────────────────

        /// <summary>First (or only) operand of the operation.</summary>
        public QuantityDTO? Operand1 { get; }

        /// <summary>Second operand; null for single-operand operations like conversion.</summary>
        public QuantityDTO? Operand2 { get; }

        /// <summary>Operation name: "Compare", "Convert", "Add", "Subtract", "Divide".</summary>
        public string OperationType { get; }

        /// <summary>Result of the operation; contains error data when <see cref="HasError"/> is true.</summary>
        public QuantityDTO? Result { get; }

        /// <summary>True when an exception was caught during the operation.</summary>
        public bool HasError { get; }

        /// <summary>Error message; only meaningful when <see cref="HasError"/> is true.</summary>
        public string ErrorMessage { get; }

        /// <summary>UTC timestamp recorded at construction time.</summary>
        public DateTime Timestamp { get; }

        // ── Constructor: single-operand (conversion) ──────────────────────────

        /// <summary>
        /// Creates a successful single-operand entity (e.g. unit conversion).
        /// </summary>
        public QuantityMeasurementEntity(QuantityDTO operand, string operationType, QuantityDTO result)
        {
            Operand1      = operand;
            Operand2      = null;
            OperationType = operationType;
            Result        = result;
            HasError      = false;
            ErrorMessage  = string.Empty;
            Timestamp     = DateTime.UtcNow;
        }

        // ── Constructor: dual-operand (add, subtract, compare, divide) ────────

        /// <summary>
        /// Creates a successful dual-operand entity (e.g. addition, comparison).
        /// </summary>
        public QuantityMeasurementEntity(QuantityDTO operand1, QuantityDTO operand2,
                                         string operationType, QuantityDTO result)
        {
            Operand1      = operand1;
            Operand2      = operand2;
            OperationType = operationType;
            Result        = result;
            HasError      = false;
            ErrorMessage  = string.Empty;
            Timestamp     = DateTime.UtcNow;
        }

        // ── Constructor: error state ──────────────────────────────────────────

        /// <summary>
        /// Creates an error-state entity when an exception occurred during the operation.
        /// </summary>
        public QuantityMeasurementEntity(QuantityDTO? operand1, QuantityDTO? operand2,
                                         string operationType, string errorMessage)
        {
            Operand1      = operand1;
            Operand2      = operand2;
            OperationType = operationType;
            HasError      = true;
            ErrorMessage  = errorMessage;
            Result        = QuantityDTO.Error(errorMessage);
            Timestamp     = DateTime.UtcNow;
        }

        // ── Display ───────────────────────────────────────────────────────────

        public override string ToString()
        {
            if (HasError)
                return $"[{Timestamp:HH:mm:ss}] ERROR in {OperationType}: {ErrorMessage}";

            return Operand2 is null
                ? $"[{Timestamp:HH:mm:ss}] {OperationType}: {Operand1} => {Result}"
                : $"[{Timestamp:HH:mm:ss}] {OperationType}: {Operand1} , {Operand2} => {Result}";
        }
    }
}
