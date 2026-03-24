using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Menu;

namespace QuantityMeasurementApp.Factories
{
    public class MenuFactory : IMenuFactory
    {
        public IQuantityMeasurementAppMenu CreateMenu()
        {
            return new QuantityMeasurementAppMenu();
        }
    }
}