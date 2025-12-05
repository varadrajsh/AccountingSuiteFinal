using System;
using System.Data;
using AccountingSuite.Data;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSuite.Models.Accounting;

public class AccountBalanceRepository
{
    private readonly DbHelperAsync _db;
    public AccountBalanceRepository(DbHelperAsync db)
    {
        _db = db;
    }

    //Get Balance for Branch / Date
    public async Task<List<AccountBalance>> GetBalancesAsync(int branchId, DateTime balanceDate)
    {
        using var conn = await _db.GetSqlConnectionAsync();
        using var cmd = _db.CreateCommand(conn, "spAccountBalance_select");

        _db.AddParameter(cmd, "@BranchId", SqlDbType.Int, branchId);
        _db.AddParameter(cmd, "@BalanceDate", SqlDbType.DateTime, balanceDate);

        var table = await _db.ExecuteDataTableAsync(cmd);
        var list = new List<AccountBalance>();

        foreach (DataRow row in table.Rows)
        {
            list.Add(new AccountBalance
            {
                    AccountBalanceId = Convert.ToInt32(row["AccountBalanceId"]),
                    BranchId = Convert.ToInt32(row["BranchId"]),
                    AccountHeadId = Convert.ToInt32(row["AccountHeadId"]),
                    BalanceDate = Convert.ToDateTime(row["BalanceDate"]),
                    OpeningBalance = Convert.ToDecimal(row["OpeningBalance"]),
                    ClosingBalance = Convert.ToInt32(row["ClosingBalance"]),
                    LockedBy = row["LockedBy"] == DBNull.Value ? null : Convert.ToInt32("LockedBy"),
                    LockedOn = row["LockedOn"] == DBNull.Value ? null : Convert.ToDateTime("LockedOn")
            });
        }
        return list;
    }

    //Recalculate Closing Balance
    public async Task RecalculateClosingAsync(int branchId, DateTime balanceDate)
    {
        using var conn = await _db.GetSqlConnectionAsync();
        using var cmd = _db.CreateCommand(conn, "spAccountBalance_updateClosing");
        
        _db.AddParameter(cmd, "@BranchId", SqlDbType.Int, branchId);
        _db.AddParameter(cmd, "@BalanceDate", SqlDbType.DateTime, balanceDate);
        
        await _db.ExecuteNonQueryAsync(cmd);
    }

    //Carry Forward Opening Balance
    public async Task CarryForwardOpeningAsync(int branchId, DateTime nextDate)
    {
        using var conn = await _db.GetSqlConnectionAsync();
        using var cmd = _db.CreateCommand(conn, "");
        _db.AddParameter(cmd, "@BranchId", SqlDbType.Int, branchId);
        _db.AddParameter(cmd, "@NextDate", SqlDbType.DateTime, nextDate);
        
        await _db.ExecuteNonQueryAsync(cmd);
    }
}
