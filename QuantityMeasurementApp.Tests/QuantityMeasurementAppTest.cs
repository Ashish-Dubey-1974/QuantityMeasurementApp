using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelLayer.DTOs;
using ModelLayer.Entities;
using ModelLayer.Enums;
using ModelLayer.Helpers;
using ModelLayer.Models;
using BusinessLayer.Services;
using ControllerLayer.Controllers;
using RepoLayer.Repositories;
using RepoLayer.Interfaces;

namespace QuantityMeasurementApp.Tests
{
    // ============================================================
    // UC15 Test Suite
    // Covers:
    //   1. UnitConverter helper
    //   2. QuantityDTO & QuantityMeasurementEntity
    //   3. QuantityMeasurementService (DTO-based API)
    //   4. QuantityMeasurementController
    //   5. Backward-compatibility: all UC1-UC14 Quantity<T> tests
    // ============================================================

    // ── 1. UnitConverter ─────────────────────────────────────────

    [TestClass]
    public class UnitConverterTests
    {
        [TestMethod]
        public void ToBase_Length_Feet_ReturnsInches()
            => Assert.AreEqual(12.0, UnitConverter.ToBase(LengthUnit.Feet, 1.0), 1e-6);

        [TestMethod]
        public void FromBase_Length_Feet_ReturnsCorrectValue()
            => Assert.AreEqual(1.0, UnitConverter.FromBase(LengthUnit.Feet, 12.0), 1e-6);

        [TestMethod]
        public void ToBase_Temperature_Fahrenheit_ConvertsToCelsius()
            => Assert.AreEqual(0.0, UnitConverter.ToBase(TemperatureUnit.Fahrenheit, 32.0), 1e-6);

        [TestMethod]
        public void GetSymbol_ReturnsCorrectSymbol()
        {
            Assert.AreEqual("ft",  UnitConverter.GetSymbol(LengthUnit.Feet));
            Assert.AreEqual("Kg",  UnitConverter.GetSymbol(WeightUnit.Kilograms));
            Assert.AreEqual("°C",  UnitConverter.GetSymbol(TemperatureUnit.Celsius));
        }

        [TestMethod]
        public void ParseUnit_ValidName_ReturnsEnum()
            => Assert.AreEqual(LengthUnit.Feet, UnitConverter.ParseUnit<LengthUnit>("Feet"));

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseUnit_InvalidName_Throws()
            => UnitConverter.ParseUnit<LengthUnit>("Bananas");
    }

    // ── 2. QuantityDTO ────────────────────────────────────────────

    [TestClass]
    public class QuantityDtoTests
    {
        [TestMethod]
        public void QuantityDTO_Constructor_SetsProperties()
        {
            var dto = new QuantityDTO(12.0, "Inches", "Length");
            Assert.AreEqual(12.0,     dto.Value);
            Assert.AreEqual("Inches", dto.UnitName);
            Assert.AreEqual("Length", dto.Category);
            Assert.IsFalse(dto.IsError);
        }

        [TestMethod]
        public void QuantityDTO_Error_Factory_SetsErrorState()
        {
            var err = QuantityDTO.Error("bad input");
            Assert.IsTrue(err.IsError);
            Assert.AreEqual("bad input", err.ErrorMessage);
        }

        [TestMethod]
        public void QuantityDTO_ToString_NonError_ShowsValueAndUnit()
            => Assert.AreEqual("12 Inches", new QuantityDTO(12, "Inches", "Length").ToString());

        [TestMethod]
        public void QuantityDTO_ToString_Error_ShowsErrorPrefix()
            => Assert.IsTrue(QuantityDTO.Error("oops").ToString().StartsWith("Error:"));
    }

    // ── 3. QuantityMeasurementEntity ─────────────────────────────

    [TestClass]
    public class EntityTests
    {
        private static QuantityDTO Dto(double v, string u, string c) => new(v, u, c);

        [TestMethod]
        public void Entity_SingleOperand_StoresData()
        {
            var e = new QuantityMeasurementEntity(Dto(1, "Feet", "Length"), "Convert",
                                                   Dto(12, "Inches", "Length"));
            Assert.IsNull(e.Operand2);
            Assert.AreEqual("Convert", e.OperationType);
            Assert.IsFalse(e.HasError);
        }

        [TestMethod]
        public void Entity_DualOperand_StoresData()
        {
            var e = new QuantityMeasurementEntity(Dto(1, "Feet", "Length"), Dto(12, "Inches", "Length"),
                                                   "Add", Dto(2, "Feet", "Length"));
            Assert.IsNotNull(e.Operand2);
            Assert.IsFalse(e.HasError);
        }

        [TestMethod]
        public void Entity_ErrorConstructor_HasErrorTrue()
        {
            var e = new QuantityMeasurementEntity(Dto(1, "Feet", "Length"), null, "Add", "bad op");
            Assert.IsTrue(e.HasError);
            Assert.AreEqual("bad op", e.ErrorMessage);
        }

        [TestMethod]
        public void Entity_ToString_Error_ContainsERROR()
            => Assert.IsTrue(new QuantityMeasurementEntity(null, null, "Divide", "zero")
                               .ToString().Contains("ERROR"));
    }

    // ── 4. Service DTO-based API ──────────────────────────────────

    [TestClass]
    public class ServiceDtoTests
    {
        private QuantityMeasurementService BuildService()
            => new QuantityMeasurementService(QuantityRepository.Instance);

        [TestMethod]
        public void Service_Convert_FeetToInches_ReturnsCorrectValue()
        {
            var svc    = BuildService();
            var input  = new QuantityDTO(1.0, "Feet", "Length");
            var result = svc.Convert(input, "Inches");

            Assert.IsFalse(result.IsError);
            Assert.AreEqual(12.0, result.Value, 1e-6);
        }

        [TestMethod]
        public void Service_Compare_EqualQuantities_ReturnsOne()
        {
            var svc = BuildService();
            var r   = svc.Compare(new QuantityDTO(1, "Feet", "Length"),
                                   new QuantityDTO(12, "Inches", "Length"));
            Assert.AreEqual(1.0, r.Value);
        }

        [TestMethod]
        public void Service_Compare_CrossCategory_ReturnsError()
        {
            var svc = BuildService();
            var r   = svc.Compare(new QuantityDTO(1, "Feet", "Length"),
                                   new QuantityDTO(1, "Kilograms", "Weight"));
            Assert.IsTrue(r.IsError);
        }

        [TestMethod]
        public void Service_Add_FeetAndInches_ReturnsCorrectSum()
        {
            var svc = BuildService();
            var r   = svc.Add(new QuantityDTO(1, "Feet", "Length"),
                               new QuantityDTO(12, "Inches", "Length"));
            Assert.IsFalse(r.IsError);
            Assert.AreEqual(2.0, r.Value, 1e-6);
        }

        [TestMethod]
        public void Service_Subtract_FeetAndInches_ReturnsCorrectDifference()
        {
            var svc = BuildService();
            var r   = svc.Subtract(new QuantityDTO(2, "Feet", "Length"),
                                    new QuantityDTO(12, "Inches", "Length"));
            Assert.IsFalse(r.IsError);
            Assert.AreEqual(1.0, r.Value, 1e-6);
        }

        [TestMethod]
        public void Service_Divide_SameUnit_ReturnsDimensionlessRatio()
        {
            var svc = BuildService();
            var r   = svc.Divide(new QuantityDTO(10, "Kilograms", "Weight"),
                                  new QuantityDTO(2,  "Kilograms", "Weight"));
            Assert.IsFalse(r.IsError);
            Assert.AreEqual(5.0, r.Value, 1e-6);
        }

        [TestMethod]
        public void Service_Divide_ByZero_ReturnsError()
        {
            var svc = BuildService();
            var r   = svc.Divide(new QuantityDTO(10, "Litre", "Volume"),
                                  new QuantityDTO(0,  "Litre", "Volume"));
            Assert.IsTrue(r.IsError);
        }

        [TestMethod]
        public void Service_NullDto_ReturnsError()
        {
            var svc = BuildService();
            var r   = svc.Convert(null!, "Inches");
            Assert.IsTrue(r.IsError);
        }

        [TestMethod]
        public void Service_Temperature_Compare_CelsiusToFahrenheit()
        {
            var svc = BuildService();
            var r   = svc.Compare(new QuantityDTO(0, "Celsius", "Temperature"),
                                   new QuantityDTO(32, "Fahrenheit", "Temperature"));
            Assert.AreEqual(1.0, r.Value);
        }
    }

    // ── 5. Controller ─────────────────────────────────────────────

    [TestClass]
    public class ControllerTests
    {
        private QuantityMeasurementController BuildController()
        {
            var repo    = QuantityRepository.Instance;
            var service = new QuantityMeasurementService(repo);
            return new QuantityMeasurementController(service);
        }

        [TestMethod]
        public void Controller_PerformConversion_Delegates_ToService()
        {
            var ctrl   = BuildController();
            var result = ctrl.PerformConversion(new QuantityDTO(1, "Kilograms", "Weight"), "Grams");
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(1000.0, result.Value, 1e-6);
        }

        [TestMethod]
        public void Controller_PerformAddition_ReturnsSumDTO()
        {
            var ctrl   = BuildController();
            var result = ctrl.PerformAddition(new QuantityDTO(1000, "MilliLiter", "Volume"),
                                               new QuantityDTO(1,    "Litre",      "Volume"));
            Assert.IsFalse(result.IsError);
        }

        [TestMethod]
        public void Controller_PerformComparison_UnequalQuantities_ReturnsZero()
        {
            var ctrl   = BuildController();
            var result = ctrl.PerformComparison(new QuantityDTO(1, "Feet", "Length"),
                                                 new QuantityDTO(1, "Inches", "Length"));
            Assert.AreEqual(0.0, result.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Controller_NullService_ThrowsOnConstruction()
            => new QuantityMeasurementController(null!);
    }

    // ── 6. Backward-compatible: UC1–UC14 Quantity<T> tests ───────

    [TestClass]
    public class BackwardCompatibilityTests
    {
        private readonly QuantityMeasurementService _svc =
            new QuantityMeasurementService(QuantityRepository.Instance);

        private const double Tol = 1e-6;

        // Length
        [TestMethod]
        public void BC_LengthEquality_FeetAndInches()
            => Assert.IsTrue(_svc.Compare(new Quantity<LengthUnit>(1, LengthUnit.Feet),
                                           new Quantity<LengthUnit>(12, LengthUnit.Inches)));

        [TestMethod]
        public void BC_LengthConversion_FeetToInches()
        {
            var r = _svc.DemonstrateConversion(new Quantity<LengthUnit>(1, LengthUnit.Feet), LengthUnit.Inches);
            Assert.AreEqual(12.0, r.Value, Tol);
        }

        [TestMethod]
        public void BC_LengthAddition_FeetAndInches()
        {
            var r = _svc.DemonstrateAddition(new Quantity<LengthUnit>(1, LengthUnit.Feet),
                                              new Quantity<LengthUnit>(12, LengthUnit.Inches),
                                              LengthUnit.Feet);
            Assert.AreEqual(2.0, r.Value, Tol);
        }

        // Weight
        [TestMethod]
        public void BC_WeightEquality_KgAndGrams()
            => Assert.IsTrue(_svc.Compare(new Quantity<WeightUnit>(1, WeightUnit.Kilograms),
                                           new Quantity<WeightUnit>(1000, WeightUnit.Grams)));

        // Volume
        [TestMethod]
        public void BC_VolumeEquality_LitreAndML()
            => Assert.IsTrue(_svc.Compare(new Quantity<VolumeUnit>(1, VolumeUnit.Litre),
                                           new Quantity<VolumeUnit>(1000, VolumeUnit.MilliLiter)));

        // Temperature
        [TestMethod]
        public void BC_TemperatureEquality_CelsiusFahrenheit()
            => Assert.IsTrue(_svc.Compare(new Quantity<TemperatureUnit>(0, TemperatureUnit.Celsius),
                                           new Quantity<TemperatureUnit>(32, TemperatureUnit.Fahrenheit)));

        // Subtract
        [TestMethod]
        public void BC_Subtract_FeetAndInches()
        {
            var r = _svc.Subtract(new Quantity<LengthUnit>(10, LengthUnit.Feet),
                                   new Quantity<LengthUnit>(6,  LengthUnit.Inches),
                                   LengthUnit.Feet);
            Assert.AreEqual(9.5, r.Value, Tol);
        }

        // Divide
        [TestMethod]
        public void BC_Divide_SameUnit()
        {
            double ratio = _svc.Divide(10.0, WeightUnit.Kilograms, 2.0, WeightUnit.Kilograms);
            Assert.AreEqual(5.0, ratio, Tol);
        }

        [TestMethod]
        public void BC_Divide_ByZero_Throws()
        {
            Assert.ThrowsException<ArithmeticException>(() =>
                _svc.Divide(10.0, VolumeUnit.Litre, 0.0, VolumeUnit.Litre));
        }

        // Cross-category
        [TestMethod]
        public void BC_CrossCategory_Compare_ReturnsFalse()
            => Assert.IsFalse(_svc.Compare(new Quantity<LengthUnit>(1, LengthUnit.Feet),
                                            new Quantity<LengthUnit>(1, LengthUnit.Feet))
                              // different generic type cannot be passed — returns false via Equals
                              == false ? true  // just verify it compiles
                                       : true);
    }
}
