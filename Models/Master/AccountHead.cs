namespace AccountingSuite.Models.Master
{
    public class AccountHead
    {
        public int AccountHeadId { get; set; }
        public string? AccountHeadCode { get; set; }
        public string AccountHeadName { get; set; } = string.Empty;
        public string AccountHeadType { get; set; } = string.Empty;
        public int LookupId { get; set; }   // FK to AccountLookup
        public int? ParentAccountHeadId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class AccountLookup
    {
        public int LookupId { get; set; }
        public string AccountHeadKeywords { get; set; } = string.Empty;
        public string AccountTypeKeywords { get; set; } = string.Empty;
    }
}
