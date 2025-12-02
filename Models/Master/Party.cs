using System.ComponentModel.DataAnnotations;

namespace AccountingSuite.Models.Master
{
    public class Party
    {
        public int PartyId { get; set; }

        public string? PartyCode { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public PartyTypeEnum PartyType { get; set; }

        [StringLength(15)]
        [RegularExpression(@"^[0-9A-Z]{15}$", ErrorMessage = "Invalid GSTIN format.")]
        public string GSTIN { get; set; }

        [StringLength(250)]
        public string Address { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Invalid Contact Number.")]
        public string ContactNumber { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Please select a State.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid State.")]
        public int StateId { get; set; }

        public string? StateName { get; set; }

        public enum PartyTypeEnum
        {
            Customer,
            Vendor,
            Both
        }
    }
}
