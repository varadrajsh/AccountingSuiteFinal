

namespace AccountingSuite.Models.Master
{
    public class AccountHead
    {
        public int AccountHeadId { get; set; }       // PK
        public string AccountHeadCode { get; set; }  // Computed in DB (e.g., AH001)
        public string AccountHeadName { get; set; }  // Required, unique
        public string AccountHeadType { get; set; }  // Asset, Liability, Equity, Income, Expense
        public int? ParentAccountHeadId { get; set; } // Self-reference
        public bool IsActive { get; set; } = true;   // Default true
    }
}
