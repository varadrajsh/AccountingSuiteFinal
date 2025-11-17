using AccountingSuite.Models.Master;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using static AccountingSuite.Models.Master.Party;

namespace AccountingSuite.Data
{
    public class PartyRepository
    {
        private readonly DbHelper _db;
        public PartyRepository(DbHelper db)
        {
            _db = db;
        }

        public IEnumerable<Party> GetAll()
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spParty_GetAll");
            using var reader = cmd.ExecuteReader();
            var list = new List<Party>();
            while (reader.Read())
            {
                list.Add(new Party
                {
                    PartyId = Convert.ToInt32(reader["PartyId"]),
                    PartyCode = reader["PartyCode"].ToString()!,
                    Name = reader["Name"].ToString()!,
                    PartyType = Enum.Parse<PartyTypeEnum>(reader["PartyType"].ToString()!),
                    GSTIN = reader["GSTIN"] as string,
                    Address = reader["Address"] as string,
                    ContactNumber = reader["ContactNumber"] as string,
                    Email = reader["Email"] as string,
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                });
            }
            return list;
        }

        public (int newId, string newCode) Create(Party party)
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spParty_Insert");
            //cmd.Parameters.AddWithValue("@PartyCode", party.PartyCode);
            cmd.Parameters.AddWithValue("@Name", party.Name);
            cmd.Parameters.AddWithValue("@PartyType", party.PartyType.ToString());
            cmd.Parameters.AddWithValue("@GSTIN", (object?)party.GSTIN ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Address", (object?)party.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContactNumber", (object?)party.ContactNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", (object?)party.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", party.IsActive);

            try
            {
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return (
                        Convert.ToInt32(reader["NewPartyId"]),
                        reader["NewPartyCode"].ToString()!
                    );
                }
                return (0, "");
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public (int updatedId, string updatedCode) Update(Party party)
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spParty_Update");
            cmd.Parameters.AddWithValue("@PartyId", party.PartyId);
            cmd.Parameters.AddWithValue("@PartyCode", party.PartyCode);
            cmd.Parameters.AddWithValue("@Name", party.Name);
            cmd.Parameters.AddWithValue("@PartyType", party.PartyType.ToString());
            cmd.Parameters.AddWithValue("@GSTIN", (object?)party.GSTIN ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Address", (object?)party.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContactNumber", (object?)party.ContactNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", (object?)party.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", party.IsActive);

            try
            {
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return (
                        Convert.ToInt32(reader["UpdatedPartyId"]),
                        reader["UpdatedPartyCode"].ToString()!
                    );
                }
                return (0, "");
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public Party? GetById(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spParty_GetById");
            cmd.Parameters.AddWithValue("@PartyId", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Party
                {
                    PartyId = Convert.ToInt32(reader["PartyId"]),
                    PartyCode = reader["PartyCode"].ToString()!,
                    Name = reader["Name"].ToString()!,
                    PartyType = Enum.Parse<PartyTypeEnum>(reader["PartyType"].ToString()!),
                    GSTIN = reader["GSTIN"] as string,
                    Address = reader["Address"] as string,
                    ContactNumber = reader["ContactNumber"] as string,
                    Email = reader["Email"] as string,
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                };
            }
            return null;
        }
                

        public void Delete(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spParty_Delete");
            cmd.Parameters.AddWithValue("@PartyId", id);
            cmd.ExecuteNonQuery();
        }
    }
}
