using System;
using System.Linq;
using Moq;
using ModelLayer.DTOs;
using ModelLayer.Entities;
using ModelLayer.Enums;
using ModelLayer.Helpers;
using ModelLayer.Models;
using BusinessLayer.Helpers;
using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using ControllerLayer.Controllers;
using RepoLayer.Interfaces;
using RepoLayer.Repositories;

namespace QuantityMeasurementApp.Tests
{
    // ── 1. UnitConverter ─────────────────────────────────────────

    [TestClass]
    public class UnitConverterTests
    {
        [TestMethod]
        public void ToBase_Feet_ReturnsInches()
            => Assert.AreEqual(12.0, UnitConverter.ToBase(LengthUnit.Feet, 1.0), 1e-6);

        [TestMethod]
        public void FromBase_Feet_ReturnsCorrect()
            => Assert.AreEqual(1.0, UnitConverter.FromBase(LengthUnit.Feet, 12.0), 1e-6);

        [TestMethod]
        public void ToBase_Fahrenheit_ToCelsius()
            => Assert.AreEqual(0.0, UnitConverter.ToBase(TemperatureUnit.Fahrenheit, 32.0), 1e-6);

        [TestMethod]
        public void GetSymbol_ReturnsCorrect()
        {
            Assert.AreEqual("ft", UnitConverter.GetSymbol(LengthUnit.Feet));
            Assert.AreEqual("Kg", UnitConverter.GetSymbol(WeightUnit.Kilograms));
            Assert.AreEqual("°C", UnitConverter.GetSymbol(TemperatureUnit.Celsius));
        }

        [TestMethod]
        public void ParseUnit_ValidName_ReturnsEnum()
            => Assert.AreEqual(LengthUnit.Feet, UnitConverter.ParseUnit<LengthUnit>("Feet"));

        [TestMethod]
        public void ParseUnit_InvalidName_Throws()
        {
            try { UnitConverter.ParseUnit<LengthUnit>("Bananas"); Assert.Fail("Expected exception"); }
            catch (ArgumentException) { }
        }
    }

    // ── 2. QuantityDTO ────────────────────────────────────────────

    [TestClass]
    public class QuantityDtoTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dto = new QuantityDTO(12.0, "Inches", "Length");
            Assert.AreEqual(12.0,     dto.Value);
            Assert.AreEqual("Inches", dto.UnitName);
            Assert.AreEqual("Length", dto.Category);
            Assert.IsFalse(dto.IsError);
        }

        [TestMethod]
        public void Error_Factory_SetsErrorState()
        {
            var err = QuantityDTO.Error("bad input");
            Assert.IsTrue(err.IsError);
            Assert.AreEqual("bad input", err.ErrorMessage);
        }

        [TestMethod]
        public void ToString_NonError_ShowsValueAndUnit()
            => Assert.AreEqual("12 Inches", new QuantityDTO(12, "Inches", "Length").ToString());

        [TestMethod]
        public void ToString_Error_ShowsErrorPrefix()
            => Assert.IsTrue(QuantityDTO.Error("oops").ToString().StartsWith("Error:"));
    }

    // ── 3. QuantityMeasurementEntity ─────────────────────────────

    [TestClass]
    public class EntityTests
    {
        private static QuantityDTO D(double v, string u, string c) => new(v, u, c);

        [TestMethod]
        public void SingleOperand_StoresData()
        {
            var e = new QuantityMeasurementEntity(D(1,"Feet","Length"), "Convert", D(12,"Inches","Length"));
            Assert.AreEqual("Convert", e.OperationType);
            Assert.IsNull(e.Operand2);
            Assert.IsFalse(e.HasError);
        }

        [TestMethod]
        public void DualOperand_StoresData()
        {
            var e = new QuantityMeasurementEntity(D(1,"Feet","Length"), D(12,"Inches","Length"), "Compare", D(1,"Boolean","Length"));
            Assert.IsNotNull(e.Operand2);
            Assert.AreEqual("Compare", e.OperationType);
        }

        [TestMethod]
        public void Error_SetsHasError()
        {
            var e = new QuantityMeasurementEntity(D(1,"Feet","Length"), null, "Add", "some error");
            Assert.IsTrue(e.HasError);
            Assert.AreEqual("some error", e.ErrorMessage);
        }
    }

    // ── 4. QuantityCalculator ─────────────────────────────────────

    [TestClass]
    public class QuantityCalculatorTests
    {
        [TestMethod]
        public void AreEqual_SameValue_DifferentUnit_True()
            => Assert.IsTrue(QuantityCalculator.AreEqual(
                new Quantity<LengthUnit>(1,  LengthUnit.Feet),
                new Quantity<LengthUnit>(12, LengthUnit.Inches)));

        [TestMethod]
        public void AreEqual_DifferentValues_False()
            => Assert.IsFalse(QuantityCalculator.AreEqual(
                new Quantity<LengthUnit>(1, LengthUnit.Feet),
                new Quantity<LengthUnit>(1, LengthUnit.Inches)));

        [TestMethod]
        public void ConvertTo_FeetToInches_Correct()
        {
            var r = QuantityCalculator.ConvertTo(
                new Quantity<LengthUnit>(1, LengthUnit.Feet), LengthUnit.Inches);
            Assert.AreEqual(12.0, r.Value, 1e-6);
        }

        [TestMethod]
        public void Add_TwoLengths_CorrectSum()
        {
            var r = QuantityCalculator.Add(
                new Quantity<LengthUnit>(1, LengthUnit.Feet),
                new Quantity<LengthUnit>(12, LengthUnit.Inches),
                LengthUnit.Inches);
            Assert.AreEqual(24.0, r.Value, 1e-6);
        }

        [TestMethod]
        public void Subtract_Lengths_Correct()
        {
            var r = QuantityCalculator.Subtract(
                new Quantity<LengthUnit>(10, LengthUnit.Feet),
                new Quantity<LengthUnit>(6,  LengthUnit.Inches),
                LengthUnit.Inches);
            Assert.AreEqual(114.0, r.Value, 1e-4);
        }

        [TestMethod]
        public void Divide_Lengths_CorrectRatio()
            => Assert.AreEqual(2.0, QuantityCalculator.Divide(
                new Quantity<LengthUnit>(1, LengthUnit.Feet),
                new Quantity<LengthUnit>(6, LengthUnit.Inches)), 1e-6);

        [TestMethod]
        public void Divide_ByZero_Throws()
        {
            try { QuantityCalculator.Divide(new Quantity<LengthUnit>(1, LengthUnit.Feet), new Quantity<LengthUnit>(0, LengthUnit.Feet)); Assert.Fail("Expected exception"); }
            catch (ArithmeticException) { }
        }

        [TestMethod]
        public void Divide_Temperature_Throws()
        {
            try { QuantityCalculator.Divide(new Quantity<TemperatureUnit>(100, TemperatureUnit.Celsius), new Quantity<TemperatureUnit>(50, TemperatureUnit.Celsius)); Assert.Fail("Expected exception"); }
            catch (InvalidOperationException) { }
        }
    }

    // ── 5. Service with Mocked Repository ────────────────────────

    [TestClass]
    public class ServiceDtoTests
    {
        private IQuantityMeasurementService Build(out Mock<IQuantityRepository> mock)
        {
            mock = new Mock<IQuantityRepository>();
            return new QuantityMeasurementService(mock.Object);
        }

        [TestMethod]
        public void Convert_FeetToInches_Correct()
        {
            var svc = Build(out _);
            var r   = svc.Convert(new QuantityDTO(1, "Feet", "Length"), "Inches");
            Assert.IsFalse(r.IsError);
            Assert.AreEqual(12.0, r.Value, 1e-6);
        }

        [TestMethod]
        public void Compare_EqualLengths_ReturnsOne()
        {
            var svc = Build(out _);
            var r   = svc.Compare(new QuantityDTO(1,"Feet","Length"), new QuantityDTO(12,"Inches","Length"));
            Assert.AreEqual(1, (int)r.Value);
        }

        [TestMethod]
        public void Add_TwoLengths_NoError()
        {
            var svc = Build(out _);
            var r   = svc.Add(new QuantityDTO(1,"Feet","Length"), new QuantityDTO(12,"Inches","Length"));
            Assert.IsFalse(r.IsError);
        }

        [TestMethod]
        public void Subtract_Lengths_NoError()
        {
            var svc = Build(out _);
            var r   = svc.Subtract(new QuantityDTO(2,"Feet","Length"), new QuantityDTO(6,"Inches","Length"));
            Assert.IsFalse(r.IsError);
        }

        [TestMethod]
        public void Divide_Lengths_CorrectRatio()
        {
            var svc = Build(out _);
            var r   = svc.Divide(new QuantityDTO(1,"Feet","Length"), new QuantityDTO(6,"Inches","Length"));
            Assert.IsFalse(r.IsError);
            Assert.AreEqual(2.0, r.Value, 1e-6);
        }

        [TestMethod]
        public void Convert_UnknownCategory_ReturnsError()
        {
            var svc = Build(out _);
            var r   = svc.Convert(new QuantityDTO(1, "Feet", "Banana"), "Inches");
            Assert.IsTrue(r.IsError);
        }

        [TestMethod]
        public void Compare_CrossCategory_ReturnsError()
        {
            var svc = Build(out _);
            var r   = svc.Compare(new QuantityDTO(1,"Feet","Length"), new QuantityDTO(1,"Grams","Weight"));
            Assert.IsTrue(r.IsError);
        }

        [TestMethod]
        public void Convert_SavesEntityToRepository()
        {
            var svc = Build(out var mock);
            svc.Convert(new QuantityDTO(1, "Feet", "Length"), "Inches");
            mock.Verify(r => r.Save(It.IsAny<QuantityMeasurementEntity>()), Times.Once);
        }
    }

    // ── 6. Controller with Mocked Service ────────────────────────

    [TestClass]
    public class ControllerTests
    {
        private (QuantityMeasurementController ctrl, Mock<IQuantityMeasurementService> mock) Build()
        {
            var mock = new Mock<IQuantityMeasurementService>();
            return (new QuantityMeasurementController(mock.Object), mock);
        }

        [TestMethod]
        public void PerformConversion_DelegatesToService()
        {
            var (ctrl, svc) = Build();
            var dto = new QuantityDTO(1, "Feet", "Length");
            svc.Setup(s => s.Convert(dto, "Inches")).Returns(new QuantityDTO(12, "Inches", "Length"));
            Assert.AreEqual(12.0, ctrl.PerformConversion(dto, "Inches").Value);
        }

        [TestMethod]
        public void PerformComparison_DelegatesToService()
        {
            var (ctrl, svc) = Build();
            var a = new QuantityDTO(1,  "Feet",   "Length");
            var b = new QuantityDTO(12, "Inches", "Length");
            svc.Setup(s => s.Compare(a, b)).Returns(new QuantityDTO(1, "Boolean", "Length"));
            Assert.AreEqual(1.0, ctrl.PerformComparison(a, b).Value);
        }

        [TestMethod]
        public void Constructor_NullService_Throws()
        {
            try { new QuantityMeasurementController(null!); Assert.Fail("Expected exception"); }
            catch (ArgumentNullException) { }
        }
    }

    // ── 7. In-Memory Repository ───────────────────────────────────

    [TestClass]
    public class InMemoryRepositoryTests
    {
        private QuantityRepository FreshRepo()
        {
            var field = typeof(QuantityRepository)
                .GetField("_instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!;
            field.SetValue(null, null);
            return QuantityRepository.Instance;
        }

        private static QuantityMeasurementEntity MakeEntity(string op = "Convert", string cat = "Length")
        {
            var d1 = new QuantityDTO(1,  "Feet",   cat);
            var d2 = new QuantityDTO(12, "Inches", cat);
            return new QuantityMeasurementEntity(d1, d2, op, d2);
        }

        [TestMethod]
        public void Save_And_GetAll_Works()
        {
            var repo = FreshRepo();
            repo.Save(MakeEntity());
            Assert.AreEqual(1, repo.GetTotalCount());
        }

        [TestMethod]
        public void GetByOperation_FiltersCorrectly()
        {
            var repo = FreshRepo();
            repo.Save(MakeEntity("Convert"));
            repo.Save(MakeEntity("Add"));
            Assert.AreEqual(1, repo.GetByOperation("Convert").Count);
        }

        [TestMethod]
        public void GetByCategory_FiltersCorrectly()
        {
            var repo = FreshRepo();
            repo.Save(MakeEntity("Convert", "Length"));
            repo.Save(MakeEntity("Convert", "Weight"));
            Assert.AreEqual(1, repo.GetByCategory("Length").Count);
        }

        [TestMethod]
        public void DeleteAll_ClearsCache()
        {
            var repo = FreshRepo();
            repo.Save(MakeEntity());
            repo.Save(MakeEntity());
            repo.DeleteAll();
            Assert.AreEqual(0, repo.GetTotalCount());
        }

        [TestMethod]
        public void GetTotalCount_ReturnsCorrect()
        {
            var repo = FreshRepo();
            repo.Save(MakeEntity());
            repo.Save(MakeEntity());
            Assert.AreEqual(2, repo.GetTotalCount());
        }

        [TestMethod]
        public void Save_Null_Throws()
        {
            try { FreshRepo().Save(null!); Assert.Fail("Expected exception"); }
            catch (ArgumentNullException) { }
        }
    }

    // ── 8. Backward-Compat UC1–UC14 ──────────────────────────────

    [TestClass]
    public class BackwardCompatibilityTests
    {
        private readonly IQuantityMeasurementService _svc =
            new QuantityMeasurementService(QuantityRepository.Instance);

        [TestMethod]
        public void UC1_FeetEqualsTwelveInches()
            => Assert.IsTrue(_svc.Compare(
                new Quantity<LengthUnit>(1, LengthUnit.Feet),
                new Quantity<LengthUnit>(12, LengthUnit.Inches)));

        [TestMethod]
        public void UC2_ConvertFeetToInches()
        {
            var r = _svc.DemonstrateConversion(
                new Quantity<LengthUnit>(1, LengthUnit.Feet), LengthUnit.Inches);
            Assert.AreEqual(12.0, r.Value, 1e-6);
        }

        [TestMethod]
        public void UC3_AddFeetAndInches_ReturnsInches()
        {
            var r = _svc.DemonstrateAddition(
                new Quantity<LengthUnit>(1, LengthUnit.Feet),
                new Quantity<LengthUnit>(12, LengthUnit.Inches),
                LengthUnit.Inches);
            Assert.AreEqual(24.0, r.Value, 1e-6);
        }

        [TestMethod]
        public void UC7_KilogramsEqualsGrams()
            => Assert.IsTrue(_svc.Compare(
                new Quantity<WeightUnit>(1,    WeightUnit.Kilograms),
                new Quantity<WeightUnit>(1000, WeightUnit.Grams)));

        [TestMethod]
        public void UC9_LitreEqualsMilliLiter()
            => Assert.IsTrue(_svc.Compare(
                new Quantity<VolumeUnit>(1,    VolumeUnit.Litre),
                new Quantity<VolumeUnit>(1000, VolumeUnit.MilliLiter)));

        [TestMethod]
        public void UC14_CelsiusEqualsFahrenheit()
            => Assert.IsTrue(_svc.Compare(
                new Quantity<TemperatureUnit>(0,  TemperatureUnit.Celsius),
                new Quantity<TemperatureUnit>(32, TemperatureUnit.Fahrenheit)));

        [TestMethod]
        public void UC13_SubtractLength_Correct()
        {
            var r = _svc.Subtract(
                new Quantity<LengthUnit>(10, LengthUnit.Feet),
                new Quantity<LengthUnit>(6,  LengthUnit.Inches),
                LengthUnit.Inches);
            Assert.AreEqual(114.0, r.Value, 1e-4);
        }

        [TestMethod]
        public void UC12_DivideLength_CorrectRatio()
            => Assert.AreEqual(2.0, _svc.Divide(
                1.0, LengthUnit.Feet, 6.0, LengthUnit.Inches), 1e-6);

        [TestMethod]
        public void OneFootNotEqualsTwoFeet()
            => Assert.IsFalse(_svc.Compare(
                new Quantity<LengthUnit>(1, LengthUnit.Feet),
                new Quantity<LengthUnit>(2, LengthUnit.Feet)));
    }
}