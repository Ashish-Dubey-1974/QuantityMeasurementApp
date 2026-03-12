using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelLayer.Enums;
using ModelLayer.Models;
using BusinessLayer.Services;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// Tests to verify the behavior of the generic Quantity model.
    /// Covers equality, conversions, addition, and validation cases.
    /// </summary>
    [TestClass]
    public class GenericQuantityTests
    {
        [TestMethod]
        public void LengthEquality_FeetAndInches_ShouldMatch()
        {
            var lengthA = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);
            var lengthB = new Quantity<LengthUnit>(12.0, LengthUnit.Inches);

            Assert.IsTrue(lengthA.Equals(lengthB));
        }

        [TestMethod]
        public void WeightEquality_KgAndGrams_ShouldMatch()
        {
            var weightA = new Quantity<WeightUnit>(1.0, WeightUnit.Kilograms);
            var weightB = new Quantity<WeightUnit>(1000.0, WeightUnit.Grams);

            Assert.IsTrue(weightA.Equals(weightB));
        }

        [TestMethod]
        public void LengthConversion_FeetToInches()
        {
            var source = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);
            var converted = source.ConvertTo(LengthUnit.Inches);

            Assert.AreEqual(12.0, converted.Value, 1e-6);
            Assert.AreEqual(LengthUnit.Inches, converted.Unit);
        }

        [TestMethod]
        public void WeightConversion_KgToGrams()
        {
            var input = new Quantity<WeightUnit>(1.0, WeightUnit.Kilograms);
            var result = input.ConvertTo(WeightUnit.Grams);

            Assert.AreEqual(1000.0, result.Value, 1e-6);
        }

        [TestMethod]
        public void LengthAddition_FeetAndInches()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);
            var b = new Quantity<LengthUnit>(12.0, LengthUnit.Inches);

            var total = a.Add(b, LengthUnit.Feet);

            Assert.AreEqual(2.0, total.Value, 1e-6);
            Assert.AreEqual(LengthUnit.Feet, total.Unit);
        }

        [TestMethod]
        public void WeightAddition_GramsAndKg()
        {
            var first = new Quantity<WeightUnit>(1000.0, WeightUnit.Grams);
            var second = new Quantity<WeightUnit>(1.0, WeightUnit.Kilograms);

            var result = first.Add(second, WeightUnit.Kilograms);

            Assert.AreEqual(2.0, result.Value, 1e-6);
        }

        [TestMethod]
        public void CrossCategoryComparison_ShouldReturnFalse()
        {
            var distance = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);
            var mass = new Quantity<WeightUnit>(1.0, WeightUnit.Kilograms);

            Assert.IsFalse(distance.Equals(mass));
        }

        [TestMethod]
        public void Constructor_InvalidNumbers_ShouldThrowException()
        {
            bool exceptionThrown = false;

            try
            {
                new Quantity<LengthUnit>(double.NaN, LengthUnit.Feet);
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);

            exceptionThrown = false;

            try
            {
                new Quantity<LengthUnit>(double.PositiveInfinity, LengthUnit.Inches);
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        public void ToString_ShouldContainSymbol()
        {
            var sample = new Quantity<LengthUnit>(5.0, LengthUnit.Inches);

            string text = sample.ToString();

            Assert.IsTrue(text.Contains("5"));
            Assert.IsTrue(text.Contains("in"));
        }

        [TestMethod]
        public void ZeroValuesAcrossUnits_ShouldBeEqual()
        {
            var zeroFeet = new Quantity<LengthUnit>(0.0, LengthUnit.Feet);
            var zeroInches = new Quantity<LengthUnit>(0.0, LengthUnit.Inches);

            Assert.IsTrue(zeroFeet.Equals(zeroInches));
        }

        [TestMethod]
        public void LargeValues_AdditionShouldRemainCorrect()
        {
            var large = new Quantity<WeightUnit>(1000000.0, WeightUnit.Kilograms);
            var small = new Quantity<WeightUnit>(1.0, WeightUnit.Grams);

            var result = large.Add(small, WeightUnit.Grams);

            Assert.AreEqual(1000000001.0, result.Value, 1e-6);
        }

        [TestMethod]
        public void PrecisionCheck_CloseValuesShouldBeEqual()
        {
            var first = new Quantity<LengthUnit>(1.0, LengthUnit.Inches);
            var second = new Quantity<LengthUnit>(1.0000001, LengthUnit.Inches);

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void NegativeValues_Addition()
        {
            var positive = new Quantity<LengthUnit>(10.0, LengthUnit.Inches);
            var negative = new Quantity<LengthUnit>(-5.0, LengthUnit.Inches);

            var result = positive.Add(negative);

            Assert.AreEqual(5.0, result.Value, 1e-6);
        }

        [TestMethod]
        public void Equality_Reflexive()
        {
            var item = new Quantity<WeightUnit>(500.0, WeightUnit.Grams);

            Assert.IsTrue(item.Equals(item));
        }

        [TestMethod]
        public void Equality_WithNull_ShouldReturnFalse()
        {
            var sample = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);

            Assert.IsFalse(sample.Equals(null));
        }

        [TestMethod]
        public void HashCode_ForEquivalentValues_ShouldMatch()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);
            var b = new Quantity<LengthUnit>(12.0, LengthUnit.Inches);

            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}