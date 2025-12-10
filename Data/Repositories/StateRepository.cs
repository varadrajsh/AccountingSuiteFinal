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

        // Insert new state (normalize + duplicate checks)
        public async Task<(string Status, int? NewId)> InsertAsync(State state)
        {
            // Normalize StateName
            string normalizedName = state.StateName?
                .Trim()
                .TrimEnd('.', '-', '_')
                .ToUpperInvariant() ?? string.Empty;

            // Check in same region
            var statesInRegion = await GetByRegionAsync(state.RegionId);
            bool existsInRegion = statesInRegion.Any(s =>
                (s.StateName?.Trim().TrimEnd('.', '-', '_').ToUpperInvariant() ?? string.Empty)
                    .Equals(normalizedName, StringComparison.OrdinalIgnoreCase));

            if (existsInRegion)
            {
                return ("DUPLICATE_REGION", null);
            }

            // Check globally (other regions)
            var allStates = await GetAllAsync();
            bool existsElsewhere = allStates.Any(s =>
                (s.StateName?.Trim().TrimEnd('.', '-', '_').ToUpperInvariant() ?? string.Empty)
                    .Equals(normalizedName, StringComparison.OrdinalIgnoreCase));

            if (existsElsewhere)
            {
                return ("DUPLICATE_OTHER_REGION", null);
            }

            bool existsSimilar = allStates.Any(s =>
            {
                var existing = (s.StateName?.Trim().TrimEnd('.', '-', '_').ToUpperInvariant() ?? string.Empty);
                return existing.StartsWith(normalizedName) || normalizedName.StartsWith(existing);
            });

            if (existsSimilar)
            {
                return ("DUPLICATE_SIMILAR", null);
            }


            // Proceed with insert
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spState_Insert");
            cmd.CommandType = CommandType.StoredProcedure;

            _db.AddParameter(cmd, "@StateName", SqlDbType.NVarChar, normalizedName, 100);
            _db.AddParameter(cmd, "@RegionId", SqlDbType.Int, state.RegionId);
            _db.AddParameter(cmd, "@CreatedBy", SqlDbType.Int, 101); // replace with current user id

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                string status = reader["Status"].ToString()!;
                int? newId = reader["NewStateId"] == DBNull.Value ? null : (int?)reader["NewStateId"];
                return (status, newId);
            }

            return ("ERROR", null);
        }
    }
}
