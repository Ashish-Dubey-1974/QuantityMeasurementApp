using ControllerLayer.Interfaces;
using ModelLayer.DTOs;
using QuantityMeasurementApp.Interfaces;

namespace QuantityMeasurementApp.Menu
{
    /// <summary>
    /// Console-based interactive menu for the Quantity Measurement application.
    ///
    /// UC15 changes vs UC14:
    ///  - Implements <see cref="IMenu"/> so Program.cs depends on the abstraction.
    ///  - Depends on <see cref="IQuantityMeasurementController"/> (injected), NOT on
    ///    the concrete service class — following Dependency Inversion Principle.
    ///  - Builds QuantityDTO objects from user input and passes them to the controller.
    ///  - All business logic has been removed; this class is presentation only.
    ///  - Quantity&lt;T&gt; is no longer constructed here; DTOs are used instead.
    /// </summary>
    public class QuantityMeasurementAppMenu : IMenu
    {
        private readonly IQuantityMeasurementController _controller;

        public QuantityMeasurementAppMenu(IQuantityMeasurementController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
        }

        // ── Main loop ─────────────────────────────────────────────────────────

        public void Run()
        {
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("\n-----------------------");
                Console.WriteLine("Quantity Measurement App");
                Console.WriteLine("-----------------------");
                Console.WriteLine("1. Length Measurement");
                Console.WriteLine("2. Weight Measurement");
                Console.WriteLine("3. Volume Measurement");
                Console.WriteLine("4. Temperature Measurement");
                Console.WriteLine("5. Exit");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1": RunCategory("Length",      "0:Inches  1:Feet  2:Yards  3:Centimeters",
                                          new[] { "Inches", "Feet", "Yards", "Centimeters" }); break;
                    case "2": RunCategory("Weight",      "0:Grams  1:Kilograms  2:Pound",
                                          new[] { "Grams", "Kilograms", "Pound" }); break;
                    case "3": RunCategory("Volume",      "0:Litre  1:MilliLiter  2:Gallon",
                                          new[] { "Litre", "MilliLiter", "Gallon" }); break;
                    case "4": RunCategory("Temperature", "0:Celsius  1:Fahrenheit  2:Kelvin",
                                          new[] { "Celsius", "Fahrenheit", "Kelvin" }); break;
                    case "5": exit = true; break;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        // ── Category sub-menu ─────────────────────────────────────────────────

        private void RunCategory(string category, string unitOptions, string[] units)
        {
            bool back = false;

            while (!back)
            {
                Console.WriteLine($"\n--- {category} Measurement ---");
                Console.WriteLine("1. Conversion\n2. Comparison\n3. Addition\n4. Subtraction\n5. Divide\n6. Back");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1": HandleConversion(category, unitOptions, units);  break;
                    case "2": HandleComparison(category, unitOptions, units);  break;
                    case "3": HandleAddition(category, unitOptions, units);    break;
                    case "4": HandleSubtraction(category, unitOptions, units); break;
                    case "5": HandleDivision(category, unitOptions, units);    break;
                    case "6": back = true; break;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        // ── Operation handlers ────────────────────────────────────────────────

        private void HandleConversion(string category, string unitOptions, string[] units)
        {
            try
            {
                Console.WriteLine(unitOptions);
                var input = ReadSingleQuantity(category, units);

                Console.Write("Target unit index: ");
                string targetUnit = SelectUnit(units);

                var result = _controller.PerformConversion(input, targetUnit);
                DisplayResult("Conversion", result);
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        private void HandleComparison(string category, string unitOptions, string[] units)
        {
            try
            {
                Console.WriteLine(unitOptions);
                var (q1, q2) = ReadTwoQuantities(category, units);
                var result   = _controller.PerformComparison(q1, q2);
                Console.WriteLine($"\nResult: {q1} {(result.Value == 1 ? "==" : "!=")} {q2}");
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        private void HandleAddition(string category, string unitOptions, string[] units)
        {
            try
            {
                Console.WriteLine(unitOptions);
                var (q1, q2) = ReadTwoQuantities(category, units);
                var result   = _controller.PerformAddition(q1, q2);
                DisplayResult("Addition", result);
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        private void HandleSubtraction(string category, string unitOptions, string[] units)
        {
            try
            {
                Console.WriteLine(unitOptions);
                var (q1, q2) = ReadTwoQuantities(category, units);
                var result   = _controller.PerformSubtraction(q1, q2);
                DisplayResult("Subtraction", result);
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        private void HandleDivision(string category, string unitOptions, string[] units)
        {
            try
            {
                Console.WriteLine(unitOptions);
                var (q1, q2) = ReadTwoQuantities(category, units);
                var result   = _controller.PerformDivision(q1, q2);
                Console.WriteLine($"\nRatio: {result.Value} (Dimensionless)");
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

        // ── Input helpers ─────────────────────────────────────────────────────

        private QuantityDTO ReadSingleQuantity(string category, string[] units)
        {
            Console.Write("Enter value: ");
            double value = double.Parse(Console.ReadLine()!);

            Console.Write("Unit index: ");
            string unit = SelectUnit(units);

            return new QuantityDTO(value, unit, category);
        }

        private (QuantityDTO, QuantityDTO) ReadTwoQuantities(string category, string[] units)
        {
            Console.Write("Value 1: ");
            double v1 = double.Parse(Console.ReadLine()!);
            Console.Write("Unit 1 index: ");
            string u1 = SelectUnit(units);

            Console.Write("Value 2: ");
            double v2 = double.Parse(Console.ReadLine()!);
            Console.Write("Unit 2 index: ");
            string u2 = SelectUnit(units);

            return (new QuantityDTO(v1, u1, category), new QuantityDTO(v2, u2, category));
        }

        private static string SelectUnit(string[] units)
        {
            int idx = int.Parse(Console.ReadLine()!);
            if (idx < 0 || idx >= units.Length)
                throw new ArgumentOutOfRangeException("Unit index out of range.");
            return units[idx];
        }

        private static void DisplayResult(string operation, QuantityDTO result)
        {
            if (result.IsError)
                Console.WriteLine($"\n[{operation} Error] {result.ErrorMessage}");
            else
                Console.WriteLine($"\nResult: {result}");
        }
    }
}
