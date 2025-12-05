using System;

namespace AccountingSuite.Models.Audit;

public class DayLockRequest
{
    public int RequestID { get; set; }
    public int BranchId { get; set; }
    public DateTime Lockdate { get; set; }
    public int RequestedBy { get; set; }
    public DateTime RequestedOn { get; set; }
    public string Reason { get; set; }
    public string Status { get; set; }
}
