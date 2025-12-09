using System;
using System.Data;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.Data.SqlClient;

namespace AccountingSuite.Data;

public class DbHelperAsync
{
    private readonly string _connectionString;
    public DbHelperAsync(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    // Open SqlConnection
    public async Task<SqlConnection> GetSqlConnectionAsync()
    {
        var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        return conn;
    }

    // Create COmmand for Store Procedure
    public SqlCommand CreateCommand(SqlConnection conn, string procName)
    {
        var cmd = conn.CreateCommand();
        cmd.CommandText = procName;
        cmd.CommandType = CommandType.StoredProcedure;
        return cmd;
    }

    //Add Parameters 
    public void AddParameter(SqlCommand cmd, string name, SqlDbType type, object? value, int size = 0)
    {
        var param = cmd.Parameters.Add(name, type);
        if (size > 0) param.Size = size;
        param.Value = value ?? DBNull.Value;
    }

    //Execute StoreProcedure that modifies data (Insert/Update/Delete) 
    public async Task<int> ExecuteNonQueryAsync(SqlCommand cmd)
    {
        return await cmd.ExecuteNonQueryAsync();
    }

    //Execute StoreProc that returns a single scalar table
    public async Task<object?> ExecuteScalarAsync(SqlCommand cmd)
    {
        return await cmd.ExecuteScalarAsync();
    }

    //Execute StoreProc that returns a DataTable
    public async Task<DataTable> ExecuteDataTableAsync(SqlCommand cmd)
    {
        using var reader = await cmd.ExecuteReaderAsync();
        var table = new DataTable();
        table.Load(reader);
        return table;
    }

    //Execute StoreProc that returns a SqlDataReader
    //Caller must dispose reader(and connection closes when reader is disposed) 
    public async Task<SqlDataReader> ExecuteReaderAsync(SqlCommand cmd)
    {
        return await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
    }
}
