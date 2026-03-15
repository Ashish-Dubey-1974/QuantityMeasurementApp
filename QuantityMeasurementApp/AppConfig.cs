using System;

namespace QuantityMeasurementApp
{
    public static class AppConfig
    {
        // Override via env var: set QM_CONNECTION_STRING=your_conn_str
        public static string ConnectionString =>
            Environment.GetEnvironmentVariable("QM_CONNECTION_STRING")
            ?? @"Server=.\SQLEXPRESS;Database=QuantityMeasurementDB;Trusted_Connection=True;TrustServerCertificate=True;";

        // Override via env var: set QM_REPO_TYPE=cache  (or "database")
        public static string RepositoryType =>
            Environment.GetEnvironmentVariable("QM_REPO_TYPE")
            ?? "database";
    }
}
