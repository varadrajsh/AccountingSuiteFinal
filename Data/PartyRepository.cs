using AccountingSuite.Models.Master;
using Microsoft.Data.SqlClient;
using System.Data;
using static AccountingSuite.Models.Master.Party;

namespace AccountingSuite.Data
{
    public class PartyRepository
    {
        private readonly DbHelperAsync _db;
        public PartyRepository(DbHelperAsync db)
        {
            _db = db;
        }

        public async Task<List<Party>> GetAllAsync()
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spParty_GetAll");

            using var reader =await cmd.ExecuteReaderAsync();
            var list = new List<Party>();

            while (await reader.ReadAsync())
            {
                Enum.TryParse<PartyTypeEnum>(reader["PartyType"]?.ToString() ?? "", true, out var partyType);

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

        public async Task<List<Party>> GetAllWithStateAsync()
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spParty_GetAllWithState");

            using var reader = await cmd.ExecuteReaderAsync();
            var list = new List<Party>();

            while (await reader.ReadAsync())
            {
                Enum.TryParse<PartyTypeEnum>(reader["PartyType"]?.ToString() ?? "", true, out var partyType);

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
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    StateId = Convert.ToInt32(reader["StateId"]),
                    StateName = reader["StateName"].ToString()!
                });
            }
            return list;
        }

        public async Task<(int newId, string newCode)> CreateAsync(Party party)
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spParty_Insert");

            // Align parameter sizes with model annotations
            _db.AddParameter(cmd, "@Name", SqlDbType.NVarChar, party.Name, 100);
            _db.AddParameter(cmd, "@PartyType", SqlDbType.NVarChar, party.PartyType.ToString(), 20);
            _db.AddParameter(cmd, "@GSTIN", SqlDbType.NVarChar, party.GSTIN, 15);
            _db.AddParameter(cmd, "@Address", SqlDbType.NVarChar, party.Address, 250);
            _db.AddParameter(cmd, "@ContactNumber", SqlDbType.NVarChar, party.ContactNumber, 20);
            _db.AddParameter(cmd, "@Email", SqlDbType.NVarChar, party.Email, 100);
            _db.AddParameter(cmd, "@IsActive", SqlDbType.Bit, party.IsActive);
            _db.AddParameter(cmd, "@StateId", SqlDbType.Int, party.StateId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return (
                    Convert.ToInt32(reader["NewPartyId"]),
                    reader["NewPartyCode"].ToString()!
                );
            }
            throw new Exception("Insert failed: no result returned.");
        }

        public async Task<(int updatedId, string updatedCode)> UpdateAsync(Party party)
        {
            using var conn = await _db.GetSqlConnectionAsync();
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
            _db.AddParameter(cmd, "@StateId", SqlDbType.Int, party.StateId);

            try
            {
                using var reader = await cmd.ExecuteReaderAsync();
                if (! await reader.ReadAsync())
                    throw new Exception("Update failed: no result returned.");

                return (
                    Convert.ToInt32(reader["UpdatedPartyId"]),
                    reader["UpdatedPartyCode"].ToString()!
                );
            }
            catch (SqlException ex)
            {
                // Wrap with friendly message mapping point (you can plug in your mapper here)
                throw new Exception("Unable to update Party. Please check inputs or duplicates.", ex);
            }
        }

        public async Task<Party?> GetByIdAsync(int id)
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spParty_GetById");

            _db.AddParameter(cmd, "@PartyId", SqlDbType.Int, id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;

            Enum.TryParse<PartyTypeEnum>(reader["PartyType"]?.ToString() ?? "", true, out var partyType);

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
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                StateId = reader["StateId"] is DBNull ? 0 : Convert.ToInt32(reader["StateId"]),
                StateName = reader["StateName"] as string
            };
        }

        public async Task Delete(int id)
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spParty_Delete");

            _db.AddParameter(cmd, "@PartyId", SqlDbType.Int, id);

            int rows = await _db.ExecuteNonQueryAsync(cmd);
            if (rows == 0)
            {
                throw new Exception("Delete failed: Party not found or already deleted.");
            }
        }
    }
}
