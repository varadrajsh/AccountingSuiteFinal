namespace AccountingSuite.Models.Master
{
    public class Party
    {
        public int PartyId { get; set; }

        public string PartyCode { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public enum PartyTypeEnum { Customer, Vendor, Both }
        public PartyTypeEnum PartyType { get; set; } = PartyTypeEnum.Customer;

        public string GSTIN { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string ContactNumber { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
