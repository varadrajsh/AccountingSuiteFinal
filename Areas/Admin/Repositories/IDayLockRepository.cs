using System.Threading.Tasks;
using AccountingSuite.Models.Audit;   // ✅ adjust namespace to match your project

namespace ProjectRoot.Areas.Admin.Repositories
{
    public interface IDayLockRepository
    {
        Task<int> CreateRequestAsync(DayLockRequest request);
        Task<int> CancelRequestAsync(int requestId, int userId);
        Task<int> ApproveRequestAsync(int requestId, int adminId, string reason);
        Task<int> DisapproveRequestAsync(int requestId, int adminId, string reason);
        Task<int> UnlockDayAsync(int requestId, int adminId);
        Task<int> LockDayAsync(int requestId, int userId);
        Task<int> AutoLockMidnightAsync();
    }
}
