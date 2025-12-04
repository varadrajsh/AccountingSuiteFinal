using System;

namespace AccountingSuite.Models.Accounting;

public class AccountBalance
{
    public int AccountBalanceId { get; set; }
    public int BranchId { get; set; }
    public int AccountHeadId { get; set; }
    public DateTime BalanceDate { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal ClosingBalance { get; set; }
    public int? LockedBy { get; set; }
    public DateTime? LockedOn { get; set; }
}
