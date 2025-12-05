using System.Threading.Tasks;
using AccountingSuite.Models.Audit;

namespace AccountingSuite.Areas.Admin.Services
{
    public interface IDayLockService
    {
        Task<bool> RequestUnlockAsync(DayLockRequest request);
        Task<bool> CancelRequestAsync(int requestId, int userId, string currentStatus);
        Task<bool> ApproveRequestAsync(int requestId, int adminId, string reason, string currentStatus);
        Task<bool> DisapproveRequestAsync(int requestId, int adminId, string reason, string currentStatus);
        Task<bool> UnlockDayAsync(int requestId, int adminId);
        Task<bool> LockDayAsync(int requestId, int userId);
        Task<bool> AutoLockMidnightAsync();
    }
}
