using AccountingSuite.Models.Accounting;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace AccountingSuite.Data
{
    public class AccountHeadRepository
    {
        private readonly DbHelper _db;
        public AccountHeadRepository(DbHelper db)
        {
            _db = db;
        }

        // Get all AccountHeads
        public IEnumerable<AccountHead> GetAll()
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spAccountHead_GetAll");
            using var reader = cmd.ExecuteReader();
            var list = new List<AccountHead>();
            while (reader.Read())
            {
                list.Add(new AccountHead
                {
                    AccountHeadId = Convert.ToInt32(reader["AccountHeadId"]),
                    AccountHeadCode = reader["AccountHeadCode"].ToString()!,
                    AccountHeadName = reader["AccountHeadName"].ToString()!,
                    AccountHeadType = Enum.Parse<AccountHeadTypeEnum>(reader["AccountHeadType"].ToString()!),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    ParentAccountHeadId = reader["ParentAccountHeadId"] as int?
                });
            }
            return list;
        }

        // Create new AccountHead (duplicate-safe via SP)
        public (int newId, string code) Create(AccountHead accountHead)
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spAccountHead_Insert");
            cmd.Parameters.AddWithValue("@AccountHeadName", accountHead.AccountHeadName);
            cmd.Parameters.AddWithValue("@AccountHeadType", accountHead.AccountHeadType.ToString());
            cmd.Parameters.AddWithValue("@IsActive", true);

            try
            {
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return (
                        Convert.ToInt32(reader["NewAccountHeadId"]),
                        reader["NewAccountHeadCode"].ToString()!
                    );
                }
                return (0, "");
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Update status only (toggle active/inactive)
        public void UpdateStatus(int id, bool isActive)
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spAccountHead_UpdateStatus");
            cmd.Parameters.AddWithValue("@AccountHeadId", id);
            cmd.Parameters.AddWithValue("@IsActive", isActive);
            cmd.ExecuteNonQuery();
        }

        // Get by Id
        public AccountHead? GetById(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spAccountHead_GetById");
            cmd.Parameters.AddWithValue("@AccountHeadId", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new AccountHead
                {
                    AccountHeadId = Convert.ToInt32(reader["AccountHeadId"]),
                    AccountHeadCode = reader["AccountHeadCode"].ToString()!,
                    AccountHeadName = reader["AccountHeadName"].ToString()!,
                    AccountHeadType = Enum.Parse<AccountHeadTypeEnum>(reader["AccountHeadType"].ToString()!),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    ParentAccountHeadId = reader["ParentAccountHeadId"] as int?
                };
            }
            return null;
        }
    }
}
