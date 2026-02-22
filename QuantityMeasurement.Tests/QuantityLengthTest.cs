using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Model;
using System;

namespace QuantityMeasurement.Tests
{
    [TestClass]
    public class QuantityLengthTest
    {
        private const double EPSILON = 0.0001;

        // =====================================================
        // UC8 – LengthUnit Enum Tests
        // =====================================================

        [TestMethod]
        public void TestLengthUnit_FeetConversionFactor()
        {
            Assert.AreEqual(1.0, LengthUnit.Feet.GetConversionFactor());
        }

        [TestMethod]
        public void TestLengthUnit_InchConversionFactor()
        {
            Assert.AreEqual(1.0 / 12.0,
                LengthUnit.Inch.GetConversionFactor(),
                EPSILON);
        }

        [TestMethod]
        public void TestLengthUnit_YardConversionFactor()
        {
            Assert.AreEqual(3.0,
                LengthUnit.Yard.GetConversionFactor());
        }

        [TestMethod]
        public void TestLengthUnit_CentimeterConversionFactor()
        {
            Assert.AreEqual(1.0 / 30.48,
                LengthUnit.Centimeter.GetConversionFactor(),
                EPSILON);
        }

        // =====================================================
        // ConvertToBaseUnit Tests (Feet is Base)
        // =====================================================

        [TestMethod]
        public void TestConvertToBaseUnit_Feet()
        {
            Assert.AreEqual(5.0,
                LengthUnit.Feet.ConvertToBaseUnit(5.0));
        }

        [TestMethod]
        public void TestConvertToBaseUnit_Inch()
        {
            Assert.AreEqual(1.0,
                LengthUnit.Inch.ConvertToBaseUnit(12.0),
                EPSILON);
        }

        [TestMethod]
        public void TestConvertToBaseUnit_Yard()
        {
            Assert.AreEqual(3.0,
                LengthUnit.Yard.ConvertToBaseUnit(1.0));
        }

        [TestMethod]
        public void TestConvertToBaseUnit_Centimeter()
        {
            Assert.AreEqual(1.0,
                LengthUnit.Centimeter.ConvertToBaseUnit(30.48),
                EPSILON);
        }

        // =====================================================
        // ConvertFromBaseUnit Tests
        // =====================================================

        [TestMethod]
        public void TestConvertFromBaseUnit_Feet()
        {
            Assert.AreEqual(2.0,
                LengthUnit.Feet.ConvertFromBaseUnit(2.0));
        }

        [TestMethod]
        public void TestConvertFromBaseUnit_Inch()
        {
            Assert.AreEqual(12.0,
                LengthUnit.Inch.ConvertFromBaseUnit(1.0),
                EPSILON);
        }

        [TestMethod]
        public void TestConvertFromBaseUnit_Yard()
        {
            Assert.AreEqual(1.0,
                LengthUnit.Yard.ConvertFromBaseUnit(3.0),
                EPSILON);
        }

        [TestMethod]
        public void TestConvertFromBaseUnit_Centimeter()
        {
            Assert.AreEqual(30.48,
                LengthUnit.Centimeter.ConvertFromBaseUnit(1.0),
                EPSILON);
        }

        // =====================================================
        // UC1 – Equality Tests
        // =====================================================

        [TestMethod]
        public void TestEquality_SameUnit_SameValue()
        {
            var q1 = new QuantityLength(5, LengthUnit.Feet);
            var q2 = new QuantityLength(5, LengthUnit.Feet);

            Assert.IsTrue(q1.Equals(q2));
        }

        [TestMethod]
        public void TestEquality_DifferentUnits()
        {
            var q1 = new QuantityLength(1, LengthUnit.Feet);
            var q2 = new QuantityLength(12, LengthUnit.Inch);

            Assert.IsTrue(q1.Equals(q2));
        }

        [TestMethod]
        public void TestEquality_Reflexive()
        {
            var q = new QuantityLength(3, LengthUnit.Yard);
            Assert.IsTrue(q.Equals(q));
        }

        // =====================================================
        // UC5 – Convert Tests
        // =====================================================

        [TestMethod]
        public void TestConvert_FeetToInch()
        {
            double result =
                QuantityLength.Convert(1, LengthUnit.Feet, LengthUnit.Inch);

            Assert.AreEqual(12.0, result, EPSILON);
        }

        [TestMethod]
        public void TestConvert_CentimeterToInch()
        {
            double result =
                QuantityLength.Convert(2.54, LengthUnit.Centimeter, LengthUnit.Inch);

            Assert.AreEqual(1.0, result, EPSILON);
        }

        // =====================================================
        // UC6 – Addition Tests
        // =====================================================

        [TestMethod]
        public void TestAdd_SameUnit()
        {
            var q1 = new QuantityLength(2, LengthUnit.Feet);
            var q2 = new QuantityLength(3, LengthUnit.Feet);

            var result = q1.Add(q2);

            Assert.IsTrue(result.Equals(
                new QuantityLength(5, LengthUnit.Feet)));
        }

        [TestMethod]
        public void TestAdd_DifferentUnits()
        {
            var q1 = new QuantityLength(1, LengthUnit.Feet);
            var q2 = new QuantityLength(12, LengthUnit.Inch);

            var result = q1.Add(q2);

            Assert.IsTrue(result.Equals(
                new QuantityLength(2, LengthUnit.Feet)));
        }

        // =====================================================
        // UC7 – Addition With Target Unit
        // =====================================================

        [TestMethod]
        public void TestAdd_WithTargetUnit()
        {
            var q1 = new QuantityLength(1, LengthUnit.Feet);
            var q2 = new QuantityLength(12, LengthUnit.Inch);

            var result =
                QuantityLength.AddTwoUnits_TargetUnit(q1, q2, LengthUnit.Yard);

            Assert.IsTrue(result.Equals(
                new QuantityLength(2.0 / 3.0, LengthUnit.Yard)));
        }

        // =====================================================
        // Round Trip Conversion
        // =====================================================

        [TestMethod]
        public void TestRoundTripConversion()
        {
            double original = 5.0;

            double inch =
                QuantityLength.Convert(original, LengthUnit.Feet, LengthUnit.Inch);

            double back =
                QuantityLength.Convert(inch, LengthUnit.Inch, LengthUnit.Feet);

            Assert.AreEqual(original, back, EPSILON);
        }

        // =====================================================
        // Validation Tests
        // =====================================================

        [TestMethod]
        public void TestInvalidValue_NaN()
        {
            Assert.Throws<ArgumentException>(() =>
                new QuantityLength(double.NaN, LengthUnit.Feet)
            );
        }

        [TestMethod]
        public void TestInvalidValue_Infinity()
        {
            Assert.Throws<ArgumentException>(() =>
                new QuantityLength(double.PositiveInfinity, LengthUnit.Feet)
            );
        }

        // =====================================================
        // Architectural Scalability Pattern Check
        // =====================================================

        [TestMethod]
        public void TestUnitImmutability()
        {
            var unit = LengthUnit.Feet;
            Assert.AreEqual(LengthUnit.Feet, unit);
        }
    }
}