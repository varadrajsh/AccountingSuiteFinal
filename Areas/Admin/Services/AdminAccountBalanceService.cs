using System;
using AccountingSuite.Models.Accounting;

namespace AccountingSuite.Areas.Admin.Services;

public class AdminAccountBalanceService
{
    private readonly AccountBalanceRepository _balanceRepo;
    public AdminAccountBalanceService(AccountBalanceRepository balanceRepo)
    {
        _balanceRepo = balanceRepo;
    }

    //Admin can view balance
    public async Task<List<AccountBalance>> GetBalancesAsync(int branchId, DateTime date)
    {
        return await _balanceRepo.GetBalancesAsync(branchId, date);
    } 

    //Admin can force Recalculation
    public async Task RecalculateClosingAsync(int branchId, DateTime date)
    {
        await _balanceRepo.RecalculateClosingAsync(branchId, date);
    }

    //Admin can carry forward opening balances
    public async Task CarryForwardOpeningAsync(int branchId, DateTime nextDate)
    {
        await _balanceRepo.CarryForwardOpeningAsync(branchId, nextDate);
    } 
}
