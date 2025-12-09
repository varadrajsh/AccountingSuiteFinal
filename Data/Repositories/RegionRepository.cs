using System.Data;
using AccountingSuite.Models.Master;

namespace AccountingSuite.Data
{
    public class RegionRepository
    {
        private readonly DbHelperAsync _db;
        public RegionRepository(DbHelperAsync db)
        {
            _db = db;
        }

        public async Task<List<Region>> GetAllAsync()
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spRegion_GetAll");
            
            using var reader = await cmd.ExecuteReaderAsync();
            var list = new List<Region>();

            while (await reader.ReadAsync())
            {
                list.Add(new Region
                {
                    RegionId = Convert.ToInt32(reader["RegionId"]),
                    RegionName = reader["RegionName"].ToString()!
                });
            }
            return list;
        }
        public async Task<List<State>> GetByRegionAsync(int regionId)
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spState_GetByRegion");

            _db.AddParameter(cmd, "@RegionId", SqlDbType.Int, regionId);
        
            using var reader = await cmd.ExecuteReaderAsync();
            var list = new List<State>();

            while (await reader.ReadAsync())
            {
                list.Add(new State
                {
                    StateId = Convert.ToInt32(reader["StateId"]),
                    StateName = reader["StateName"].ToString()!,
                    RegionId = Convert.ToInt32(reader["RegionId"])
                });
            }
            return list;
        }
        public async Task<bool> ExistsAsync(string stateName, int regionId)
        {
            var normalized = stateName?.Trim().TrimEnd('.').ToUpperInvariant();
            var states = await GetByRegionAsync(regionId);

            return states.Any(s => string.Equals(
                s.StateName?.Trim().TrimEnd('.').ToUpperInvariant(),
                normalized,
                StringComparison.OrdinalIgnoreCase));
        }
    }
}
