using System.IO;
using System.Text.Json;

namespace QuantityMeasurementApp.Config
{
    public static class AppConfig
    {
        private static string? connectionString;

        public static string GetConnectionString()
        {
            if (connectionString != null)
                return connectionString;

            string json = File.ReadAllText("appsettings.json");

            using var document = JsonDocument.Parse(json);

            connectionString =
                document.RootElement
                .GetProperty("ConnectionStrings")
                .GetProperty("DefaultConnection")
                .GetString();

            return connectionString!;
        }
    }
}