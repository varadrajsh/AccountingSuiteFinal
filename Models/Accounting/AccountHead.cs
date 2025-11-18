using System;
using System.ComponentModel.DataAnnotations;

namespace AccountingSuite.Models.Accounting
{
    public class AccountHead
    {
         [Key]
        public int AccountHeadId { get; set; }

        [Required(ErrorMessage = "Account Head Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string AccountHeadName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Account Head Type is required")]
        public AccountHeadTypeEnum AccountHeadType { get; set; }

        [StringLength(20)]
        public string? AccountHeadCode { get; set; }

        public bool IsActive { get; set; } = true;

        // Optional hierarchy support
        public int? ParentAccountHeadId { get; set; }
    }

    public enum AccountHeadTypeEnum
    {
        Assets,
        Liabilities,
        Equity,
        Income,
        Expenses
    }

}

