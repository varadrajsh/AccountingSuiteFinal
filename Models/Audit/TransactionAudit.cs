using System;

namespace AccountingSuite.Models.Audit
{
    public class TransactionAudit
    {
        public string AuditId { get; set; }              // e.g. AUD-000001 (generated in SQL)
        public string AuditReportCode { get; set; }      // e.g. ARPT-2025-0001 (generated in SQL)
        public string ModelName { get; set; }            // e.g. Party, Branch, State, AccountHead
        public string Module { get; set; }               // e.g. DayLock, Journal, Sale, Party, Branch
        public int TransactionId { get; set; }           // ID of the record in that module
        public int BranchId { get; set; }                // Branch context
        public DateTime ActionDate { get; set; }         // Business date of the transaction
        public string Action { get; set; }               // CRUD/API action (Add, Edit, Delete, Lock, Unlock, Approve, Disapprove, Post)
        public int PerformedBy { get; set; }             // User/Admin/System ID
        public DateTime PerformedOn { get; set; }        // Timestamp of action
        public string Status { get; set; }               // Resulting state (Active, Deleted, Locked, Unlocked, Approved, Disapproved, Posted, Failed)
        public string? Reason { get; set; }              // Optional explanation (approval reason, disapproval reason, error message)

        // Approval/Disapproval details
        public int? ApprovedBy { get; set; }             // Approver ID
        public DateTime? ApprovedOn { get; set; }        // Approval timestamp
        public string? ApprovalReason { get; set; }      // Reason for approval

        public int? DisapprovedBy { get; set; }          // Disapprover ID
        public DateTime? DisapprovedOn { get; set; }     // Disapproval timestamp
        public string? DisapprovalReason { get; set; }   // Reason for disapproval
    }
}
