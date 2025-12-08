using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AccountingSuite.Models.Master;
using Microsoft.Data.SqlClient;

namespace AccountingSuite.Data
{
    public class StateRepository
    {
        private readonly DbHelperAsync _db;
        public StateRepository(DbHelperAsync db) => _db = db;

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


        public async Task<int> InsertAsync(State state)
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spState_Insert");

            _db.AddParameter(cmd, "@", SqlDbType.NVarChar, state.StateName);
            _db.AddParameter(cmd, "@regionId", SqlDbType.Int, state.RegionId);

            return await _db.ExecuteNonQueryAsync(cmd);
        }
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

        public async Task<bool> Exists(string stateName, int regionId)
        {
            var states = await GetByRegionAsync(regionId); 
            return states.Any(s => string.Equals(s.StateName, stateName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
