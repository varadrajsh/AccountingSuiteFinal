using System;

namespace AccountingSuite.Areas.SuperAdmin.Services;

public class ITransactionAuditService
{
    Task<bool> LogInsertAsync(TransactionAudit audit);
    Task<bool> LogUpdateAsync(TransactionAudit audit);
    Task<bool> LogDeleteAsync(TransactionAudit audit);
    Task<bool> LogBackDatedAsync(TransactionAudit audit);
    Task<IEnumerable<TransactionAudit>> GetAuditLogsAsync(int branchId, DateTime fromDate, DateTime toDate);
}
