using System;
using AccountingSuite.Data.Repositories;
using AccountingSuite.Models.Accounting;

namespace AccountingSuite.Areas.Admin.Services;

public class AdminDayLockService
{
    private readonly DayLockRepository _dayLockRepo;
    private readonly AccountBalanceRepository _balanceRepo;

    public AdminDayLockService(DayLockRepository dayLockRepo, AccountBalanceRepository balanceRepo)
    {
        _dayLockRepo = dayLockRepo;
        _balanceRepo = balanceRepo;
    }

    //Admin Can Lock a day
    public async Task LockDayAsync(int branchId, DateTime date, int adminId)
    {
        await _dayLockRepo.LockDayAsync(branchId, date, adminId);
        await _balanceRepo.RecalculateClosingAsync(branchId, date);
        await _balanceRepo.CarryForwardOpeningAsync(branchId, date.AddDays(1));
    }
    //Admin Can Unlock a day
    public async Task UnlockDayAsync(int branchId, DateTime date, int adminId)
    {
        await _dayLockRepo.UnlockDayAsync(branchId, date, adminId);
        await _balanceRepo.RecalculateClosingAsync(branchId, date);
        await _balanceRepo.CarryForwardOpeningAsync(branchId, date.AddDays(1));
    }

    //Admin can view lock status
    public async Task<DayLock?> GetDayLockStatusAsync(int branchId, DateTime date)
    {
            return await _dayLockRepo.GetDayLockStatusAsync(branchId, date);
    }
}
