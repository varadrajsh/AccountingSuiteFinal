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
        using var conn = _db.GetConnection();
        using var cmd = _db.CreateCommand(conn, "spAccountHead_Insert");

        _db.AddParameter(cmd, "@AccountHeadName", SqlDbType.NVarChar, accountHead.AccountHeadName, 100);
        _db.AddParameter(cmd, "@AccountHeadType", SqlDbType.NVarChar, accountHead.AccountHeadType, 50);
        _db.AddParameter(cmd, "@LookupId", SqlDbType.Int, accountHead.LookupId);
        _db.AddParameter(cmd, "@ParentAccountHeadId", SqlDbType.Int, accountHead.ParentAccountHeadId);

        try
        {
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return (
                    Convert.ToInt32(reader["NewAccountHeadId"]),
                    reader["NewAccountHeadCode"].ToString()!
                );
            }

            modelState.AddModelError("", "Insert failed: no result returned.");
            return (0, string.Empty);
        }
        catch (SqlException ex)
        {
            SqlErrorMapper.Map(ex, modelState);
            return (0, string.Empty);
        }
        catch (Exception ex)
        {
            modelState.AddModelError("", $"Unexpected error: {ex.Message}");
            return (0, string.Empty);
        }
    }

    public async Task<IEnumerable<AccountLookup>> SearchAccountLookupAsync(string term)
    {
        using var conn = _db.GetConnection();
        using var cmd = _db.CreateCommand(conn, "spAccountLookup_Search");

        _db.AddParameter(cmd, "@SearchTerm", SqlDbType.NVarChar, term, 100);

        var results = new List<AccountLookup>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new AccountLookup
            {
                LookupId = Convert.ToInt32(reader["LookupId"]),
                AccountHeadKeywords = reader["AccountHeadKeywords"].ToString()!,
                AccountTypeKeywords = reader["AccountTypeKeywords"].ToString()!
            });
        }
        return results;
    }

    public void Update(AccountHead accountHead)
    {
        using var conn = _db.GetConnection();
        using var cmd = _db.CreateCommand(conn, "spAccountHead_Update");

        _db.AddParameter(cmd, "@AccountHeadId", SqlDbType.Int, accountHead.AccountHeadId);
        _db.AddParameter(cmd, "@AccountHeadName", SqlDbType.NVarChar, accountHead.AccountHeadName, 100);
        _db.AddParameter(cmd, "@AccountHeadType", SqlDbType.NVarChar, accountHead.AccountHeadType, 50);
        _db.AddParameter(cmd, "@ParentAccountHeadId", SqlDbType.Int, accountHead.ParentAccountHeadId);
        _db.AddParameter(cmd, "@IsActive", SqlDbType.Bit, accountHead.IsActive);

        try
        {
            cmd.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            throw new Exception("Unable to update Account Head. Please check inputs or duplicates.", ex);
        }
    }

    public void UpdateStatus(int accountHeadId, bool isActive)
    {
        using var conn = _db.GetConnection();
        using var cmd = _db.CreateCommand(conn, "spAccountHead_UpdateStatus");

        _db.AddParameter(cmd, "@AccountHeadId", SqlDbType.Int, accountHeadId);
        _db.AddParameter(cmd, "@IsActive", SqlDbType.Bit, isActive);

        try
        {
            cmd.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            throw new Exception("Unable to update Account Head status.", ex);
        }
    }
}
