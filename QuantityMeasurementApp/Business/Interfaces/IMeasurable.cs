using System;
using QuantityMeasurementApp.Model;
using QuantityMeasurementApp.Business.Services;
namespace QuantityMeasurementApp.Business.Interfaces
{
    public interface IMeasurable
    {
        double GetConversionFactor();
        double ConvertToBaseUnit(double value);
        double ConvertFromBaseUnit(double baseValue);
        string GetUnitName();
        bool SupportsArithmetic() => true;
        void ValidateOperationSupport(string operation)
        {
            if (!SupportsArithmetic())
                throw new NotSupportedException(
                    $"Operation '{operation}' not supported for this unit.");
        }
    }
}