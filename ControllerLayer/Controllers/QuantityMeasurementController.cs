using BusinessLayer.Interfaces;
using ControllerLayer.Interfaces;
using ModelLayer.DTOs;

namespace ControllerLayer.Controllers
{
    /// <summary>
    /// Facade controller for the Quantity Measurement application.
    ///
    /// UC15 responsibilities:
    ///  - Receive QuantityDTO requests from the Menu (or future REST endpoints).
    ///  - Delegate every operation to the injected IQuantityMeasurementService.
    ///  - Return QuantityDTO results — it never performs computation itself.
    ///  - Contain no Console I/O (that belongs to the Menu / presentation layer).
    ///
    /// Design patterns applied:
    ///  - Facade      : simplifies the service API for the presentation layer.
    ///  - Dependency Injection : service injected via constructor (loosely coupled).
    ///
    /// Future extension: annotate these methods with [HttpPost] / [HttpGet] to
    /// expose them as REST endpoints with minimal code changes.
    ///   POST /api/quantity/convert
    ///   POST /api/quantity/compare
    ///   POST /api/quantity/add
    ///   POST /api/quantity/subtract
    ///   POST /api/quantity/divide
    /// </summary>
    public class QuantityMeasurementController : IQuantityMeasurementController
    {
        private readonly IQuantityMeasurementService _service;

        public QuantityMeasurementController(IQuantityMeasurementService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public QuantityDTO PerformConversion(QuantityDTO input, string targetUnitName)
            => _service.Convert(input, targetUnitName);

        public QuantityDTO PerformComparison(QuantityDTO first, QuantityDTO second)
            => _service.Compare(first, second);

        public QuantityDTO PerformAddition(QuantityDTO first, QuantityDTO second)
            => _service.Add(first, second);

        public QuantityDTO PerformSubtraction(QuantityDTO first, QuantityDTO second)
            => _service.Subtract(first, second);

        public QuantityDTO PerformDivision(QuantityDTO first, QuantityDTO second)
            => _service.Divide(first, second);
    }
}
