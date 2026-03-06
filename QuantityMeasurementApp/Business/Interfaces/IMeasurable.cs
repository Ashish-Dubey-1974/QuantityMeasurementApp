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
    }
}