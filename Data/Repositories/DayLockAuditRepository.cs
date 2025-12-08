using System;
using Microsoft.CodeAnalysis.Elfie.Serialization;

namespace AccountingSuite.Data.Repositories;

public class DayLockAuditRepository
{  
    private readonly DbHelperAsync _db;
    public DayLockAuditRepository(DbHelperAsync db)
    {
        _db = db;
    }

   
}
