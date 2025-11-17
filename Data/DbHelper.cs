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

        // Create Sql Command and StoreProcedure
        public SqlCommand CreateCommand(SqlConnection conn, string procName)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = procName;
            cmd.CommandType = CommandType.StoredProcedure;
            return cmd;
        }
    }
}
