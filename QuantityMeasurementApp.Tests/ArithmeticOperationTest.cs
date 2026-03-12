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
            var firstQuantity = new Quantity<LengthUnit>(10.0, LengthUnit.Feet);
            var secondQuantity = new Quantity<LengthUnit>(5.0, LengthUnit.Feet);

            var result = firstQuantity.Subtract(secondQuantity);

            Assert.AreEqual(5.0, result.Value);
            Assert.AreEqual(LengthUnit.Feet, result.Unit);
        }

        [TestMethod]
        public void Subtract_FeetAndInches_ShouldNormalizeResult()
        {
            // 10 ft - 6 in = 9.5 ft
            var left = new Quantity<LengthUnit>(10.0, LengthUnit.Feet);
            var right = new Quantity<LengthUnit>(6.0, LengthUnit.Inches);

            var output = left.Subtract(right);

            Assert.AreEqual(9.5, output.Value);
        }

        [TestMethod]
        public void Subtract_LitreValues_ToMillilitres()
        {
            var first = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre);
            var second = new Quantity<VolumeUnit>(2.0, VolumeUnit.Litre);

            var answer = first.Subtract(second, VolumeUnit.MilliLiter);

            Assert.AreEqual(3000.0, answer.Value);
            Assert.AreEqual(VolumeUnit.MilliLiter, answer.Unit);
        }

        [TestMethod]
        public void Subtract_WhenResultIsNegative_ShouldKeepSign()
        {
            var smaller = new Quantity<WeightUnit>(2.0, WeightUnit.Kilograms);
            var larger = new Quantity<WeightUnit>(5.0, WeightUnit.Kilograms);

            var diff = smaller.Subtract(larger);

            Assert.AreEqual(-3.0, diff.Value);
        }

        [TestMethod]
        public void Subtract_OrderMatters_ShouldProduceDifferentResults()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.Feet);
            var b = new Quantity<LengthUnit>(5.0, LengthUnit.Feet);

            double firstResult = a.Subtract(b).Value;
            double secondResult = b.Subtract(a).Value;

            Assert.AreNotEqual(firstResult, secondResult);
        }

        [TestMethod]
        public void Subtract_WithZeroOperand_ShouldReturnSameValue()
        {
            var baseValue = new Quantity<LengthUnit>(5.0, LengthUnit.Feet);
            var zeroValue = new Quantity<LengthUnit>(0.0, LengthUnit.Inches);

            var output = baseValue.Subtract(zeroValue);

            Assert.AreEqual(5.0, output.Value);
        }

        [TestMethod]
        public void Divide_SameUnits_ShouldReturnExpectedRatio()
        {
            var numerator = new Quantity<WeightUnit>(10.0, WeightUnit.Kilograms);
            var denominator = new Quantity<WeightUnit>(2.0, WeightUnit.Kilograms);

            double ratio = numerator.Divide(denominator);

            Assert.AreEqual(5.0, ratio);
        }

        [TestMethod]
        public void Divide_InchesByFeet_ShouldNormalizeCorrectly()
        {
            var inchesValue = new Quantity<LengthUnit>(24.0, LengthUnit.Inches);
            var feetValue = new Quantity<LengthUnit>(2.0, LengthUnit.Feet);

            double result = inchesValue.Divide(feetValue);

            Assert.AreEqual(1.0, result, tolerance);
        }

        [TestMethod]
        public void Divide_ByZeroQuantity_ShouldThrowArithmeticException()
        {
            var numerator = new Quantity<VolumeUnit>(10.0, VolumeUnit.Litre);
            var zeroDenominator = new Quantity<VolumeUnit>(0.0, VolumeUnit.Litre);

            try
            {
                numerator.Divide(zeroDenominator);
                Assert.Fail("Expected ArithmeticException was not thrown.");
            }
            catch (ArithmeticException)
            {
                // Expected behavior
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void Subtract_ShouldNotModifyOriginalObject()
        {
            var original = new Quantity<LengthUnit>(10.0, LengthUnit.Feet);
            var deduction = new Quantity<LengthUnit>(2.0, LengthUnit.Feet);

            original.Subtract(deduction);

            Assert.AreEqual(10.0, original.Value);
        }

        [TestMethod]
        public void AddThenSubtract_ShouldReturnInitialValue()
        {
            var start = new Quantity<LengthUnit>(10.0, LengthUnit.Feet);
            var delta = new Quantity<LengthUnit>(2.0, LengthUnit.Feet);

            var result = start.Add(delta).Subtract(delta);

            Assert.AreEqual(10.0, result.Value, tolerance);
        }
    }
}