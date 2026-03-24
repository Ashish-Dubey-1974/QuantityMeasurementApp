using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelLayer.Enums;
using ModelLayer.Models;
using BusinessLayer.Services;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class CentralizeArithmeticOprTest
    {
        private const double tolerance = 1e-6;

        [TestMethod]
        public void Refactor_Add_UsesInternalHelper()
        {
            var first = new Quantity<LengthUnit>(1.0, LengthUnit.Feet, new LengthUnitConverter());
            var second = new Quantity<LengthUnit>(6.0, LengthUnit.Inches, new LengthUnitConverter());

            var output = first.Add(second, LengthUnit.Inches);

            Assert.AreEqual(18.0, output.Value, tolerance);
            Assert.AreEqual(LengthUnit.Inches, output.Unit);
        }

        [TestMethod]
        public void Refactor_Subtract_UsesInternalHelper()
        {
            var first = new Quantity<LengthUnit>(1.0, LengthUnit.Feet, new LengthUnitConverter());
            var second = new Quantity<LengthUnit>(2.0, LengthUnit.Inches, new LengthUnitConverter());

            var difference = first.Subtract(second, LengthUnit.Inches);

            Assert.AreEqual(10.0, difference.Value, tolerance);
            Assert.AreEqual(LengthUnit.Inches, difference.Unit);
        }

        [TestMethod]
        public void Refactor_Divide_UsesInternalHelper()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.Feet, new LengthUnitConverter());
            var b = new Quantity<LengthUnit>(4.0, LengthUnit.Inches, new LengthUnitConverter());

            double ratio = a.Divide(b);

            Assert.AreEqual(3.0, ratio, tolerance);
        }

        [TestMethod]
        public void Arithmetic_Add_BasicEnumScenario()
        {
            var left = new Quantity<LengthUnit>(10, LengthUnit.Inches, new LengthUnitConverter());
            var right = new Quantity<LengthUnit>(5, LengthUnit.Inches, new LengthUnitConverter());

            var sum = left.Add(right);

            Assert.AreEqual(15.0, sum.Value, tolerance);
        }

        [TestMethod]
        public void Arithmetic_Subtract_BasicEnumScenario()
        {
            var left = new Quantity<LengthUnit>(10, LengthUnit.Inches, new LengthUnitConverter());
            var right = new Quantity<LengthUnit>(5, LengthUnit.Inches, new LengthUnitConverter());

            var result = left.Subtract(right);

            Assert.AreEqual(5.0, result.Value, tolerance);
        }

        [TestMethod]
        public void Arithmetic_Divide_BasicEnumScenario()
        {
            var first = new Quantity<LengthUnit>(10, LengthUnit.Inches, new LengthUnitConverter());
            var second = new Quantity<LengthUnit>(5, LengthUnit.Inches, new LengthUnitConverter());

            double result = first.Divide(second);

            Assert.AreEqual(2.0, result, tolerance);
        }

        [TestMethod]
        public void Arithmetic_Divide_ByZero_ShouldThrow()
        {
            var first = new Quantity<LengthUnit>(10, LengthUnit.Inches, new LengthUnitConverter());
            var second = new Quantity<LengthUnit>(0, LengthUnit.Inches, new LengthUnitConverter());

            try
            {
                first.Divide(second);
                Assert.Fail("Expected ArithmeticException was not thrown.");
            }
            catch (ArithmeticException ex)
            {
                Assert.AreEqual("Cannot divide by zero.", ex.Message);
            }
        }

        [TestMethod]
        public void Divide_DoesNotModifyOriginalQuantity()
        {
            var weightA = new Quantity<WeightUnit>(20, WeightUnit.Kilograms, new WeightUnitConverter());
            var weightB = new Quantity<WeightUnit>(2, WeightUnit.Kilograms, new WeightUnitConverter());

            weightA.Divide(weightB);

            Assert.AreEqual(20.0, weightA.Value);
        }

        [TestMethod]
        public void HelperMethod_HandlesConversionAndMath()
        {
            var first = new Quantity<LengthUnit>(1.0, LengthUnit.Feet, new LengthUnitConverter());
            var second = new Quantity<LengthUnit>(1.0, LengthUnit.Inches, new LengthUnitConverter());

            var result = first.Add(second, LengthUnit.Inches);

            Assert.AreEqual(13.0, result.Value, tolerance);
        }

        [TestMethod]
        public void Addition_WithDifferentVolumeUnits()
        {
            var gallonQty = new Quantity<VolumeUnit>(1.0, VolumeUnit.Gallon, new VolumeUnitConverter());
            var litreQty = new Quantity<VolumeUnit>(3.78, VolumeUnit.Litre, new VolumeUnitConverter());

            var result = gallonQty.Add(litreQty, VolumeUnit.Litre);

            Assert.AreEqual(7.56, result.Value, 0.01);
        }

        [TestMethod]
        public void Subtraction_WithWeightUnits()
        {
            var kilo = new Quantity<WeightUnit>(1.0, WeightUnit.Kilograms, new WeightUnitConverter());
            var grams = new Quantity<WeightUnit>(500, WeightUnit.Grams, new WeightUnitConverter());

            var outcome = kilo.Subtract(grams, WeightUnit.Kilograms);

            Assert.AreEqual(0.5, outcome.Value, tolerance);
        }

        [TestMethod]
        public void Division_WithSameUnits()
        {
            var first = new Quantity<LengthUnit>(2.0, LengthUnit.Feet, new LengthUnitConverter());
            var second = new Quantity<LengthUnit>(1.0, LengthUnit.Feet, new LengthUnitConverter());

            double ratio = first.Divide(second);

            Assert.AreEqual(2.0, ratio, tolerance);
        }

        [TestMethod]
        public void Divide_DoesNotRoundResult()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.Inches, new LengthUnitConverter());
            var b = new Quantity<LengthUnit>(3.0, LengthUnit.Inches, new LengthUnitConverter());

            double value = a.Divide(b);

            Assert.AreNotEqual(3.33, value);
            Assert.AreEqual(3.333333, value, tolerance);
        }

        [TestMethod]
        public void DefaultTargetUnit_AddAndSubtract()
        {
            var first = new Quantity<LengthUnit>(1.0, LengthUnit.Feet, new LengthUnitConverter());
            var second = new Quantity<LengthUnit>(12.0, LengthUnit.Inches, new LengthUnitConverter());

            var addResult = first.Add(second);
            var subtractResult = first.Subtract(second);

            Assert.AreEqual(LengthUnit.Feet, addResult.Unit);
            Assert.AreEqual(2.0, addResult.Value, tolerance);

            Assert.AreEqual(LengthUnit.Feet, subtractResult.Unit);
            Assert.AreEqual(0.0, subtractResult.Value, tolerance);
        }

        [TestMethod]
        public void ExplicitTargetUnit_ShouldOverrideDefault()
        {
            var first = new Quantity<LengthUnit>(1.0, LengthUnit.Feet, new LengthUnitConverter());
            var second = new Quantity<LengthUnit>(1.0, LengthUnit.Feet, new LengthUnitConverter());

            var converted = first.Add(second, LengthUnit.Inches);

            Assert.AreEqual(LengthUnit.Inches, converted.Unit);
            Assert.AreEqual(24.0, converted.Value, tolerance);
        }

        [TestMethod]
        public void Add_ShouldNotMutateOriginalObjects()
        {
            var baseWeight = new Quantity<WeightUnit>(10.0, WeightUnit.Kilograms, new WeightUnitConverter());
            var addedWeight = new Quantity<WeightUnit>(5.0, WeightUnit.Kilograms, new WeightUnitConverter());

            baseWeight.Add(addedWeight);

            Assert.AreEqual(10.0, baseWeight.Value);
            Assert.AreEqual(WeightUnit.Kilograms, baseWeight.Unit);

            Assert.AreEqual(5.0, addedWeight.Value);
        }
    }
}