using System;
using ModelLayer.Enums;
using ModelLayer.Models;
using BusinessLayer.Services;

namespace QuantityMeasurementApp.Menu
{
    public class QuantityMeasurementAppMenu
    {
        private readonly QuantityMeasurementService measurementService = new QuantityMeasurementService();

        public void Run()
        {
            bool terminateProgram = false;

            while (!terminateProgram)
            {
                Console.WriteLine("\n-----------------------");
                Console.WriteLine("Quantity Measurement App");
                Console.WriteLine("-----------------------");
                Console.WriteLine("1. Length Measurement");
                Console.WriteLine("2. Weight Measurement");
                Console.WriteLine("3. Volume Measurement");
                Console.WriteLine("4. Temperature Measurement");
                Console.WriteLine("5. Exit");

                string menuChoice = Console.ReadLine() ?? "";

                switch (menuChoice)
                {
                    case "1":
                        RunCategory<LengthUnit>("Length", "0:Inches, 1:Feet, 2:Yards, 3:CM");
                        break;

                    case "2":
                        RunCategory<WeightUnit>("Weight", "0:Grams, 1:Kilograms, 2:Pounds");
                        break;

                    case "3":
                        RunCategory<VolumeUnit>("Volume", "0:Liter, 1:MilliLiter, 2:Gallon");
                        break;

                    case "4":
                        RunCategory<TemperatureUnit>("Temperature", "0:Celsius, 1:Fahrenheit, 2:Kelvin");
                        break;

                    case "5":
                        terminateProgram = true;
                        break;

                    default:
                        Console.WriteLine("Invalid choice");
                        break;
                }
            }
        }

        /// <summary>
        /// Handles operations for a chosen measurement category.
        /// </summary>
        private void RunCategory<T>(string categoryTitle, string unitOptions) where T : struct, Enum
        {
            bool goBack = false;

            while (!goBack)
            {
                Console.WriteLine($"\n--- {categoryTitle} Measurement ---");
                Console.WriteLine("1. Conversion\n2. Comparison\n3. Addition\n4. subtraction\n5. Divide\n6. back");

                string actionChoice = Console.ReadLine() ?? "";

                switch (actionChoice)
                {
                    case "1":
                        HandleConversion<T>(unitOptions);
                        break;

                    case "2":
                        HandleComparison<T>(unitOptions);
                        break;

                    case "3":
                        HandleAddition<T>(unitOptions);
                        break;

                    case "4":
                        HandleSubtraction<T>(unitOptions);
                        break;

                    case "5":
                        HandleDivision<T>(unitOptions);
                        break;

                    case "6":
                        goBack = true;
                        break;

                    default:
                        Console.WriteLine("Invalid Choice");
                        break;
                }
            }
        }

        /// <summary>
        /// Calculates the ratio between two quantities.
        /// </summary>
        private void HandleDivision<T>(string unitOptions) where T : struct, Enum
        {
            try
            {
                Console.WriteLine(unitOptions);

                Console.Write("Value 1: ");
                double firstNumber = double.Parse(Console.ReadLine()!);

                Console.Write("Unit 1 Index: ");
                T firstUnit = (T)(object)int.Parse(Console.ReadLine()!);

                Console.Write("Value 2: ");
                double secondNumber = double.Parse(Console.ReadLine()!);

                Console.Write("Unit 2 Index: ");
                T secondUnit = (T)(object)int.Parse(Console.ReadLine()!);

                double ratio = measurementService.Divide(firstNumber, firstUnit, secondNumber, secondUnit);

                Console.WriteLine($"\nResult Ratio: {ratio} (Dimensionless)");
            }
            catch (ArithmeticException error)
            {
                Console.WriteLine("Math Error: " + error.Message);
            }
            catch (Exception error)
            {
                Console.WriteLine("Error: " + error.Message);
            }
        }

        /// <summary>
        /// Subtracts one quantity from another.
        /// </summary>
        private void HandleSubtraction<T>(string unitOptions) where T : struct, Enum
        {
            try
            {
                Console.WriteLine(unitOptions);

                Console.Write("Value 1: ");
                double firstValue = double.Parse(Console.ReadLine()!);

                Console.Write("Unit 1 Index: ");
                T firstUnit = (T)(object)int.Parse(Console.ReadLine()!);

                Console.Write("Value 2: ");
                double secondValue = double.Parse(Console.ReadLine()!);

                Console.Write("Unit 2 Index: ");
                T secondUnit = (T)(object)int.Parse(Console.ReadLine()!);

                Quantity<T> firstQuantity = new Quantity<T>(firstValue, firstUnit);
                Quantity<T> secondQuantity = new Quantity<T>(secondValue, secondUnit);

                Console.Write("Explicit target selection (Y/N): ");

                if ((Console.ReadLine() ?? "").ToLower() == "y")
                {
                    Console.Write("Target Unit Index: ");
                    T selectedUnit = (T)(object)int.Parse(Console.ReadLine()!);

                    Console.WriteLine($"Result: {measurementService.Subtract(firstQuantity, secondQuantity, selectedUnit)}");
                }
                else
                {
                    Console.WriteLine($"Result: {measurementService.Subtract(firstQuantity, secondQuantity, firstQuantity.Unit)}");
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Error: " + error.Message);
            }
        }

        private void HandleComparison<T>(string unitOptions) where T : struct, Enum
        {
            try
            {
                Console.WriteLine(unitOptions);

                Console.Write("Value 1: ");
                double firstNumber = double.Parse(Console.ReadLine()!);

                Console.Write("Unit 1 Index: ");
                T firstUnit = (T)(object)int.Parse(Console.ReadLine()!);

                Console.Write("Value 2: ");
                double secondNumber = double.Parse(Console.ReadLine()!);

                Console.Write("Unit 2 Index: ");
                T secondUnit = (T)(object)int.Parse(Console.ReadLine()!);

                Quantity<T> quantityA = new Quantity<T>(firstNumber, firstUnit);
                Quantity<T> quantityB = new Quantity<T>(secondNumber, secondUnit);

                bool areEqual = measurementService.Compare(quantityA, quantityB);

                Console.WriteLine($"\nResult: {quantityA} {(areEqual ? "==" : "!=")} {quantityB}");
            }
            catch (Exception error)
            {
                Console.WriteLine("Error: " + error.Message);
            }
        }

        private void HandleConversion<T>(string unitOptions) where T : struct, Enum
        {
            try
            {
                Console.WriteLine(unitOptions);

                Console.Write("Enter Value: ");
                double enteredValue = double.Parse(Console.ReadLine()!);

                Console.Write("Source Unit Index: ");
                T sourceUnit = (T)(object)int.Parse(Console.ReadLine()!);

                Console.Write("Target Unit Index: ");
                T targetUnit = (T)(object)int.Parse(Console.ReadLine()!);

                Quantity<T> initialQuantity = new Quantity<T>(enteredValue, sourceUnit);
                Quantity<T> convertedQuantity = measurementService.DemonstrateConversion(initialQuantity, targetUnit);

                Console.WriteLine($"Result: {convertedQuantity}");
            }
            catch (Exception error)
            {
                Console.WriteLine("Error: " + error.Message);
            }
        }

        private void HandleAddition<T>(string unitOptions) where T : struct, Enum
        {
            try
            {
                Console.WriteLine(unitOptions);

                Console.Write("Value 1: ");
                double firstNumber = double.Parse(Console.ReadLine()!);

                Console.Write("Unit 1 Index: ");
                T firstUnit = (T)(object)int.Parse(Console.ReadLine()!);

                Console.Write("Value 2: ");
                double secondNumber = double.Parse(Console.ReadLine()!);

                Console.Write("Unit 2 Index: ");
                T secondUnit = (T)(object)int.Parse(Console.ReadLine()!);

                Quantity<T> quantityOne = new Quantity<T>(firstNumber, firstUnit);
                Quantity<T> quantityTwo = new Quantity<T>(secondNumber, secondUnit);

                Console.Write("Explicit target selection (Y/N): ");

                if ((Console.ReadLine() ?? "").ToLower() == "y")
                {
                    Console.Write("Target Unit Index: ");
                    T chosenUnit = (T)(object)int.Parse(Console.ReadLine()!);

                    Console.WriteLine($"Result: {measurementService.DemonstrateAddition(quantityOne, quantityTwo, chosenUnit)}");
                }
                else
                {
                    Console.WriteLine($"Result: {measurementService.DemonstrateAddition(quantityOne, quantityTwo)}");
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Error: " + error.Message);
            }
        }
    }
}