using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelLayer.Enums;
using ModelLayer.Models;
using BusinessLayer.Services;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class VolumeMeasurementTests
    {
        private const double tolerance = 1e-6;

        // -------- Equality Tests --------

        [TestMethod]
        public void testEquality_LitreToLitre_SameValue_ShouldReturnTrue()
        {
            var firstVolume = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
            var secondVolume = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);

            bool comparison = firstVolume.Equals(secondVolume);

            Assert.IsTrue(comparison);
        }

        [TestMethod]
        public void testEquality_LitreToML_EquivalentValue_ShouldReturnTrue()
        {
            // 1 L = 1000 mL
            var litreQuantity = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
            var mlQuantity = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MilliLiter);

            Assert.IsTrue(litreQuantity.Equals(mlQuantity));
        }

        [TestMethod]
        public void testEquality_LitreToGallon_EquivalentValue_ShouldReturnTrue()
        {
            // 3.78541 L equals 1 gallon
            var litreAmount = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre);
            var gallonAmount = new Quantity<VolumeUnit>(1.0, VolumeUnit.Gallon);

            bool result = litreAmount.Equals(gallonAmount);
            Assert.IsTrue(result);
        }

        // -------- Conversion Tests --------

        [TestMethod]
        public void testConversion_LitreToML_ShouldReturnCorrectValue()
        {
            var litreInput = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);

            Quantity<VolumeUnit> converted = litreInput.ConvertTo(VolumeUnit.MilliLiter);

            Assert.AreEqual(1000.0, converted.Value, tolerance);
            Assert.AreEqual(VolumeUnit.MilliLiter, converted.Unit);
        }

        [TestMethod]
        public void testConversion_GallonToLitre_ShouldReturnCorrectValue()
        {
            var gallonInput = new Quantity<VolumeUnit>(1.0, VolumeUnit.Gallon);

            Quantity<VolumeUnit> convertedValue = gallonInput.ConvertTo(VolumeUnit.Litre);

            Assert.AreEqual(3.78541, convertedValue.Value, tolerance);
        }

        // -------- Addition Tests --------

        [TestMethod]
        public void testAddition_LitreAndML_ShouldReturnSumInLitre()
        {
            // 1L + 1000mL = 2L
            var first = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
            var second = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MilliLiter);

            Quantity<VolumeUnit> total = first.Add(second);

            Assert.AreEqual(2.0, total.Value, tolerance);
        }

        [TestMethod]
        public void testAddition_LitreAndGallon_ExplicitTarget_ShouldReturnSumInGallon()
        {
            // 3.78541L + 1 gallon = 2 gallons
            var partOne = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre);
            var partTwo = new Quantity<VolumeUnit>(1.0, VolumeUnit.Gallon);

            Quantity<VolumeUnit> outcome = partOne.Add(partTwo, VolumeUnit.Gallon);

            Assert.AreEqual(2.0, outcome.Value, tolerance);
        }

        // -------- Cross Type Equality --------

        [TestMethod]
        public void testEquality_VolumeVsLength_ShouldReturnFalse()
        {
            var volumeValue = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
            var lengthValue = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);

            bool check = volumeValue.Equals(lengthValue);

            Assert.IsFalse(check);
        }

        [TestMethod]
        public void testEquality_VolumeVsWeight_ShouldReturnFalse()
        {
            var volumeSample = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
            var weightSample = new Quantity<WeightUnit>(1.0, WeightUnit.Kilograms);

            Assert.IsFalse(volumeSample.Equals(weightSample));
        }

        // -------- Edge Cases --------

        [TestMethod]
        public void testZeroValue_AcrossVolumeUnits_ShouldBeEqual()
        {
            var zeroLitres = new Quantity<VolumeUnit>(0.0, VolumeUnit.Litre);
            var zeroGallons = new Quantity<VolumeUnit>(0.0, VolumeUnit.Gallon);

            Assert.IsTrue(zeroLitres.Equals(zeroGallons));
        }

        [TestMethod]
        public void testSymmetricEquality_GallonToLitre_ShouldBeSameBothWays()
        {
            var gallonUnit = new Quantity<VolumeUnit>(1.0, VolumeUnit.Gallon);
            var litreUnit = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre);

            Assert.IsTrue(gallonUnit.Equals(litreUnit));
            Assert.IsTrue(litreUnit.Equals(gallonUnit));
        }

        [TestMethod]
        public void testTransitiveEquality_GallonToLitreToML()
        {
            var gallonVal = new Quantity<VolumeUnit>(1.0, VolumeUnit.Gallon);
            var litreVal = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre);
            var mlVal = new Quantity<VolumeUnit>(3785.41, VolumeUnit.MilliLiter);

            Assert.IsTrue(gallonVal.Equals(litreVal));
            Assert.IsTrue(litreVal.Equals(mlVal));
            Assert.IsTrue(gallonVal.Equals(mlVal));
        }

        [TestMethod]
        public void testLargeVolume_ConversionPrecision()
        {
            var largeInput = new Quantity<VolumeUnit>(1000000.0, VolumeUnit.Litre);

            Quantity<VolumeUnit> result = largeInput.ConvertTo(VolumeUnit.MilliLiter);

            Assert.AreEqual(1000000000.0, result.Value, tolerance);
        }

        [TestMethod]
        public void testSmallVolume_MillLitreToGallon()
        {
            // 1 mL ≈ 0.000264172 gallons
            var tinyVolume = new Quantity<VolumeUnit>(1.0, VolumeUnit.MilliLiter);

            Quantity<VolumeUnit> converted = tinyVolume.ConvertTo(VolumeUnit.Gallon);

            Assert.AreEqual(0.000264172, converted.Value, tolerance);
        }

        [TestMethod]
        public void testNegativeVolume_Addition()
        {
            var positivePart = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre);
            var negativePart = new Quantity<VolumeUnit>(-2.0, VolumeUnit.Litre);

            Quantity<VolumeUnit> result = positivePart.Add(negativePart);

            Assert.AreEqual(3.0, result.Value, tolerance);
        }

        [TestMethod]
        public void testAddition_WithZero_ShouldReturnOriginalValue()
        {
            var baseQuantity = new Quantity<VolumeUnit>(10.0, VolumeUnit.Gallon);
            var zeroQuantity = new Quantity<VolumeUnit>(0.0, VolumeUnit.MilliLiter);

            Quantity<VolumeUnit> result = baseQuantity.Add(zeroQuantity);

            Assert.AreEqual(10.0, result.Value, tolerance);
            Assert.AreEqual(VolumeUnit.Gallon, result.Unit);
        }

        [TestMethod]
        public void testRoundTripConversion_LitreToGallonToLitre()
        {
            var startingPoint = new Quantity<VolumeUnit>(10.0, VolumeUnit.Litre);

            Quantity<VolumeUnit> converted = startingPoint.ConvertTo(VolumeUnit.Gallon);
            Quantity<VolumeUnit> backAgain = converted.ConvertTo(VolumeUnit.Litre);

            Assert.AreEqual(10.0, backAgain.Value, tolerance);
        }
    }
}