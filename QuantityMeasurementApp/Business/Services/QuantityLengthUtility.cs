using QuantityMeasurementApp.Business.Interfaces;
using QuantityMeasurementApp.Model;
namespace QuantityMeasurementApp.Business.Services
{
    public class QuantityLengthUtility : IQuantityLength
    {
        public bool CheckEquality(double value1, LengthUnit unit1, double value2, LengthUnit unit2)
        {
            double valueInInches1 = ConvertToInches(value1, unit1);
            double valueInInches2 = ConvertToInches(value2, unit2);
            return valueInInches1 == valueInInches2;
        }

        private double ConvertToInches(double value, LengthUnit unit)
        {
            switch (unit)
            {
                case LengthUnit.Feet:
                    return value * 12; // 1 foot = 12 inches
                case LengthUnit.Inch:
                    return value;
                default:
                    throw new ArgumentException("Invalid length unit");
            }
        }
    }
}