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

        // Open SqlConnection (caller must dispose with using)
        public SqlConnection GetConnection()
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            return conn;
        }

        // Create SqlCommand for stored procedure
        public SqlCommand CreateCommand(SqlConnection conn, string procName)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = procName;
            cmd.CommandType = CommandType.StoredProcedure;
            return cmd;
        }

        // Add parameters safely (explicit type avoids AddWithValue pitfalls)
        public void AddParameter(SqlCommand cmd, string name, SqlDbType type, object? value, int size = 0)
        {
            var param = cmd.Parameters.Add(name, type);
            if (size > 0) param.Size = size;
            param.Value = value ?? DBNull.Value;
        }

        // Execute stored procedure that modifies data (Insert/Update/Delete)
        public int ExecuteNonQuery(SqlCommand cmd)
        {
            return cmd.ExecuteNonQuery();
        }

        // Execute stored procedure that returns a single scalar value
        public object? ExecuteScalar(SqlCommand cmd)
        {
            return cmd.ExecuteScalar();
        }

        // Execute stored procedure that returns a DataTable
        public DataTable ExecuteDataTable(SqlCommand cmd)
        {
            using var reader = cmd.ExecuteReader();
            var table = new DataTable();
            table.Load(reader);
            return table;
        }
    }
}
