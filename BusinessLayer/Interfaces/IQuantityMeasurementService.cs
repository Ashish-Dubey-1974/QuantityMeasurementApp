using System;
using ModelLayer.DTOs;
using ModelLayer.Models;

namespace BusinessLayer.Interfaces
{
    public interface IQuantityMeasurementService
    {
        QuantityDTO Convert(QuantityDTO input, string targetUnitName);
        QuantityDTO Compare(QuantityDTO first, QuantityDTO second);
        QuantityDTO Add(QuantityDTO first, QuantityDTO second);
        QuantityDTO Subtract(QuantityDTO first, QuantityDTO second);
        QuantityDTO Divide(QuantityDTO first, QuantityDTO second);

        // Backward-compatible generic API
        bool Compare<U>(Quantity<U> first, Quantity<U> second) where U : struct, Enum;
        Quantity<U> DemonstrateConversion<U>(Quantity<U> source, U target) where U : struct, Enum;
        double DemonstrateConversion<U>(double value, U sourceUnit, U targetUnit) where U : struct, Enum;
        Quantity<U> DemonstrateAddition<U>(Quantity<U> a, Quantity<U> b) where U : struct, Enum;
        Quantity<U> DemonstrateAddition<U>(Quantity<U> a, Quantity<U> b, U resultUnit) where U : struct, Enum;
        Quantity<U> Subtract<U>(Quantity<U> a, Quantity<U> b, U resultUnit) where U : struct, Enum;
        double Divide<U>(double valA, U unitA, double valB, U unitB) where U : struct, Enum;
    }
}
