using QuantityMeasurementApp.Menu;

namespace QuantityMeasurementApp
{
    /// <summary>
    /// Entry point of the Quantity Measurement application.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Starts the application and runs the main menu.
        /// </summary>
        private static void Main(string[] args)
        {
            var menu = new QuantityMeasurementAppMenu();
            menu.Run();
        }
    }
}