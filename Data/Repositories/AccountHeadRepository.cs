using AccountingSuite.Infrastructure;
using AccountingSuite.Models.Master;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace AccountingSuite.Data;

public class AccountHeadRepository
{
    private readonly DbHelperAsync _db;

    public AccountHeadRepository(DbHelperAsync db)
    {
        _db = db;
    }

    // Get All AccountHeads
    public async Task<IEnumerable<AccountHead>> GetAllAsync()
    {
        using var conn = await _db.GetSqlConnectionAsync();
        using var cmd = _db.CreateCommand(conn, "spAccountHead_GetAll");
        using var reader = await _db.ExecuteReaderAsync(cmd);

        var list = new List<AccountHead>();
        while (await reader.ReadAsync())
        {
            list.Add(new AccountHead
            {
                AccountHeadId = reader.GetInt32(reader.GetOrdinal("AccountHeadId")),
                AccountHeadCode = reader["AccountHeadCode"].ToString()!,
                AccountHeadName = reader["AccountHeadName"].ToString()!,
                AccountHeadType = reader["AccountHeadType"].ToString()!,
                ParentAccountHeadId = reader["ParentAccountHeadId"] == DBNull.Value ? null : reader.GetInt32(reader.GetOrdinal("ParentAccountHeadId")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            });
        }
        return list;
    }

    // Get AccountHead By ID
    public async Task<AccountHead?> GetByIdAsync(int accountHeadId)
    {
        using var conn = await _db.GetSqlConnectionAsync();
        using var cmd = _db.CreateCommand(conn, "spAccountHead_GetById");
        _db.AddParameter(cmd, "@AccountHeadId", SqlDbType.Int, accountHeadId);

        using var reader = await _db.ExecuteReaderAsync(cmd);
        if (!await reader.ReadAsync()) return null;

        return new AccountHead
        {
            AccountHeadId = reader.GetInt32(reader.GetOrdinal("AccountHeadId")),
            AccountHeadCode = reader["AccountHeadCode"].ToString()!,
            AccountHeadName = reader["AccountHeadName"].ToString()!,
            AccountHeadType = reader["AccountHeadType"].ToString()!,
            ParentAccountHeadId = reader["ParentAccountHeadId"] == DBNull.Value ? null : reader.GetInt32(reader.GetOrdinal("ParentAccountHeadId")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
        };
    }

    // Insert AccountHead
    public async Task<(int newId, string newCode)> CreateAsync(AccountHead accountHead, ModelStateDictionary modelState)
    {
        using var conn = await _db.GetSqlConnectionAsync();
        using var cmd = _db.CreateCommand(conn, "spAccountHead_Insert");

        _db.AddParameter(cmd, "@AccountHeadName", SqlDbType.NVarChar, accountHead.AccountHeadName, 100);
        _db.AddParameter(cmd, "@AccountHeadType", SqlDbType.NVarChar, accountHead.AccountHeadType, 50);
        _db.AddParameter(cmd, "@LookupId", SqlDbType.Int, accountHead.LookupId);
        _db.AddParameter(cmd, "@ParentAccountHeadId", SqlDbType.Int, accountHead.ParentAccountHeadId);

        try
        {
            using var reader = await _db.ExecuteReaderAsync(cmd);
            if (await reader.ReadAsync())
            {
                return (
                    reader.GetInt32(reader.GetOrdinal("NewAccountHeadId")),
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
    }

    // Search AccountLookup
    public async Task<IEnumerable<AccountLookup>> SearchAccountLookupAsync(string term)
    {
        using var conn = await _db.GetSqlConnectionAsync();
        using var cmd = _db.CreateCommand(conn, "spAccountLookup_Search");
        _db.AddParameter(cmd, "@SearchTerm", SqlDbType.NVarChar, term, 100);

        var results = new List<AccountLookup>();
        using var reader = await _db.ExecuteReaderAsync(cmd);
        while (await reader.ReadAsync())
        {
            results.Add(new AccountLookup
            {
                LookupId = reader.GetInt32(reader.GetOrdinal("LookupId")),
                AccountHeadKeywords = reader["AccountHeadKeywords"].ToString()!,
                AccountTypeKeywords = reader["AccountTypeKeywords"].ToString()!
            });
        }
        return results;
    }

   
    // Update Status
    public async Task UpdateStatusAsync(int accountHeadId, bool isActive, ModelStateDictionary modelState)
    {
        using var conn = await _db.GetSqlConnectionAsync();
        using var cmd = _db.CreateCommand(conn, "spAccountHead_UpdateStatus");

        _db.AddParameter(cmd, "@AccountHeadId", SqlDbType.Int, accountHeadId);
        _db.AddParameter(cmd, "@IsActive", SqlDbType.Bit, isActive);

        try
        {
            await _db.ExecuteNonQueryAsync(cmd);

            // Audit entry
            using var auditCmd = _db.CreateCommand(conn, "spTransactionAudit_Insert");
            _db.AddParameter(auditCmd, "@ModelName", SqlDbType.NVarChar, "AccountHead", 100);
            _db.AddParameter(auditCmd, "@Module", SqlDbType.NVarChar, "Master", 100);
            _db.AddParameter(auditCmd, "@TransactionId", SqlDbType.Int, accountHeadId);
            _db.AddParameter(auditCmd, "@BranchId", SqlDbType.Int, branchId);
            _db.AddParameter(auditCmd, "@PerformedBy", SqlDbType.Int, performedBy);
            _db.AddParameter(auditCmd, "@Status", SqlDbType.NVarChar, isActive ? "Active" : "Inactive", 50);
            _db.AddParameter(auditCmd, "@Reason", SqlDbType.NVarChar, "Status toggle", 255);

            await _db.ExecuteNonQueryAsync(auditCmd);
        }
        catch (SqlException ex)
        {
            SqlErrorMapper.Map(ex, modelState);
        }
    }
}
