namespace AccountingSuite.Models.Sales
{
    public class InvoiceLine
    {
        public int InvoiceLineId { get; set; }            // PK
        public int InvoiceId { get; set; }                // FK to Invoice
        public string Description { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => Quantity * UnitPrice; // Computed in DB

    }
}
