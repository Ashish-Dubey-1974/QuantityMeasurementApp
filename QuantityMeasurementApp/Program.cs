using BusinessLayer.Services;
using ControllerLayer.Controllers;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Menu;
using RepoLayer.Repositories;

namespace QuantityMeasurementApp
{
    /// <summary>
    /// Application entry point for the Quantity Measurement system (UC15).
    ///
    /// UC15 responsibilities:
    ///  - Wire up all dependencies using the Factory pattern (manual DI).
    ///  - Hand control to IMenu.Run() and nothing else.
    ///  - Contain zero business logic.
    ///
    /// Dependency chain (bottom → top):
    ///   QuantityRepository (Singleton)
    ///       → QuantityMeasurementService
    ///           → QuantityMeasurementController
    ///               → QuantityMeasurementAppMenu   (implements IMenu)
    ///                   → Program.Main
    ///
    /// Dependency Injection: every layer receives its dependencies through
    /// constructor injection, making future framework integration (e.g. ASP.NET DI,
    /// Microsoft.Extensions.DependencyInjection) straightforward.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Factory: build and wire all layers
            var repository  = QuantityRepository.Instance;                    // Singleton
            var service     = new QuantityMeasurementService(repository);     // Business
            var controller  = new QuantityMeasurementController(service);     // Controller / Facade
            IMenu menu      = new QuantityMeasurementAppMenu(controller);     // Presentation

            // Delegate: entry point only starts the app, owns no logic
            menu.Run();
        }
    }
}
