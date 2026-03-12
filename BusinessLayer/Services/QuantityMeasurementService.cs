using System;
using ModelLayer.Models;
using RepoLayer.Interfaces;
using RepoLayer.Repositories;

namespace BusinessLayer.Services
{
    /// <summary>
    /// Provides helper operations for working with Quantity objects.
    /// It exposes simple methods for comparison, conversion and arithmetic.
    /// </summary>
    public class QuantityMeasurementService
    {
        private readonly IQuantityRepository repository;

        public QuantityMeasurementService()
        {
            repository = new QuantityRepository();
        }

        /// <summary>
        /// Checks whether two quantities represent the same value.
        /// </summary>
        public bool Compare<U>(Quantity<U> firstQuantity, Quantity<U> secondQuantity) where U : struct, Enum
        {
            if (firstQuantity == null || secondQuantity == null)
                return false;

            return firstQuantity.Equals(secondQuantity);
        }

        /// <summary>
        /// Converts a raw numeric value from one unit to another.
        /// </summary>
        public double DemonstrateConversion<U>(double numericValue, U sourceType, U targetType) where U : struct, Enum
        {
            Quantity<U> tempQuantity = new Quantity<U>(numericValue, sourceType);
            Quantity<U> converted = tempQuantity.ConvertTo(targetType);

            return converted.Value;
        }

        /// <summary>
        /// Converts an existing Quantity instance to a different unit.
        /// </summary>
        public Quantity<U> DemonstrateConversion<U>(Quantity<U> originalQuantity, U desiredUnit) where U : struct, Enum
        {
            Quantity<U> resultQuantity = originalQuantity.ConvertTo(desiredUnit);
            return resultQuantity;
        }

        /// <summary>
        /// Adds two quantities and returns the sum.
        /// </summary>
        public Quantity<U> DemonstrateAddition<U>(Quantity<U> leftOperand, Quantity<U> rightOperand) where U : struct, Enum
        {
            Quantity<U> sum = leftOperand.Add(rightOperand);
            return sum;
        }

        /// <summary>
        /// Adds two quantities and converts the result to a chosen unit.
        /// </summary>
        public Quantity<U> DemonstrateAddition<U>(Quantity<U> leftOperand, Quantity<U> rightOperand, U resultUnit) where U : struct, Enum
        {
            Quantity<U> result = leftOperand.Add(rightOperand, resultUnit);
            return result;
        }

        /// <summary>
        /// Performs subtraction between two quantities.
        /// </summary>
        public Quantity<U> Subtract<U>(Quantity<U> firstValue, Quantity<U> secondValue, U resultUnit) where U : struct, Enum
        {
            Quantity<U> difference = firstValue.Subtract(secondValue, resultUnit);
            return difference;
        }

        /// <summary>
        /// Divides one quantity by another and returns the ratio.
        /// </summary>
        public double Divide<T>(double firstValue, T firstUnit, double secondValue, T secondUnit) where T : struct, Enum
        {
            Quantity<T> dividend = new Quantity<T>(firstValue, firstUnit);
            Quantity<T> divisor = new Quantity<T>(secondValue, secondUnit);

            double outcome = dividend.Divide(divisor);
            return outcome;
        }
    }
}