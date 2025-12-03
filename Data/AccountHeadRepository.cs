using System;
using System.Data;
using AccountingSuite.Infrastructure;
using AccountingSuite.Models.Master;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;

namespace AccountingSuite.Data;

public class AccountHeadRepository
{
    private readonly DbHelper _db;

    public AccountHeadRepository(DbHelper db)
    {
        _db = db;
    }

    //Get All AccounhHeads
    public async Task<IEnumerable<AccountHead>> GetAllAsync()
    {
        return await Task.Run(() =>
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
                    AccountHeadType = reader["AccountHeadType"].ToString()!,
                    ParentAccountHeadId = reader["ParentAccountHeadId"]
                    == DBNull.Value ? null : Convert.ToInt32(reader["ParentAccountHeadId"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                });
            }
            return list;
        });
    }

    //Get AccountHead By ID
    public async Task<AccountHead?> GetByIdAsync(int accountHead)
    {
        return await Task.Run(() =>
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spAccountHead_GetById");

            _db.AddParameter(cmd, "@AccountHeadId", SqlDbType.Int, accountHead);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new AccountHead
            {
                AccountHeadId = Convert.ToInt32(reader["AccountHeadId"]),
                AccountHeadCode = reader["AccountHeadCode"].ToString()!,
                AccountHeadName = reader["AccountHeadName"].ToString()!,
                AccountHeadType = reader["AccountHeadType"].ToString()!,
                ParentAccountHeadId = reader["ParentAccountHeadId"] 
                    == DBNull.Value ? null : Convert.ToInt32(reader["ParentAccountHeadId"]),
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };
        });
    }

    //Insert AccountHead
    public async Task<(int newId, string newCode)> CreateAsync(AccountHead accountHead, ModelStateDictionary modelState)
    {
        return await Task.Run(() =>
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spAccountHead_Insert");

            _db.AddParameter(cmd, "@AccountHeadName", SqlDbType.NVarChar, accountHead.AccountHeadName, 100);
            _db.AddParameter(cmd, "@AccountHeadType", SqlDbType.NVarChar, accountHead.AccountHeadType, 50);
            _db.AddParameter(cmd, "@ParentAccountHeadId", SqlDbType.Int, accountHead.ParentAccountHeadId);

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
                throw new Exception("Insert failed: no result returned.");
            }
            catch (SqlException ex)
            {
                SqlErrorMapper.Map(ex, modelState);
                return (0, string.Empty);
            }
        });
    }

    // Update only  Active / Inactive status
    public async Task<bool> UpdateStatusAsync(int accountHeadId, bool isActive, ModelStateDictionary modelState)
    {
        return await Task.Run(() =>
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spAccountHead_UpdateStatus");

            _db.AddParameter(cmd, "@AccountHeadId", SqlDbType.Int, accountHeadId);
            _db.AddParameter(cmd, "@IsActive", SqlDbType.Bit, isActive);

            try
            {
                return _db.ExecuteNonQuery(cmd)>0;
            }
            catch (SqlException ex)
            {
                SqlErrorMapper.Map(ex, modelState);
                return false;
            }
        });
    }
}
