using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountingSuite.Models.Audit;   // ✅ reference your TransactionAudit model

namespace ProjectRoot.Areas.SuperAdmin.Services
{
    public interface ITransactionAuditService
    {
        Task<bool> LogInsertAsync(TransactionAudit audit);
        Task<bool> LogUpdateAsync(TransactionAudit audit);
        Task<bool> LogDeleteAsync(TransactionAudit audit);
        Task<bool> LogBackDatedAsync(TransactionAudit audit);
        Task<IEnumerable<TransactionAudit>> GetAuditLogsAsync(int branchId, DateTime fromDate, DateTime toDate);
    }
}
