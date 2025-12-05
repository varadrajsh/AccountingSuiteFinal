using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountingSuite.Models.Audit;

namespace AccountingSuite.Areas.SuperAdmin.Repositories
{
    public interface ITransactionAuditRepository
    {
        Task<int> LogInsertAsync(TransactionAudit audit);
        Task<int> LogUpdateAsync(TransactionAudit audit);
        Task<int> LogDeleteAsync(TransactionAudit audit);
        Task<int> LogBackDatedAsync(TransactionAudit audit);
        Task<IEnumerable<TransactionAudit>> GetAuditLogsAsync(int branchId, DateTime fromDate, DateTime toDate);
    }
}
