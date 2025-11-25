using AccountingSuite.Models.Master;

namespace AccountingSuite.Data
{
    public class RegionRepository
    {
        private readonly DbHelper _db;
        public RegionRepository(DbHelper db)
        {
            _db = db;
        }

        public IEnumerable<Region> GetAll()
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spRegion_GetAll");
            using var reader = cmd.ExecuteReader();

            var list = new List<Region>();
            while (reader.Read())
            {
                list.Add(new Region
                {
                    RegionId = Convert.ToInt32(reader["RegionId"]),
                    RegionName = reader["RegionName"].ToString()!
                });
            }
            return list;
        }
        public IEnumerable<State> GetByRegion(int regionId)
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spState_GetByRegion");
            cmd.Parameters.AddWithValue("@RegionId", regionId);
            using var reader = cmd.ExecuteReader();
            var list = new List<State>();
            while (reader.Read())
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
        public bool Exists(string stateName, int regionId)
        {
            var normalized = stateName?.Trim().TrimEnd('.').ToUpperInvariant();

            return GetByRegion(regionId)
                   .Any(s => string.Equals(
                       s.StateName?.Trim().TrimEnd('.').ToUpperInvariant(),
                       normalized,
                       StringComparison.OrdinalIgnoreCase));
        }



    }
}
