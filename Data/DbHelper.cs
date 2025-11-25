using Microsoft.Data.SqlClient;
using System.Data;

namespace AccountingSuite.Data
{
    public class DbHelper
    {
        private readonly string _connectionString;
        public DbHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Open Sql Connection
        public SqlConnection GetConnection()
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            return conn;
        }

        // Create Sql Command for Stored Procedure
        public SqlCommand CreateCommand(SqlConnection conn, string procName)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = procName;
            cmd.CommandType = CommandType.StoredProcedure;
            return cmd;
        }

        // Explicit parameter helper (avoids AddWithValue pitfalls)
        public void AddParameter(SqlCommand cmd, string name, SqlDbType type, object? value, int size = 0)
        {
            var param = cmd.Parameters.Add(name, type);
            if (size > 0) param.Size = size;
            param.Value = value ?? DBNull.Value;
        }
    }
}
