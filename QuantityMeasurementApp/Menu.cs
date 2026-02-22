using QuantityMeasurementApp.Business.Interfaces;
using QuantityMeasurementApp.Business.Services;
using QuantityMeasurementApp.Model;
namespace QuantityMeasurementApp
{
    internal class Menu
    {
        public static void StartApp()
        {

            IQuantityLength utility = new QuantityLengthUtility();

            Console.Write("Enter First Value: ");
            double value1 = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter Unit (Feet/Inch/Yard/Centimeter): ");
            LengthUnit unit1 = (LengthUnit)Enum.Parse(typeof(LengthUnit), Console.ReadLine(), true);

            Console.Write("Enter Second Value: ");
            double value2 = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter Unit (Feet/Inch/Yard/Centimeter): ");
            LengthUnit unit2 = (LengthUnit)Enum.Parse(typeof(LengthUnit),Console.ReadLine(),true);
            bool result = utility.CheckEquality(value1, unit1, value2, unit2);
            Console.WriteLine("Equality Result: " + result);

        }
    }
}