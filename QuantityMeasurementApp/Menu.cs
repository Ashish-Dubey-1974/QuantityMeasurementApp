using QuantityMeasurementApp.Business.Interfaces;
using QuantityMeasurementApp.Business.Services;
namespace QuantityMeasurementApp
{
    internal class Menu
    {
        public static void StartApp()
        {
            int input;
            do
            {
                Console.WriteLine("\nPress 0. To Exit");
                Console.WriteLine("Press 1. For checking Feet Equality");
                while(!int.TryParse(Console.ReadLine(),out input))Console.WriteLine("Invalid Input : ");
                if(input == 0)return;
                if (input == 1)
                {
                    IFeet obj = new FeetUtility();
                    obj.CompareFeet();
                }
            }while(true);
        }
    }
}