
namespace AccountingSuite.Models.Master
{
    public class AccountLookup
    {
        public int LookupId { get; set; }               // PK
        public int AccountHeadId { get; set; }          // FK to AccountHead
        public string AccountHeadKeywords { get; set; } // Keyword for typeahead
        public string AccountTypeKeywords { get; set; } // Asset, Liability, Equity, Income, Expense
    }
}
