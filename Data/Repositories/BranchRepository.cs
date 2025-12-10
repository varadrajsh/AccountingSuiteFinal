using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AccountingSuite.Infrastructure;
using AccountingSuite.Models.Master;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

        // Get all branches with state info
        public async Task<IEnumerable<Branch>> GetAllWithStateAsync()
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spBranch_GetAllWithState");
            using var reader = await _db.ExecuteReaderAsync(cmd);

            var list = new List<Branch>();
            while (await reader.ReadAsync())
            {
                list.Add(new Branch
                {
                    BranchId = reader.GetInt32(reader.GetOrdinal("BranchId")),
                    BranchCode = reader["BranchCode"].ToString()!,
                    BranchName = reader["BranchName"].ToString()!,
                    Email = reader["Email"].ToString()!,
                    Address = reader["Address"].ToString()!,
                    PinCode = reader["PinCode"].ToString()!,
                    LandNumber = reader["LandNumber"].ToString()!,
                    MobNumber = reader["MobNumber"].ToString()!,
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    IsParcelBooking = reader.GetBoolean(reader.GetOrdinal("IsParcelBooking")),
                    StateId = reader.GetInt32(reader.GetOrdinal("StateId")),
                    State = new State
                    {
                        StateId = reader.GetInt32(reader.GetOrdinal("StateId")),
                        StateName = reader["StateName"].ToString()!
                    }
                });
            }
            return list;
        }

        // Get branch by ID
        public async Task<Branch?> GetByIdAsync(int branchId)
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spBranch_GetById");
            _db.AddParameter(cmd, "@BranchId", SqlDbType.Int, branchId);

            using var reader = await _db.ExecuteReaderAsync(cmd);
            if (!await reader.ReadAsync()) return null;

            return new Branch
            {
                BranchId = reader.GetInt32(reader.GetOrdinal("BranchId")),
                BranchCode = reader["BranchCode"].ToString()!,
                BranchName = reader["BranchName"].ToString()!,
                Email = reader["Email"].ToString()!,
                Address = reader["Address"].ToString()!,
                PinCode = reader["PinCode"].ToString()!,
                LandNumber = reader["LandNumber"].ToString()!,
                MobNumber = reader["MobNumber"].ToString()!,
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                IsParcelBooking = reader.GetBoolean(reader.GetOrdinal("IsParcelBooking")),
                StateId = reader.GetInt32(reader.GetOrdinal("StateId")),
                State = new State
                {
                    StateId = reader.GetInt32(reader.GetOrdinal("StateId")),
                    StateName = reader["StateName"].ToString()!
                }
            };
        }

        // Insert branch
        public async Task<int> CreateAsync(Branch branch, ModelStateDictionary modelState)
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spBranch_Insert");

            _db.AddParameter(cmd, "@BranchCode", SqlDbType.NVarChar, branch.BranchCode, 50);
            _db.AddParameter(cmd, "@BranchName", SqlDbType.NVarChar, branch.BranchName, 100);
            _db.AddParameter(cmd, "@StateId", SqlDbType.Int, branch.StateId);
            _db.AddParameter(cmd, "@Email", SqlDbType.NVarChar, branch.Email, 100);
            _db.AddParameter(cmd, "@Address", SqlDbType.NVarChar, branch.Address, 250);
            _db.AddParameter(cmd, "@PinCode", SqlDbType.NVarChar, branch.PinCode, 10);
            _db.AddParameter(cmd, "@LandNumber", SqlDbType.NVarChar, branch.LandNumber, 15);
            _db.AddParameter(cmd, "@MobNumber", SqlDbType.NVarChar, branch.MobNumber, 15);
            _db.AddParameter(cmd, "@IsParcelBooking", SqlDbType.Bit, branch.IsParcelBooking);
            _db.AddParameter(cmd, "@CreatedBy", SqlDbType.Int, branch.CreatedBy);

            try
            {
               using var reader = await _db.ExecuteReaderAsync(cmd);
if (await reader.ReadAsync())
{
    var status = reader["Status"].ToString();
    if (status == "SUCCESS") return 1;

    var errorMsg = reader["ErrorMessage"]?.ToString();
    if (!string.IsNullOrEmpty(errorMsg))
        modelState.AddModelError("", errorMsg);
}
return 0;

            }
            catch (SqlException ex)
            {
                SqlErrorMapper.Map(ex, modelState);
                return 0;
            }
        }


        // Update branch (only editable fields)
        public async Task<int> UpdateAsync(Branch branch, ModelStateDictionary modelState)
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spBranch_Update");

            _db.AddParameter(cmd, "@BranchId", SqlDbType.Int, branch.BranchId);
            _db.AddParameter(cmd, "@Email", SqlDbType.NVarChar, branch.Email, 100);
            _db.AddParameter(cmd, "@Address", SqlDbType.NVarChar, branch.Address, 250);
            _db.AddParameter(cmd, "@PinCode", SqlDbType.NVarChar, branch.PinCode, 10);
            _db.AddParameter(cmd, "@LandNumber", SqlDbType.NVarChar, branch.LandNumber, 15);
            _db.AddParameter(cmd, "@MobNumber", SqlDbType.NVarChar, branch.MobNumber, 15);
            _db.AddParameter(cmd, "@IsParcelBooking", SqlDbType.Bit, branch.IsParcelBooking);
            _db.AddParameter(cmd, "@IsActive", SqlDbType.Bit, branch.IsActive);

            try
            {
                return await _db.ExecuteNonQueryAsync(cmd);
            }
            catch (SqlException ex)
            {
                SqlErrorMapper.Map(ex, modelState);
                return 0;
            }
        }

        // Update status only
        public async Task UpdateStatusAsync(int branchId, bool isActive, ModelStateDictionary modelState)
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spBranch_UpdateStatus");

            _db.AddParameter(cmd, "@BranchId", SqlDbType.Int, branchId);
            _db.AddParameter(cmd, "@IsActive", SqlDbType.Bit, isActive);

            try
            {
                await _db.ExecuteNonQueryAsync(cmd);
            }
            catch (SqlException ex)
            {
                SqlErrorMapper.Map(ex, modelState);
            }
        }
    }
}
