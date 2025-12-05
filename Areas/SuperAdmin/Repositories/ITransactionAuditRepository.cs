using System;
using System.Transactions;
using AccountingSuite.Models.Audit;

namespace AccountingSuite.Areas.SuperAdmin.Repositories;

public class ITransactionAuditRepository
{
    Task<int> LogInsertAsync(TransactionAudit audit);
    Task<int> LogUpdateAsync(TransactionAudit audit);
    Task<int> LogDeleteAsync(TransactionAudit audit);
    Task<int> LogBackDateAsync(TransactionAudit audit);
    Task<IEnumerable<TransactionAudit>> GetAuditLogAsync(int branchId, DateTime fromDate, DateTime toDate);
}
