using System;

namespace AccountingSuite.Models.Audit;

public class TransactionAudit
{
    public int AuditId { get; set; }
    public int TransactionId { get; set; }
    public int BranchId { get; set; }
    public string TableName { get; set; }
    public string Action { get; set; }
    public string OldValues { get; set; }
    public string NewValues { get; set; }
    public int PerformedBy { get; set; }
    public DateTime PerformedOn { get; set; }

    //BackDated Tracking
    public bool IsBackDated { get; set; }
    public DateTime? PreviouslyEnteredDate { get; set; }
    public DateTime? NewDate { get; set; }
    public DateTime? AlteredOn { get; set; }
}
