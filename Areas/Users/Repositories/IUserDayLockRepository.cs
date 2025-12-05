using System;
using AccountingSuite.Models.Audit;

namespace AccountingSuite.Areas.Users.Repositories;

public class IUserDayLockRepository
{
    Task<int> CreateRequestAsync(DayLockRequest request);
    Task<int> CancelRequestAsync(int requestId, int userId);
    Task<int> LockDayAsync(int requestId, int userId);
}
