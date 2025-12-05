using System.Threading.Tasks;
using AccountingSuite.Models.Audit;

namespace AccountingSuite.Areas.Users.Repositories
{
    public interface IUserDayLockRepository
    {
        Task<int> CreateRequestAsync(DayLockRequest request);
        Task<int> CancelRequestAsync(int requestId, int userId);
        Task<int> LockDayAsync(int requestId, int userId);
    }
}
