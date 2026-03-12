using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelLayer.Enums;
using ModelLayer.Models;
using BusinessLayer.Services;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class TemperatureTest
    {
        private const double Precision = 0.0001;

        [TestMethod]
        public void TestTemperatureEquality_CelsiusToCelsius_SameValue()
        {
            var tempA = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius);
            var tempB = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius);

            Assert.IsTrue(tempA.Equals(tempB));
        }

        [TestMethod]
        public void TestTemperatureEquality_FahrenheitToFahrenheit_SameValue()
        {
            var tempA = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit);
            var tempB = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit);

            Assert.IsTrue(tempA.Equals(tempB));
        }

        [TestMethod]
        public void TestTemperatureEquality_CelsiusToFahrenheit_0Celsius32Fahrenheit()
        {
            var cVal = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius);
            var fVal = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit);

            Assert.IsTrue(cVal.Equals(fVal));
        }

        [TestMethod]
        public void TestTemperatureEquality_CelsiusToFahrenheit_100Celsius212Fahrenheit()
        {
            var cVal = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius);
            var fVal = new Quantity<TemperatureUnit>(212.0, TemperatureUnit.Fahrenheit);

            Assert.IsTrue(cVal.Equals(fVal));
        }

        [TestMethod]
        public void TestTemperatureEquality_CelsiusToFahrenheit_Negative40Equal()
        {
            var cVal = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.Celsius);
            var fVal = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.Fahrenheit);

            Assert.IsTrue(cVal.Equals(fVal));
        }

        [TestMethod]
        public void TestTemperatureEquality_SymmetricProperty()
        {
            var first = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius);
            var second = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit);

            Assert.IsTrue(first.Equals(second));
            Assert.IsTrue(second.Equals(first));
        }

        [TestMethod]
        public void TestTemperatureEquality_ReflexiveProperty()
        {
            var sampleTemp = new Quantity<TemperatureUnit>(25.0, TemperatureUnit.Celsius);

            Assert.IsTrue(sampleTemp.Equals(sampleTemp));
        }

        [DataTestMethod]
        [DataRow(50.0, 122.0)]
        [DataRow(-20.0, -4.0)]
        public void TestTemperatureConversion_CelsiusToFahrenheit_VariousValues(double celsiusInput, double expectedFahrenheit)
        {
            var source = new Quantity<TemperatureUnit>(celsiusInput, TemperatureUnit.Celsius);

            var converted = source.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.AreEqual(expectedFahrenheit, converted.Value, Precision);
        }

        [DataTestMethod]
        [DataRow(122.0, 50.0)]
        [DataRow(-4.0, -20.0)]
        public void TestTemperatureConversion_FahrenheitToCelsius_VariousValues(double fahrenheitInput, double expectedCelsius)
        {
            var source = new Quantity<TemperatureUnit>(fahrenheitInput, TemperatureUnit.Fahrenheit);

            var converted = source.ConvertTo(TemperatureUnit.Celsius);

            Assert.AreEqual(expectedCelsius, converted.Value, Precision);
        }

        [TestMethod]
        public void TestTemperatureConversion_RoundTrip_PreservesValue()
        {
            double original = 75.5;

            var start = new Quantity<TemperatureUnit>(original, TemperatureUnit.Fahrenheit);

            var toC = start.ConvertTo(TemperatureUnit.Celsius);
            var backToF = toC.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.AreEqual(original, backToF.Value, Precision);
        }

        [TestMethod]
        public void TestTemperatureConversion_SameUnit()
        {
            var temp = new Quantity<TemperatureUnit>(25.0, TemperatureUnit.Celsius);

            var result = temp.ConvertTo(TemperatureUnit.Celsius);

            Assert.AreEqual(25.0, result.Value, Precision);
        }

        [TestMethod]
        public void TestTemperatureConversion_ZeroValue()
        {
            var temp = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius);

            var converted = temp.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.AreEqual(32.0, converted.Value, Precision);
        }

        [TestMethod]
        public void TestTemperatureConversion_NegativeValues()
        {
            var temp = new Quantity<TemperatureUnit>(-10.0, TemperatureUnit.Celsius);

            var converted = temp.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.AreEqual(14.0, converted.Value, Precision);
        }

        [TestMethod]
        public void TestTemperatureConversion_LargeValues()
        {
            var temp = new Quantity<TemperatureUnit>(1000.0, TemperatureUnit.Celsius);

            var converted = temp.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.AreEqual(1832.0, converted.Value, Precision);
        }

        [TestMethod]
        public void TestTemperatureOperation_Add_ShouldReturnCorrectSum()
        {
            var first = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius);
            var second = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius);

            var sum = first.Add(second);

            Assert.AreEqual(150.0, sum.Value);
            Assert.AreEqual(TemperatureUnit.Celsius, sum.Unit);
        }

        [TestMethod]
        public void TestTemperatureUnsupportedOperation_Subtract()
        {
            var first = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius);
            var second = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius);

            var result = first.Subtract(second);

            Assert.AreEqual(50.0, result.Value);
            Assert.AreEqual(TemperatureUnit.Celsius, result.Unit);
        }

        [TestMethod]
        public void TestTemperatureUnsupportedOperation_Divide()
        {
            var first = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius);
            var second = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius);

            try
            {
                first.Divide(second);
                Assert.Fail("Expected InvalidOperationException was not thrown.");
            }
            catch (InvalidOperationException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void TestTemperatureUnsupportedOperation_ErrorMessage()
        {
            var first = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius);
            var second = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius);

            try
            {
                first.Divide(second);
                Assert.Fail("Exception expected");
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("Temperature does not support divide operations.", ex.Message);
            }
        }

        [TestMethod]
        public void TestTemperatureVsLengthIncompatibility()
        {
            var temp = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius);
            var len = new Quantity<LengthUnit>(100.0, LengthUnit.Feet);

            Assert.IsFalse(temp.Equals(len));
        }

        [TestMethod]
        public void TestTemperatureVsWeightIncompatibility()
        {
            var temp = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius);
            var wt = new Quantity<WeightUnit>(50.0, WeightUnit.Kilograms);

            Assert.IsFalse(temp.Equals(wt));
        }

        [TestMethod]
        public void TestTemperatureVsVolumeIncompatibility()
        {
            var temp = new Quantity<TemperatureUnit>(25.0, TemperatureUnit.Celsius);
            var vol = new Quantity<VolumeUnit>(25.0, VolumeUnit.Litre);

            Assert.IsFalse(temp.Equals(vol));
        }

        [TestMethod]
        public void TestTemperatureUnit_NonLinearConversion()
        {
            double result = TemperatureUnit.Fahrenheit.ConvertFromBase(0.0);

            Assert.AreEqual(32.0, result);
        }

        [TestMethod]
        public void TestTemperatureUnit_NameMethod()
        {
            Assert.AreEqual("Celsius", TemperatureUnit.Celsius.ToString());
            Assert.AreEqual("°C", TemperatureUnit.Celsius.GetSymbol());
            Assert.AreEqual("°F", TemperatureUnit.Fahrenheit.GetSymbol());
        }

        [TestMethod]
        public void TestTemperatureUnit_ConversionFactor()
        {
            double baseVal = TemperatureUnit.Celsius.ConvertToBase(1.0);

            Assert.AreEqual(1.0, baseVal);
        }
    }
}