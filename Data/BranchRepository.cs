using AccountingSuite.Models.Master;
using System.Data.SqlClient;

namespace AccountingSuite.Data.Repositories
{
    public class BranchRepository
    {
        private readonly DbHelper _db;

        public BranchRepository(DbHelper db)
        {
            _db = db;
        }
    
        public IEnumerable<Branch> GetAll()
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spBranch_GetAll");
            using var reader = cmd.ExecuteReader();

            var list = new List<Branch>();
            while (reader.Read())
            {
                list.Add(new Branch
                {
                    BranchId = Convert.ToInt32(reader["BranchId"]),
                    BranchCode = reader["BranchCode"].ToString()!,
                    BranchName = reader["BranchName"].ToString()!,
                    StateId = Convert.ToInt32(reader["StateId"]),
                    Email = reader["Email"].ToString()!,
                    Address = reader["Address"].ToString()!,
                    PinCode = reader["PinCode"].ToString()!,
                    LandNumber = reader["LandNumber"].ToString()!,
                    MobNumber = reader["MobNumber"].ToString()!,
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    IsParcelBooking = Convert.ToBoolean(reader["IsParcelBooking"]),
                    State = new State
                    {
                        StateId = Convert.ToInt32(reader["StateId"]),
                        StateName = reader["StateName"].ToString()!
                    }
                });
            }
            return list;
        }

        public Branch? GetById(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spBranch_GetById");
            cmd.Parameters.AddWithValue("@BranchId", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Branch
                {
                    BranchId = Convert.ToInt32(reader["BranchId"]),
                    BranchCode = reader["BranchCode"].ToString()!,
                    BranchName = reader["BranchName"].ToString()!,
                    StateId = Convert.ToInt32(reader["StateId"]),
                    Email = reader["Email"].ToString()!,
                    Address = reader["Address"].ToString()!,
                    PinCode = reader["PinCode"].ToString()!,
                    LandNumber = reader["LandNumber"].ToString()!,
                    MobNumber = reader["MobNumber"].ToString()!,
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    IsParcelBooking = Convert.ToBoolean(reader["IsParcelBooking"]),
                    State = new State
                    {
                        StateId = Convert.ToInt32(reader["StateId"]),
                        StateName = reader["StateName"].ToString()!
                    }
                };
            }
            return null;
        }

        public void Insert(Branch branch)
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spBranch_Insert");

            cmd.Parameters.AddWithValue("@BranchCode", branch.BranchCode.Trim().ToUpperInvariant());
            cmd.Parameters.AddWithValue("@BranchName", branch.BranchName.Trim().ToUpperInvariant());
            cmd.Parameters.AddWithValue("@StateId", branch.StateId);
            cmd.Parameters.AddWithValue("@Email", branch.Email);
            cmd.Parameters.AddWithValue("@Address", branch.Address);
            cmd.Parameters.AddWithValue("@PinCode", branch.PinCode);
            cmd.Parameters.AddWithValue("@LandNumber", branch.LandNumber);
            cmd.Parameters.AddWithValue("@MobNumber", branch.MobNumber);
            cmd.Parameters.AddWithValue("@IsParcelBooking", branch.IsParcelBooking);

            cmd.ExecuteNonQuery();
        }

        public void UpdateStatus(int branchId, bool isActive)
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spBranch_UpdateStatus");
            cmd.Parameters.AddWithValue("@BranchId", branchId);
            cmd.Parameters.AddWithValue("@IsActive", isActive);
            cmd.ExecuteNonQuery();
        }

        public void Update(Branch branch)
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spBranch_Update");

            cmd.Parameters.AddWithValue("@BranchId", branch.BranchId);
            cmd.Parameters.AddWithValue("@BranchCode", branch.BranchCode.Trim().ToUpperInvariant());
            cmd.Parameters.AddWithValue("@BranchName", branch.BranchName.Trim().ToUpperInvariant());
            cmd.Parameters.AddWithValue("@StateId", branch.StateId);
            cmd.Parameters.AddWithValue("@Email", branch.Email);
            cmd.Parameters.AddWithValue("@Address", branch.Address);
            cmd.Parameters.AddWithValue("@PinCode", branch.PinCode);
            cmd.Parameters.AddWithValue("@LandNumber", branch.LandNumber);
            cmd.Parameters.AddWithValue("@MobNumber", branch.MobNumber);
            cmd.Parameters.AddWithValue("@IsActive", branch.IsActive); // ✅ toggle allowed
            cmd.Parameters.AddWithValue("@IsParcelBooking", branch.IsParcelBooking);

            cmd.ExecuteNonQuery();
        }

    }
}
