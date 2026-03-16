namespace FourCasto.Application.Interfaces;

using FourCasto.Contracts.Enums;

public interface IBalanceService
{
    Task<decimal> GetAvailableBalanceAsync(Guid tradingAccountId);
    Task LockFundsAsync(Guid tradingAccountId, Guid betId, decimal amount, Guid fourCastoWlId, Guid userId);
    Task ReleaseFundsAsync(Guid tradingAccountId, Guid betId, decimal amount, HoldReleaseReason reason, Guid fourCastoWlId);
    Task CreditWinningsAsync(Guid tradingAccountId, decimal amount, Guid betId, Guid fourCastoWlId);
    Task DebitPenaltyAsync(Guid tradingAccountId, decimal amount, Guid betId, Guid fourCastoWlId);
    Task TransferToTradingAccountAsync(Guid walletId, Guid tradingAccountId, decimal amount, Guid fourCastoWlId, Guid userId);
    Task TransferToWalletAsync(Guid tradingAccountId, Guid walletId, decimal amount, Guid fourCastoWlId, Guid userId);
}
