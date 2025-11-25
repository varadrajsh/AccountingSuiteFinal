using AccountingSuite.Models.Master;
using Microsoft.Data.SqlClient;
using System.Data;
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
                var partyTypeStr = reader["PartyType"].ToString();
                Enum.TryParse<PartyTypeEnum>(partyTypeStr, out var partyType);

                list.Add(new Party
                {
                    PartyId = Convert.ToInt32(reader["PartyId"]),
                    PartyCode = reader["PartyCode"].ToString()!,
                    Name = reader["Name"].ToString()!,
                    PartyType = partyType,
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

            _db.AddParameter(cmd, "@Name", SqlDbType.NVarChar, party.Name, 100);
            _db.AddParameter(cmd, "@PartyType", SqlDbType.NVarChar, party.PartyType.ToString(), 20);
            _db.AddParameter(cmd, "@GSTIN", SqlDbType.NVarChar, party.GSTIN, 15);
            _db.AddParameter(cmd, "@Address", SqlDbType.NVarChar, party.Address, 250);
            _db.AddParameter(cmd, "@ContactNumber", SqlDbType.NVarChar, party.ContactNumber, 20);
            _db.AddParameter(cmd, "@Email", SqlDbType.NVarChar, party.Email, 100);
            _db.AddParameter(cmd, "@IsActive", SqlDbType.Bit, party.IsActive);

            try
            {
                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                    throw new Exception("Insert failed: no result returned.");

                return (
                    Convert.ToInt32(reader["NewPartyId"]),
                    reader["NewPartyCode"].ToString()!
                );
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error during Create: {ex.Message}", ex);
            }
        }

        public (int updatedId, string updatedCode) Update(Party party)
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spParty_Update");

            _db.AddParameter(cmd, "@PartyId", SqlDbType.Int, party.PartyId);
            _db.AddParameter(cmd, "@PartyCode", SqlDbType.NVarChar, party.PartyCode, 20);
            _db.AddParameter(cmd, "@Name", SqlDbType.NVarChar, party.Name, 100);
            _db.AddParameter(cmd, "@PartyType", SqlDbType.NVarChar, party.PartyType.ToString(), 20);
            _db.AddParameter(cmd, "@GSTIN", SqlDbType.NVarChar, party.GSTIN, 15);
            _db.AddParameter(cmd, "@Address", SqlDbType.NVarChar, party.Address, 250);
            _db.AddParameter(cmd, "@ContactNumber", SqlDbType.NVarChar, party.ContactNumber, 20);
            _db.AddParameter(cmd, "@Email", SqlDbType.NVarChar, party.Email, 100);
            _db.AddParameter(cmd, "@IsActive", SqlDbType.Bit, party.IsActive);

            try
            {
                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                    throw new Exception("Update failed: no result returned.");

                return (
                    Convert.ToInt32(reader["UpdatedPartyId"]),
                    reader["UpdatedPartyCode"].ToString()!
                );
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error during Update: {ex.Message}", ex);
            }
        }

        public Party? GetById(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spParty_GetById");
            _db.AddParameter(cmd, "@PartyId", SqlDbType.Int, id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            Enum.TryParse<PartyTypeEnum>(reader["PartyType"].ToString(), out var partyType);

            return new Party
            {
                PartyId = Convert.ToInt32(reader["PartyId"]),
                PartyCode = reader["PartyCode"].ToString()!,
                Name = reader["Name"].ToString()!,
                PartyType = partyType,
                GSTIN = reader["GSTIN"] as string,
                Address = reader["Address"] as string,
                ContactNumber = reader["ContactNumber"] as string,
                Email = reader["Email"] as string,
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };
        }

        public void Delete(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spParty_Delete");
            _db.AddParameter(cmd, "@PartyId", SqlDbType.Int, id);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error during Delete: {ex.Message}", ex);
            }
        }
    }
}
