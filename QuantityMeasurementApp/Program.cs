using QuantityMeasurementApp.Config;
using QuantityMeasurementApp.Factories;
using QuantityMeasurementApp.Interfaces;
// using RepoLayer.Database;

namespace QuantityMeasurementApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string connectionString = AppConfig.GetConnectionString();

            DatabaseInitializer.Initialize(connectionString);

            IMenuFactory factory = new MenuFactory();

            IQuantityMeasurementAppMenu menu = factory.CreateMenu();

            menu.Run();
        }
    }
}