namespace AccountingSuite.Models.Sales
{
    public class Invoice
    {
        public int InvoiceId { get; set; }                // PK
        public int PartyId { get; set; }                  // FK to Party
        public DateTime InvoiceDate { get; set; } = DateTime.Today;
        public string InvoiceNumber { get; set; } = string.Empty; // UK
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";   // Pending | Paid | Overdue

        public List<InvoiceLine> Lines { get; set; } = new(); // Child records
    }
}
