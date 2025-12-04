using System;

namespace AccountingSuite.Models.Accounting;

public class DayLock
{
    public int DayLockId { get; set; }
    public int BranchId { get; set; }
    public DateTime LockDate { get; set; }
    public bool IsLocked { get; set; }
    public int LockedBy { get; set; }
    public DateTime LockedOn { get; set; }
    public int? UnlockedBy { get; set; }
    public DateTime? UnlockedOn { get; set; }

}
