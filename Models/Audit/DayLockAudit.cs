using System;

namespace AccountingSuite.Models.Audit;

public class DayLockAudit
{
    public int AuditId { get; set; }
    public int RequestId { get; set; }
    public int BranchId { get; set; }
    public DateTime LockDate { get; set; }
    public string Action { get; set; }
    public int PerformedBy { get; set; }
    public DateTime PerformedOn { get; set; }

    // Approval / Disapproval
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedOn { get; set; }
    public string ApprovalReason { get; set; }
    public int? DisapprovedBy { get; set; }
    public DateTime? DisapprovedOn { get; set; }
    public string DisapprovalReason { get; set; }

    //Cancellation
    public int? CancelledBy { get; set; }
    public DateTime? CancelledOn { get; set; }
    public string? CancellReason { get; set; }

    //AutoLock
    public DateTime? AutoClosedOn { get; set; }

    public string Status { get; set; } 
}
