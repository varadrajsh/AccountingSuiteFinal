using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AccountingSuite.Models.Master;
using Microsoft.Data.SqlClient;
using static AccountingSuite.Models.Master.Party;

namespace AccountingSuite.Data.Repositories
{
    public class PartyRepository
    {
        private readonly DbHelperAsync _db;

        public PartyRepository(DbHelperAsync db)
        {
            _db = db;
        }

        //  Get all parties
        public async Task<List<Party>> GetAll()
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spParty_GetAll");

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
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                });
            }
            return list;
        }

        //  Get all parties with state info
        public async Task<List<Party>> GetAllWithState()
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

        //  Create new party
        public async Task<(int newId, string newCode)> Create(Party party)
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spParty_Insert");

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
                var newId = Convert.ToInt32(reader["NewPartyId"]);
                var newCode = reader["NewPartyCode"].ToString()!;

                // Audit entry
                using var auditCmd = _db.CreateCommand(conn, "spTransactionAudit_Insert");
                _db.AddParameter(auditCmd, "@TableName", SqlDbType.NVarChar, "Party", 50);
                _db.AddParameter(auditCmd, "@RecordId", SqlDbType.Int, newId);
                _db.AddParameter(auditCmd, "@Action", SqlDbType.NVarChar, "INSERT", 20);
                _db.AddParameter(auditCmd, "@ActionBy", SqlDbType.NVarChar, party.CreatedBy.ToString(), 50);
                _db.AddParameter(auditCmd, "@Details", SqlDbType.NVarChar,
                    $"Party {party.Name} created with code {newCode}", 250);

                await _db.ExecuteNonQueryAsync(auditCmd);

                return (newId, newCode);
            }
            throw new Exception("Insert failed: no result returned.");
        }

        //  Update party
        public async Task<(int updatedId, string updatedCode)> Update(Party party)
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spParty_Update");

            _db.AddParameter(cmd, "@PartyId", SqlDbType.Int, party.PartyId);
            // _db.AddParameter(cmd, "@PartyCode", SqlDbType.NVarChar, party.PartyCode, 20);
            _db.AddParameter(cmd, "@Name", SqlDbType.NVarChar, party.Name, 100);
            _db.AddParameter(cmd, "@PartyType", SqlDbType.NVarChar, party.PartyType.ToString(), 20);
            _db.AddParameter(cmd, "@GSTIN", SqlDbType.NVarChar, party.GSTIN, 15);
            _db.AddParameter(cmd, "@Address", SqlDbType.NVarChar, party.Address, 250);
            _db.AddParameter(cmd, "@ContactNumber", SqlDbType.NVarChar, party.ContactNumber, 20);
            _db.AddParameter(cmd, "@Email", SqlDbType.NVarChar, party.Email, 100);
            _db.AddParameter(cmd, "@IsActive", SqlDbType.Bit, party.IsActive);
            _db.AddParameter(cmd, "@StateId", SqlDbType.Int, party.StateId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                throw new Exception("Update failed: no result returned.");

            return (
                Convert.ToInt32(reader["UpdatedPartyId"]),
                reader["UpdatedPartyCode"].ToString()!
            );
        }

        //  Get party by ID
        public async Task<Party?> GetById(int id)
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

        //  Delete party
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
