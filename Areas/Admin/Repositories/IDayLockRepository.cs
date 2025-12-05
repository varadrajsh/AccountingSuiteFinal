using System;
using AccountingSuite.Models.Audit;

namespace AccountingSuite.Areas.Admin.Repositories;

public class IDayLockRepository
{
    Task<int> CreateRequsetAsync(DayLockRequest request);
    Task<int> CancelRequestAsync(int requestId, int userId);
    Task<int> ApproveRequestAsync(int requestId, int adminId, string reason);
    Task<int> DisapprovedRequestAsync(int requestId, int adminId, string reason);
    Task<int> UnlockDayAsync(int requestId, int adminId);
    Task<int> LockDayAsync(int requestId, int userId);
    Task<int> AutoLockMidnightAsync();
}
