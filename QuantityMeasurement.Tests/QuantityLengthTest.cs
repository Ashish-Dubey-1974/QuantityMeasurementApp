using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Business.Services;
using QuantityMeasurementApp.Model;

namespace QuantityMeasurementAppTests
{
    [TestClass]
    public class QuantityTests
    {

        // -------------------------
        // LENGTH EQUALITY
        // -------------------------

        [TestMethod]
        public void Test_LengthEquality_FeetToInches()
        {
            var q1 = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            var q2 = new Quantity<LengthUnit>(12.0, LengthUnit.INCHES);

            Assert.IsTrue(q1.Equals(q2));
        }

        [TestMethod]
        public void Test_LengthEquality_InchesToFeet()
        {
            var q1 = new Quantity<LengthUnit>(24.0, LengthUnit.INCHES);
            var q2 = new Quantity<LengthUnit>(2.0, LengthUnit.FEET);

            Assert.IsTrue(q1.Equals(q2));
        }


        // -------------------------
        // LENGTH CONVERSION
        // -------------------------

        [TestMethod]
        public void Test_LengthConversion_FeetToInches()
        {
            var q = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);

            var result = q.ConvertTo(LengthUnit.INCHES);

            Assert.AreEqual(12.0, result.Value);
        }

        [TestMethod]
        public void Test_LengthConversion_YardToFeet()
        {
            var q = new Quantity<LengthUnit>(1.0, LengthUnit.YARDS);

            var result = q.ConvertTo(LengthUnit.FEET);

            Assert.AreEqual(3.0, result.Value);
        }


        // -------------------------
        // LENGTH ADDITION
        // -------------------------

        [TestMethod]
        public void Test_LengthAddition_FeetPlusInches()
        {
            var q1 = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            var q2 = new Quantity<LengthUnit>(12.0, LengthUnit.INCHES);

            var result = q1.Add(q2);

            Assert.AreEqual(2.0, result.Value);
        }

        [TestMethod]
        public void Test_LengthAddition_TargetUnit_Inches()
        {
            var q1 = new Quantity<LengthUnit>(1.0, LengthUnit.FEET);
            var q2 = new Quantity<LengthUnit>(12.0, LengthUnit.INCHES);

            var result = q1.Add(q2, LengthUnit.INCHES);

            Assert.AreEqual(24.0, result.Value);
        }


        // -------------------------
        // WEIGHT TESTS
        // -------------------------

        [TestMethod]
        public void Test_WeightEquality_KgToGram()
        {
            var w1 = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var w2 = new Quantity<WeightUnit>(1000.0, WeightUnit.GRAM);

            Assert.IsTrue(w1.Equals(w2));
        }

        [TestMethod]
        public void Test_WeightConversion_KgToGram()
        {
            var w = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);

            var result = w.ConvertTo(WeightUnit.GRAM);

            Assert.AreEqual(1000.0, result.Value);
        }

        [TestMethod]
        public void Test_WeightAddition_KgPlusGram()
        {
            var w1 = new Quantity<WeightUnit>(1.0, WeightUnit.KILOGRAM);
            var w2 = new Quantity<WeightUnit>(1000.0, WeightUnit.GRAM);

            var result = w1.Add(w2);

            Assert.AreEqual(2.0, result.Value);
        }


        // -------------------------
        // VOLUME TESTS
        // -------------------------

        [TestMethod]
        public void Test_VolumeEquality_LitreToMillilitre()
        {
            var v1 = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var v2 = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            Assert.IsTrue(v1.Equals(v2));
        }

        [TestMethod]
        public void Test_VolumeConversion_LitreToMillilitre()
        {
            var v = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);

            var result = v.ConvertTo(VolumeUnit.MILLILITRE);

            Assert.AreEqual(1000.0, result.Value);
        }

        [TestMethod]
        public void Test_VolumeAddition_LitrePlusML()
        {
            var v1 = new Quantity<VolumeUnit>(1.0, VolumeUnit.LITRE);
            var v2 = new Quantity<VolumeUnit>(1000.0, VolumeUnit.MILLILITRE);

            var result = v1.Add(v2);

            Assert.AreEqual(2.0, result.Value);
        }


        // -------------------------
        // SUBTRACTION TESTS (UC12)
        // -------------------------

        [TestMethod]
        public void Test_Subtraction_FeetMinusInches()
        {
            var q1 = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);
            var q2 = new Quantity<LengthUnit>(6.0, LengthUnit.INCHES);

            var result = q1.Subtract(q2);

            Assert.AreEqual(9.5, result.Value);
        }

        [TestMethod]
        public void Test_Subtraction_ResultZero()
        {
            var q1 = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);
            var q2 = new Quantity<LengthUnit>(120.0, LengthUnit.INCHES);

            var result = q1.Subtract(q2);

            Assert.AreEqual(0.0, result.Value);
        }


        // -------------------------
        // DIVISION TESTS (UC12)
        // -------------------------

        [TestMethod]
        public void Test_Division_SameUnit()
        {
            var q1 = new Quantity<LengthUnit>(10.0, LengthUnit.FEET);
            var q2 = new Quantity<LengthUnit>(2.0, LengthUnit.FEET);

            var result = q1.Divide(q2);

            Assert.AreEqual(5.0, result);
        }

        [TestMethod]
        public void Test_Division_CrossUnit()
        {
            var q1 = new Quantity<LengthUnit>(24.0, LengthUnit.INCHES);
            var q2 = new Quantity<LengthUnit>(2.0, LengthUnit.FEET);

            var result = q1.Divide(q2);

            Assert.AreEqual(1.0, result);
        }

        [TestMethod]
        public void Test_TemperatureEquality_CelsiusToFahrenheit()
        {
            var t1 = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);
            var t2 = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.FAHRENHEIT);

            Assert.IsTrue(t1.Equals(t2));
        }
        [TestMethod]
        public void Test_TemperatureEquality_KelvinToCelsius()
        {
            var t1 = new Quantity<TemperatureUnit>(273.15, TemperatureUnit.KELVIN);
            var t2 = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.CELSIUS);

            Assert.IsTrue(t1.Equals(t2));
        }
        [TestMethod]
        public void Test_TemperatureConversion_CelsiusToFahrenheit()
        {
            var t = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);

            var result = t.ConvertTo(TemperatureUnit.FAHRENHEIT);

            Assert.AreEqual(212.0, result.Value, 0.01);
        }
        [TestMethod]
        public void Test_TemperatureConversion_FahrenheitToCelsius()
        {
            var t = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.FAHRENHEIT);

            var result = t.ConvertTo(TemperatureUnit.CELSIUS);

            Assert.AreEqual(0.0, result.Value, 0.01);
        }
        [TestMethod]
        public void Test_TemperatureUnsupported_Add()
        {
            try
            {
                var t1 = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
                var t2 = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.CELSIUS);

                t1.Add(t2);

                Assert.Fail("Expected NotSupportedException was not thrown.");
            }
            catch (NotSupportedException)
            {
                Assert.IsTrue(true);
            }
        }
        [TestMethod]
        public void Test_TemperatureUnsupported_Subtract()
        {
            try
            {
                var t1 = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
                var t2 = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.CELSIUS);

                t1.Subtract(t2);

                Assert.Fail("Expected NotSupportedException was not thrown.");
            }
            catch (NotSupportedException)
            {
                Assert.IsTrue(true);
            }
        }
        [TestMethod]
        public void Test_TemperatureUnsupported_Divide()
        {
            try
            {
                var t1 = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.CELSIUS);
                var t2 = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.CELSIUS);

                t1.Divide(t2);

                Assert.Fail("Expected NotSupportedException was not thrown.");
            }
            catch (NotSupportedException)
            {
                Assert.IsTrue(true);
            }
        }

    }
}