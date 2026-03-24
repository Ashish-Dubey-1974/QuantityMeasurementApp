using System;
using BusinessLayer.Services;
using ControllerLayer.Controllers;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Menu;
using RepoLayer.Interfaces;
using RepoLayer.Repositories;

namespace QuantityMeasurementApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IQuantityRepository repository = CreateRepository();

            var service    = new QuantityMeasurementService(repository);
            var controller = new QuantityMeasurementController(service);
            IMenu menu     = new QuantityMeasurementAppMenu(controller);

            Console.WriteLine($"[UC16] Using repository: {repository.GetType().Name}");
            Console.WriteLine("[UC16] Database auto-created if not found.");

            menu.Run();

            Console.WriteLine($"\n[UC16] Total operations recorded: {repository.GetTotalCount()}");
            PrintHistory(repository);

            if (repository is IDisposable d)
                d.Dispose();
        }

        private static IQuantityRepository CreateRepository()
        {
            if (AppConfig.RepositoryType.Equals("cache", StringComparison.OrdinalIgnoreCase))
                return QuantityRepository.Instance;

            try
            {
                return new QuantityDatabaseRepository(AppConfig.ConnectionString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARN] DB unavailable ({ex.Message}). Falling back to in-memory cache.");
                return QuantityRepository.Instance;
            }
        }

        private static void PrintHistory(IQuantityRepository repo)
        {
            var all = repo.GetAll();
            if (all.Count == 0) return;
            Console.WriteLine("\n--- Operation History ---");
            foreach (var e in all)
                Console.WriteLine(e);
        }
    }
}
