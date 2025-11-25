using System.ComponentModel.DataAnnotations;

namespace AccountingSuite.Models.Master
{
    public class Branch
    {
        [Key]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Branch code is required.")]
        [StringLength(50)]
        public string BranchCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Branch name is required.")]
        [StringLength(100)]
        public string BranchName { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is required.")]
        public int StateId { get; set; }
        public State State { get; set; } = new State();

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(250)]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pin Code is required.")]
        [StringLength(10)]
        public string PinCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Land Number is required.")]
        [StringLength(15)]
        public string LandNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile Number is required.")]
        [StringLength(15)]
        public string MobNumber { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;          // default Active
        public bool IsParcelBooking { get; set; } = false;  // default false
    }
}
