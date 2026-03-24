using ModelLayer.Enums;
using ModelLayer.Interfaces;
using BusinessLayer.Services;

namespace QuantityMeasurementApp.Tests
{
    public static class TestHelper
    {
        public static IUnitConverter<LengthUnit> LengthConverter
            = new LengthUnitConverter();

        public static IUnitConverter<WeightUnit> WeightConverter
            = new WeightUnitConverter();

        public static IUnitConverter<VolumeUnit> VolumeConverter
            = new VolumeUnitConverter();

        public static IUnitConverter<TemperatureUnit> TemperatureConverter
            = new TemperatureUnitConverter();
    }
}