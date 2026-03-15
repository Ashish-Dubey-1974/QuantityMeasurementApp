using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ModelLayer.DTOs;
using ModelLayer.Entities;
using RepoLayer.Exceptions;
using RepoLayer.Interfaces;

namespace RepoLayer.Repositories
{
    public class QuantityDatabaseRepository : IQuantityRepository, IDisposable
    {
        private readonly string _connectionString;

        public QuantityDatabaseRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            _connectionString = connectionString;
            EnsureDatabaseExists();
            EnsureTablesExist();
        }

        private void EnsureDatabaseExists()
        {
            var masterConn = new SqlConnectionStringBuilder(_connectionString) { InitialCatalog = "master" };
            using var conn = new SqlConnection(masterConn.ConnectionString);
            conn.Open();

            using var checkCmd = conn.CreateCommand();
            checkCmd.CommandText = "SELECT COUNT(*) FROM sys.databases WHERE name = @dbName";
            checkCmd.Parameters.AddWithValue("@dbName", GetDatabaseName());
            int exists = (int)checkCmd.ExecuteScalar()!;

            if (exists == 0)
            {
                string dbName = GetDatabaseName();
                using var createCmd = conn.CreateCommand();
                createCmd.CommandText = $"CREATE DATABASE [{dbName}]";
                createCmd.ExecuteNonQuery();
                Console.WriteLine($"[DB] Created database: {dbName}");
            }
        }

        private void EnsureTablesExist()
        {
            using var conn = OpenConnection();
            using var cmd  = conn.CreateCommand();
            cmd.CommandText = @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'QuantityMeasurements')
                BEGIN
                    CREATE TABLE QuantityMeasurements (
                        Id               INT IDENTITY(1,1) PRIMARY KEY,
                        Op1Value         FLOAT          NOT NULL,
                        Op1Unit          NVARCHAR(50)   NOT NULL,
                        Op1Category      NVARCHAR(50)   NOT NULL,
                        Op2Value         FLOAT              NULL,
                        Op2Unit          NVARCHAR(50)       NULL,
                        Op2Category      NVARCHAR(50)       NULL,
                        OperationType    NVARCHAR(50)   NOT NULL,
                        ResultValue      FLOAT              NULL,
                        ResultUnit       NVARCHAR(50)       NULL,
                        ResultCategory   NVARCHAR(50)       NULL,
                        HasError         BIT            NOT NULL DEFAULT 0,
                        ErrorMessage     NVARCHAR(500)  NOT NULL DEFAULT '',
                        Timestamp        DATETIME2      NOT NULL DEFAULT GETUTCDATE()
                    );
                    CREATE INDEX IX_QM_OpType ON QuantityMeasurements(OperationType);
                    CREATE INDEX IX_QM_Cat    ON QuantityMeasurements(Op1Category);
                END";
            cmd.ExecuteNonQuery();
        }

        public void Save(QuantityMeasurementEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            try
            {
                using var conn = OpenConnection();
                using var cmd  = conn.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO QuantityMeasurements
                        (Op1Value,Op1Unit,Op1Category,
                         Op2Value,Op2Unit,Op2Category,
                         OperationType,
                         ResultValue,ResultUnit,ResultCategory,
                         HasError,ErrorMessage,Timestamp)
                    VALUES
                        (@op1v,@op1u,@op1c,
                         @op2v,@op2u,@op2c,
                         @opType,
                         @resv,@resu,@resc,
                         @hasErr,@errMsg,@ts)";

                cmd.Parameters.AddWithValue("@op1v",   (object?)entity.Operand1?.Value    ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@op1u",   (object?)entity.Operand1?.UnitName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@op1c",   (object?)entity.Operand1?.Category ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@op2v",   (object?)entity.Operand2?.Value    ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@op2u",   (object?)entity.Operand2?.UnitName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@op2c",   (object?)entity.Operand2?.Category ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@opType", entity.OperationType);
                cmd.Parameters.AddWithValue("@resv",   (object?)entity.Result?.Value      ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@resu",   (object?)entity.Result?.UnitName   ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@resc",   (object?)entity.Result?.Category   ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@hasErr", entity.HasError ? 1 : 0);
                cmd.Parameters.AddWithValue("@errMsg", entity.ErrorMessage ?? string.Empty);
                cmd.Parameters.AddWithValue("@ts",     entity.Timestamp);
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex) { throw new DatabaseException("Failed to save measurement.", ex); }
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetAll()
            => Query("SELECT * FROM QuantityMeasurements ORDER BY Timestamp ASC", Array.Empty<SqlParameter>());

        public IReadOnlyList<QuantityMeasurementEntity> GetByOperation(string operationType)
            => Query("SELECT * FROM QuantityMeasurements WHERE OperationType=@v ORDER BY Timestamp ASC",
                     new[] { new SqlParameter("@v", operationType) });

        public IReadOnlyList<QuantityMeasurementEntity> GetByCategory(string category)
            => Query("SELECT * FROM QuantityMeasurements WHERE Op1Category=@v ORDER BY Timestamp ASC",
                     new[] { new SqlParameter("@v", category) });

        public int GetTotalCount()
        {
            try
            {
                using var conn = OpenConnection();
                using var cmd  = conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM QuantityMeasurements";
                return (int)cmd.ExecuteScalar()!;
            }
            catch (SqlException ex) { throw new DatabaseException("Failed to get count.", ex); }
        }

        public void DeleteAll()
        {
            try
            {
                using var conn = OpenConnection();
                using var cmd  = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM QuantityMeasurements";
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex) { throw new DatabaseException("Failed to delete all.", ex); }
        }

        private IReadOnlyList<QuantityMeasurementEntity> Query(string sql, SqlParameter[] parameters)
        {
            try
            {
                var list = new List<QuantityMeasurementEntity>();
                using var conn = OpenConnection();
                using var cmd  = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(parameters);
                using var reader = cmd.ExecuteReader();
                while (reader.Read()) list.Add(MapRow(reader));
                return list.AsReadOnly();
            }
            catch (SqlException ex) { throw new DatabaseException("Query failed.", ex); }
        }

        private static QuantityMeasurementEntity MapRow(SqlDataReader r)
        {
            var op1 = new QuantityDTO(
                r.IsDBNull(r.GetOrdinal("Op1Value"))    ? 0  : r.GetDouble(r.GetOrdinal("Op1Value")),
                r.IsDBNull(r.GetOrdinal("Op1Unit"))     ? "" : r.GetString(r.GetOrdinal("Op1Unit")),
                r.IsDBNull(r.GetOrdinal("Op1Category")) ? "" : r.GetString(r.GetOrdinal("Op1Category")));

            QuantityDTO? op2 = null;
            if (!r.IsDBNull(r.GetOrdinal("Op2Unit")))
                op2 = new QuantityDTO(
                    r.GetDouble(r.GetOrdinal("Op2Value")),
                    r.GetString(r.GetOrdinal("Op2Unit")),
                    r.GetString(r.GetOrdinal("Op2Category")));

            string opType  = r.GetString(r.GetOrdinal("OperationType"));
            bool   hasErr  = r.GetBoolean(r.GetOrdinal("HasError"));
            string errMsg  = r.GetString(r.GetOrdinal("ErrorMessage"));

            QuantityDTO? result = null;
            if (!r.IsDBNull(r.GetOrdinal("ResultUnit")))
                result = new QuantityDTO(
                    r.GetDouble(r.GetOrdinal("ResultValue")),
                    r.GetString(r.GetOrdinal("ResultUnit")),
                    r.GetString(r.GetOrdinal("ResultCategory")));

            if (hasErr)          return new QuantityMeasurementEntity(op1, op2, opType, errMsg);
            if (op2 == null)     return new QuantityMeasurementEntity(op1, opType, result!);
            return new QuantityMeasurementEntity(op1, op2, opType, result!);
        }

        private SqlConnection OpenConnection()
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            return conn;
        }

        private string GetDatabaseName()
            => new SqlConnectionStringBuilder(_connectionString).InitialCatalog;

        public void Dispose() { }
    }
}
