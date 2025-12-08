public class DayLockAudit
{
    public int AuditId { get; set; }
    public int BranchId { get; set; }
    public DateTime LockDate { get; set; }
    public string Action { get; set; }   // "Lock" or "Unlock"
    public int PerformedBy { get; set; }
    public DateTime PerformedOn { get; set; }
    public string Status { get; set; }   // "Locked" or "Unlocked"
}
