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

        private readonly TemperatureUnitConverter converter = new TemperatureUnitConverter();

        [TestMethod]
        public void TestTemperatureEquality_CelsiusToCelsius_SameValue()
        {
            var tempA = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius, converter);
            var tempB = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius, converter);

            Assert.IsTrue(tempA.Equals(tempB));
        }

        [TestMethod]
        public void TestTemperatureEquality_FahrenheitToFahrenheit_SameValue()
        {
            var tempA = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit, converter);
            var tempB = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit, converter);

            Assert.IsTrue(tempA.Equals(tempB));
        }

        [TestMethod]
        public void TestTemperatureEquality_CelsiusToFahrenheit_0Celsius32Fahrenheit()
        {
            var cVal = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius, converter);
            var fVal = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit, converter);

            Assert.IsTrue(cVal.Equals(fVal));
        }

        [TestMethod]
        public void TestTemperatureEquality_CelsiusToFahrenheit_100Celsius212Fahrenheit()
        {
            var cVal = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius, converter);
            var fVal = new Quantity<TemperatureUnit>(212.0, TemperatureUnit.Fahrenheit, converter);

            Assert.IsTrue(cVal.Equals(fVal));
        }

        [TestMethod]
        public void TestTemperatureEquality_CelsiusToFahrenheit_Negative40Equal()
        {
            var cVal = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.Celsius, converter);
            var fVal = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.Fahrenheit, converter);

            Assert.IsTrue(cVal.Equals(fVal));
        }

        [TestMethod]
        public void TestTemperatureEquality_SymmetricProperty()
        {
            var first = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius, converter);
            var second = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit, converter);

            Assert.IsTrue(first.Equals(second));
            Assert.IsTrue(second.Equals(first));
        }

        [TestMethod]
        public void TestTemperatureEquality_ReflexiveProperty()
        {
            var sampleTemp = new Quantity<TemperatureUnit>(25.0, TemperatureUnit.Celsius, converter);

            Assert.IsTrue(sampleTemp.Equals(sampleTemp));
        }

        [TestMethod]
        public void TestTemperatureConversion_CelsiusToFahrenheit()
        {
            var source = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius, converter);

            var converted = source.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.AreEqual(122.0, converted.Value, Precision);
        }

        [TestMethod]
        public void TestTemperatureConversion_FahrenheitToCelsius()
        {
            var source = new Quantity<TemperatureUnit>(122.0, TemperatureUnit.Fahrenheit, converter);

            var converted = source.ConvertTo(TemperatureUnit.Celsius);

            Assert.AreEqual(50.0, converted.Value, Precision);
        }

        [TestMethod]
        public void TestTemperatureConversion_RoundTrip_PreservesValue()
        {
            double original = 75.5;

            var start = new Quantity<TemperatureUnit>(original, TemperatureUnit.Fahrenheit, converter);

            var toC = start.ConvertTo(TemperatureUnit.Celsius);
            var backToF = toC.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.AreEqual(original, backToF.Value, Precision);
        }

        [TestMethod]
        public void TestTemperatureConversion_SameUnit()
        {
            var temp = new Quantity<TemperatureUnit>(25.0, TemperatureUnit.Celsius, converter);

            var result = temp.ConvertTo(TemperatureUnit.Celsius);

            Assert.AreEqual(25.0, result.Value, Precision);
        }

        [TestMethod]
        public void TestTemperatureConversion_ZeroValue()
        {
            var temp = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius, converter);

            var converted = temp.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.AreEqual(32.0, converted.Value, Precision);
        }

        [TestMethod]
        public void TestTemperatureOperation_Add_ShouldReturnCorrectSum()
        {
            var first = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius, converter);
            var second = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius, converter);

            var sum = first.Add(second);

            Assert.AreEqual(150.0, sum.Value);
            Assert.AreEqual(TemperatureUnit.Celsius, sum.Unit);
        }

        [TestMethod]
        public void TestTemperatureDivide_ShouldReturnCorrectRatio()
        {
            var first = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius, converter);
            var second = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius, converter);

            double ratio = first.Divide(second);

            Assert.AreEqual(2.0, ratio, Precision);
        }

        [TestMethod]
        public void TestTemperatureUnit_NameMethod()
        {
            Assert.AreEqual("Celsius", TemperatureUnit.Celsius.ToString());
            Assert.AreEqual("°C", converter.GetSymbol(TemperatureUnit.Celsius));
            Assert.AreEqual("°F", converter.GetSymbol(TemperatureUnit.Fahrenheit));
        }

        [TestMethod]
        public void TestTemperatureUnit_ConversionFactor()
        {
            double baseVal = converter.ConvertToBase(TemperatureUnit.Celsius, 1.0);

            Assert.AreEqual(1.0, baseVal);
        }
    }
}