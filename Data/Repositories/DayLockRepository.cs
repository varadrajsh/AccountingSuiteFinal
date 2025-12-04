using System;
using System.Data;
using AccountingSuite.Models.Accounting;

namespace AccountingSuite.Data.Repositories;

public class DayLockRepository
{  
    private readonly DbHelper _db;
    public DayLockRepository(DbHelper db)
    {
        _db = db;
    }
    
    // Lock a day
    public void LockDay(int branchId, DateTime lockDate, int userId)
    {
        using var conn = _db.GetConnection();
        using var cmd = _db.CreateCommand(conn, "spLockDay_insert");

        _db.AddParameter(cmd, "@branchId", SqlDbType.Int, branchId);
        _db.AddParameter(cmd, "@lockDate", SqlDbType.Date, lockDate);
        _db.AddParameter(cmd, "@userId", SqlDbType.Int, userId);
        _db.ExecuteNonQuery(cmd);
    }

    // Unlock a day
    public  void UnlockDay(int branchId, DateTime lockDate, int adminId)
    {
        using var conn = _db.GetConnection();
        using var cmd = _db.CreateCommand(conn, "spUnlockDay_update");

        _db.AddParameter(cmd, "@branchId", SqlDbType.Int, branchId);
        _db.AddParameter(cmd,"@lockDate", SqlDbType.Date, lockDate);
        _db.AddParameter(cmd, "@adminId", SqlDbType.Int, adminId);
        _db.ExecuteNonQuery(cmd);
    }

    //Get Lock Status
    public DayLock? GetDayLockStatus(int branchId, DateTime lockDate)
    {
        using var conn = _db.GetConnection();
        using var cmd = _db.CreateCommand(conn, "spDayLock_select");

        _db.AddParameter(cmd, "@branchId", SqlDbType.Int, branchId);
        _db.AddParameter(cmd, "@lockDate", SqlDbType.Date, lockDate);

        var table = _db.ExecuteDataTable(cmd);
        if(table.Rows.Count == 0) return null;

        var row = table.Rows[0];
        return new DayLock
        {
          DayLockId = Convert.ToInt32(row["DayLockId"]),
          BranchId = Convert.ToInt32(row["BranchId"]),
          LockDate = Convert.ToDateTime(row["LockDate"]),
          IsLocked = Convert.ToBoolean(row["IsLocked"]),
          LockedBy = Convert.ToInt32(row["LockedBy"]),
          LockedOn = Convert.ToDateTime(row["LockedOn"]),
          UnlockedBy = row["UnlockedBy"] == DBNull.Value ? null : Convert.ToInt32(row["UnlockedBy"]),
          UnlockedOn = row["UnlockedOn"]  == DBNull.Value ? null : Convert.ToDateTime(row["UnlockedOn"])
        };
    }
}
