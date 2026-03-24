using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelLayer.Enums;
using ModelLayer.Models;
using BusinessLayer.Services;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class ArithmeticOperationTest
    {
        private const double tolerance = 1e-6;

        [TestMethod]
        public void Subtract_FeetMinusFeet_ReturnsCorrectDifference()
        {
            var firstQuantity = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, new LengthUnitConverter());
            var secondQuantity = new Quantity<LengthUnit>(5.0, LengthUnit.Feet, new LengthUnitConverter());

            var result = firstQuantity.Subtract(secondQuantity);

            Assert.AreEqual(5.0, result.Value);
            Assert.AreEqual(LengthUnit.Feet, result.Unit);
        }

        [TestMethod]
        public void Subtract_FeetAndInches_ShouldNormalizeResult()
        {
            var left = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, new LengthUnitConverter());
            var right = new Quantity<LengthUnit>(6.0, LengthUnit.Inches, new LengthUnitConverter());

            var output = left.Subtract(right);

            Assert.AreEqual(9.5, output.Value);
        }

        [TestMethod]
        public void Subtract_LitreValues_ToMillilitres()
        {
            var first = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre, new VolumeUnitConverter());
            var second = new Quantity<VolumeUnit>(2.0, VolumeUnit.Litre, new VolumeUnitConverter());

            var answer = first.Subtract(second, VolumeUnit.MilliLiter);

            Assert.AreEqual(3000.0, answer.Value);
            Assert.AreEqual(VolumeUnit.MilliLiter, answer.Unit);
        }

        [TestMethod]
        public void Subtract_WhenResultIsNegative_ShouldKeepSign()
        {
            var smaller = new Quantity<WeightUnit>(2.0, WeightUnit.Kilograms, new WeightUnitConverter());
            var larger = new Quantity<WeightUnit>(5.0, WeightUnit.Kilograms, new WeightUnitConverter());

            var diff = smaller.Subtract(larger);

            Assert.AreEqual(-3.0, diff.Value);
        }

        [TestMethod]
        public void Subtract_OrderMatters_ShouldProduceDifferentResults()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, new LengthUnitConverter());
            var b = new Quantity<LengthUnit>(5.0, LengthUnit.Feet, new LengthUnitConverter());

            double firstResult = a.Subtract(b).Value;
            double secondResult = b.Subtract(a).Value;

            Assert.AreNotEqual(firstResult, secondResult);
        }

        [TestMethod]
        public void Subtract_WithZeroOperand_ShouldReturnSameValue()
        {
            var baseValue = new Quantity<LengthUnit>(5.0, LengthUnit.Feet, new LengthUnitConverter());
            var zeroValue = new Quantity<LengthUnit>(0.0, LengthUnit.Inches, new LengthUnitConverter());

            var output = baseValue.Subtract(zeroValue);

            Assert.AreEqual(5.0, output.Value);
        }

        [TestMethod]
        public void Divide_SameUnits_ShouldReturnExpectedRatio()
        {
            var numerator = new Quantity<WeightUnit>(10.0, WeightUnit.Kilograms, new WeightUnitConverter());
            var denominator = new Quantity<WeightUnit>(2.0, WeightUnit.Kilograms, new WeightUnitConverter());

            double ratio = numerator.Divide(denominator);

            Assert.AreEqual(5.0, ratio);
        }

        [TestMethod]
        public void Divide_InchesByFeet_ShouldNormalizeCorrectly()
        {
            var inchesValue = new Quantity<LengthUnit>(24.0, LengthUnit.Inches, new LengthUnitConverter());
            var feetValue = new Quantity<LengthUnit>(2.0, LengthUnit.Feet, new LengthUnitConverter());

            double result = inchesValue.Divide(feetValue);

            Assert.AreEqual(1.0, result, tolerance);
        }

        [TestMethod]
        public void Divide_ByZeroQuantity_ShouldThrowArithmeticException()
        {
            var numerator = new Quantity<VolumeUnit>(10.0, VolumeUnit.Litre, new VolumeUnitConverter());
            var zeroDenominator = new Quantity<VolumeUnit>(0.0, VolumeUnit.Litre, new VolumeUnitConverter());

            try
            {
                numerator.Divide(zeroDenominator);
                Assert.Fail("Expected ArithmeticException was not thrown.");
            }
            catch (ArithmeticException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void Subtract_ShouldNotModifyOriginalObject()
        {
            var original = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, new LengthUnitConverter());
            var deduction = new Quantity<LengthUnit>(2.0, LengthUnit.Feet, new LengthUnitConverter());

            original.Subtract(deduction);

            Assert.AreEqual(10.0, original.Value);
        }

        [TestMethod]
        public void AddThenSubtract_ShouldReturnInitialValue()
        {
            var start = new Quantity<LengthUnit>(10.0, LengthUnit.Feet, new LengthUnitConverter());
            var delta = new Quantity<LengthUnit>(2.0, LengthUnit.Feet, new LengthUnitConverter());

            var result = start.Add(delta).Subtract(delta);

            Assert.AreEqual(10.0, result.Value, tolerance);
        }
    }
}