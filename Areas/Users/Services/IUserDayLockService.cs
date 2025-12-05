using System;

namespace AccountingSuite.Areas.Users.Services;

public interface IUserDayLockService
{
    Task<bool> RequestUnlockAsync(DayLockRequest request);
    Task<bool> CancelRequestAsync(int requestId, int userId, string currentStatus);
    Task<bool> LockDayAsync(int requestId, int userId);
}
