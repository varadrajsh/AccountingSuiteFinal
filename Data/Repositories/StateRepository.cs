using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AccountingSuite.Models.Master;
using Microsoft.Data.SqlClient;

namespace AccountingSuite.Data.Repositories
{
    public class StateRepository
    {
        private readonly DbHelperAsync _db;
        public StateRepository(DbHelperAsync db) => _db = db;

        // Get all states
        public async Task<List<State>> GetAllAsync()
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spState_GetAll");
            var table = await _db.ExecuteDataTableAsync(cmd);

            var states = new List<State>();
            foreach (DataRow row in table.Rows)
            {
                states.Add(new State
                {
                    StateId = Convert.ToInt32(row["StateId"]),
                    StateName = row["StateName"].ToString()!,
                    RegionId = Convert.ToInt32(row["RegionId"])
                });
            }
            return states;
        }

        // Get states by region
        public async Task<List<State>> GetByRegionAsync(int regionId)
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spState_GetByRegion");
            _db.AddParameter(cmd, "@RegionId", SqlDbType.Int, regionId);

            var table = await _db.ExecuteDataTableAsync(cmd);

            var states = new List<State>();
            foreach (DataRow row in table.Rows)
            {
                states.Add(new State
                {
                    StateId = Convert.ToInt32(row["StateId"]),
                    StateName = row["StateName"].ToString()!,
                    RegionId = Convert.ToInt32(row["RegionId"])
                });
            }
            return states;
        }

        // Insert new state (Party-style pattern: Status + NewId)
        public async Task<(string Status, int? NewId)> InsertAsync(State state)
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spState_Insert");
            cmd.CommandType = CommandType.StoredProcedure;

            _db.AddParameter(cmd, "@StateName", SqlDbType.NVarChar, state.StateName, 100);
            _db.AddParameter(cmd, "@RegionId", SqlDbType.Int, state.RegionId);
            _db.AddParameter(cmd, "@CreatedBy", SqlDbType.Int, 101); // or current user id

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                string status = reader["Status"].ToString()!;
                int? newId = reader["NewStateId"] == DBNull.Value ? null : (int?)reader["NewStateId"];
                return (status, newId);
            }

            return ("ERROR", null);
        }

        // Get state by ID
        public async Task<State?> GetByIdAsync(int id)
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spState_GetById");
            _db.AddParameter(cmd, "@StateId", SqlDbType.Int, id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new State
                {
                    StateId = Convert.ToInt32(reader["StateId"]),
                    StateName = reader["StateName"].ToString()!,
                    RegionId = Convert.ToInt32(reader["RegionId"])
                };
            }
            return null;
        }

        // Check if state exists in region
        public async Task<bool> Exists(string stateName, int regionId)
        {
            var states = await GetByRegionAsync(regionId);
            return states.Any(s => string.Equals(s.StateName, stateName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
