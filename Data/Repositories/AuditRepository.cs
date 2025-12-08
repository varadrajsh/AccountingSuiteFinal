using System;
using System.Data;
using AccountingSuite.Models.Audit;
using AccountingSuite.Models.Common;

namespace AccountingSuite.Data.Repositories
{
    public class AuditRepository
    {
        private readonly DbHelperAsync _db;
        public AuditRepository(DbHelperAsync db)
        {
            _db = db;
        }

        // Fetch Audit Report with filters + pagination
        public async Task<PaginatedList<TransactionAudit>> GetAuditsReportAsync(
            string role,
            int userId,
            string? modelName = null,
            string? branchSearch = null,
            int? transactionId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 20)
        {
            using var conn = await _db.GetSqlConnectionAsync();
            using var cmd = _db.CreateCommand(conn, "spTransactionAudit_Select");

            _db.AddParameter(cmd, "@Role", SqlDbType.NVarChar, role);
            _db.AddParameter(cmd, "@UserId", SqlDbType.Int, userId);
            _db.AddParameter(cmd, "@ModelName", SqlDbType.NVarChar, modelName ?? (object)DBNull.Value);
            _db.AddParameter(cmd, "@BranchSearch", SqlDbType.NVarChar, branchSearch ?? (object)DBNull.Value);
            _db.AddParameter(cmd, "@TransactionId", SqlDbType.Int, transactionId ?? (object)DBNull.Value);
            _db.AddParameter(cmd, "@FromDate", SqlDbType.DateTime, fromDate ?? (object)DBNull.Value);
            _db.AddParameter(cmd, "@ToDate", SqlDbType.DateTime, toDate ?? (object)DBNull.Value);
            _db.AddParameter(cmd, "@PageNumber", SqlDbType.Int, pageNumber);
            _db.AddParameter(cmd, "@PageSize", SqlDbType.Int, pageSize);

            var audits = new List<TransactionAudit>();
            int totalCount = 0;

            using var reader = await _db.ExecuteReaderAsync(cmd);

            // First result set: paged records
            while (await reader.ReadAsync())
            {
                audits.Add(new TransactionAudit
                {
                    AuditId = reader["AuditId"].ToString(),
                    AuditReportCode = reader["AuditReportCode"].ToString(),
                    Module = reader["Module"].ToString(),
                    ModelName = reader["ModelName"].ToString(),
                    TransactionId = Convert.ToInt32(reader["TransactionId"]),
                    BranchId = Convert.ToInt32(reader["BranchId"]),
                    ActionDate = Convert.ToDateTime(reader["ActionDate"]),
                    Action = reader["Action"].ToString(),
                    PerformedBy = Convert.ToInt32(reader["PerformedBy"]),
                    PerformedOn = Convert.ToDateTime(reader["PerformedOn"]),
                    Status = reader["Status"].ToString(),
                    Reason = reader["Reason"] == DBNull.Value ? null : reader["Reason"].ToString(),
                    ApprovedBy = reader["ApprovedBy"] == DBNull.Value ? null : Convert.ToInt32(reader["ApprovedBy"]),
                    ApprovedOn = reader["ApprovedOn"] == DBNull.Value ? null : Convert.ToDateTime(reader["ApprovedOn"]),
                    ApprovalReason = reader["ApprovalReason"] == DBNull.Value ? null : reader["ApprovalReason"].ToString(),
                    DisapprovedBy = reader["DisapprovedBy"] == DBNull.Value ? null : Convert.ToInt32(reader["DisapprovedBy"]),
                    DisapprovedOn = reader["DisapprovedOn"] == DBNull.Value ? null : Convert.ToDateTime(reader["DisapprovedOn"]),
                    DisapprovalReason = reader["DisapprovalReason"] == DBNull.Value ? null : reader["DisapprovalReason"].ToString()
                });
            }

            // Second result set: total count
            if (await reader.NextResultAsync() && await reader.ReadAsync())
            {
                totalCount = reader.GetInt32(0);
            }

            return new PaginatedList<TransactionAudit>(audits, totalCount, pageNumber, pageSize);
        }
    }
}
