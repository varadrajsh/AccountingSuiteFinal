using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AccountingSuite.Models.Master;
using Microsoft.Data.SqlClient;

namespace AccountingSuite.Data.Repositories
{
    public class BranchRepository
    {
        private readonly DbHelperAsync _db;

        public BranchRepository(DbHelperAsync db)
        {
            _db = db;
        }
        public async Task<List<Branch>> GetAllWithState()
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spBranch_GetAllWithState");

            using var reader = await cmd.ExecuteReaderAsync();
            var list = new List<Branch>();

            while (await reader.ReadAsync())
            {
                list.Add(new Branch
                {
                    BranchId = Convert.ToInt32(reader["BranchId"]),
                    BranchCode = reader["BranchCode"].ToString()!,
                    BranchName = reader["BranchName"].ToString()!,
                    MobNumber = reader["MobNumber"]?.ToString(),
                    StateId = Convert.ToInt32(reader["StateId"]),
                    State = new State
                    {
                        StateId = Convert.ToInt32(reader["StateId"]),
                        StateName = reader["StateName"].ToString()!
                    },
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                });
            }
            return list;
        }
        //  Get all branches
        public async Task<List<Branch>> GetAll()
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spBranch_GetAllWithState"); //  join with State

            using var reader = await cmd.ExecuteReaderAsync();
            var list = new List<Branch>();

            while (await reader.ReadAsync())
            {
                list.Add(new Branch
                {
                    BranchId = Convert.ToInt32(reader["BranchId"]),
                    BranchCode = reader["BranchCode"].ToString()!,
                    BranchName = reader["BranchName"].ToString()!,
                    MobNumber = reader["MobNumber"]?.ToString(), //  populate mobile
                    StateId = Convert.ToInt32(reader["StateId"]),
                    State = new State
                    {
                        StateId = Convert.ToInt32(reader["StateId"]),
                        StateName = reader["StateName"].ToString()!
                    },
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                });
            }
            return list;
        }

        //  Insert branch
        public async Task<int> Insert(Branch branch)
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spBranch_Insert");

            _db.AddParameter(cmd, "@BranchCode", SqlDbType.NVarChar, branch.BranchCode, 20);
            _db.AddParameter(cmd, "@BranchName", SqlDbType.NVarChar, branch.BranchName, 100);
            _db.AddParameter(cmd, "@StateId", SqlDbType.Int, branch.StateId);

            return await _db.ExecuteNonQueryAsync(cmd);
        }

        //  Update branch
        public async Task<int> Update(Branch branch)
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spBranch_Update");

            _db.AddParameter(cmd, "@BranchId", SqlDbType.Int, branch.BranchId);
            _db.AddParameter(cmd, "@BranchName", SqlDbType.NVarChar, branch.BranchName, 100);
            _db.AddParameter(cmd, "@StateId", SqlDbType.Int, branch.StateId);
            _db.AddParameter(cmd, "@IsActive", SqlDbType.Bit, branch.IsActive);
            _db.AddParameter(cmd, "@Email", SqlDbType.NVarChar, branch.Email, 100);
            _db.AddParameter(cmd, "@Address", SqlDbType.NVarChar, branch.Address, 250);
            _db.AddParameter(cmd, "@PinCode", SqlDbType.NVarChar, branch.PinCode, 10);
            _db.AddParameter(cmd, "@LandNumber", SqlDbType.NVarChar, branch.LandNumber, 20);
            _db.AddParameter(cmd, "@MobNumber", SqlDbType.NVarChar, branch.MobNumber, 20);
            _db.AddParameter(cmd, "@IsParcelBooking", SqlDbType.Bit, branch.IsParcelBooking);

            return await _db.ExecuteNonQueryAsync(cmd);
        }

        //  Update branch status
        public async Task<int> UpdateStatus(int branchId, bool isActive)
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spBranch_UpdateStatus");

            _db.AddParameter(cmd, "@BranchId", SqlDbType.Int, branchId);
            _db.AddParameter(cmd, "@IsActive", SqlDbType.Bit, isActive);

            return await _db.ExecuteNonQueryAsync(cmd);
        }

        //  Get branch by ID
        public async Task<Branch?> GetById(int id)
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spBranch_GetById");
            _db.AddParameter(cmd, "@BranchId", SqlDbType.Int, id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;

            return new Branch
            {
                BranchId = Convert.ToInt32(reader["BranchId"]),
                BranchCode = reader["BranchCode"].ToString()!,
                BranchName = reader["BranchName"].ToString()!,
                Email = reader["Email"]?.ToString(),
                Address = reader["Address"]?.ToString(),
                PinCode = reader["PinCode"]?.ToString(),
                LandNumber = reader["LandNumber"]?.ToString(),
                MobNumber = reader["MobNumber"]?.ToString(),
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                IsParcelBooking = Convert.ToBoolean(reader["IsParcelBooking"]),
                StateId = Convert.ToInt32(reader["StateId"]),
                State = new State
                {
                    StateId = Convert.ToInt32(reader["StateId"]),
                    StateName = reader["StateName"].ToString()!
                }
            };
        }

        //  Delete branch
        public async Task<int> Delete(int id)
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spBranch_Delete");
            _db.AddParameter(cmd, "@BranchId", SqlDbType.Int, id);

            return await _db.ExecuteNonQueryAsync(cmd);
        }
    }
}
