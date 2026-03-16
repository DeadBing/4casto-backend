namespace FourCasto.Application.Services;

using Microsoft.EntityFrameworkCore;
using FourCasto.Application.Interfaces;
using FourCasto.Contracts.Enums;
using FourCasto.Domain.Accounts;
using FourCasto.Infrastructure.Persistence;

public class BalanceService : IBalanceService
{
    private readonly FourCastoDbContext _db;

    public BalanceService(FourCastoDbContext db)
    {
        _db = db;
    }

    public async Task<decimal> GetAvailableBalanceAsync(Guid tradingAccountId)
    {
        var balance = await _db.TradingAccountBalances
            .FirstOrDefaultAsync(b => b.TradingAccountId == tradingAccountId)
            ?? throw new InvalidOperationException($"Balance not found for trading account {tradingAccountId}");

        return balance.AvailableBalance;
    }

    public async Task LockFundsAsync(Guid tradingAccountId, Guid betId, decimal amount, Guid fourCastoWlId, Guid userId)
    {
        var balance = await _db.TradingAccountBalances
            .FirstOrDefaultAsync(b => b.TradingAccountId == tradingAccountId)
            ?? throw new InvalidOperationException($"Balance not found for trading account {tradingAccountId}");

        if (balance.AvailableBalance < amount)
            throw new InvalidOperationException($"Insufficient funds. Available: {balance.AvailableBalance}, Required: {amount}");

        balance.AvailableBalance -= amount;
        balance.LockedBalance += amount;
        balance.UpdatedAt = DateTime.UtcNow;

        // Create hold reservation
        _db.BetHoldReservations.Add(new BetHoldReservation
        {
            FourCastoWlId = fourCastoWlId,
            UserId = userId,
            TradingAccountId = tradingAccountId,
            BetId = betId,
            AmountLocked = amount,
            CurrencyCode = "USD",
            Status = HoldStatus.ACTIVE
        });

        // Create ledger entry
        _db.LedgerEntries.Add(new LedgerEntry
        {
            FourCastoWlId = fourCastoWlId,
            AccountType = LedgerAccountType.TRADING_ACCOUNT,
            AccountId = tradingAccountId,
            BalanceType = BalanceType.REAL,
            EntryType = LedgerEntryType.BET_HOLD,
            Amount = -amount,
            RefType = "Bet",
            RefId = betId,
            BalanceTotalAfter = balance.TotalBalance,
            BalanceAvailableAfter = balance.AvailableBalance,
            BalanceLockedAfter = balance.LockedBalance
        });

        await _db.SaveChangesAsync();
    }

    public async Task ReleaseFundsAsync(Guid tradingAccountId, Guid betId, decimal amount, HoldReleaseReason reason, Guid fourCastoWlId)
    {
        var balance = await _db.TradingAccountBalances
            .FirstOrDefaultAsync(b => b.TradingAccountId == tradingAccountId)
            ?? throw new InvalidOperationException($"Balance not found for trading account {tradingAccountId}");

        var hold = await _db.BetHoldReservations
            .FirstOrDefaultAsync(h => h.BetId == betId && h.Status == HoldStatus.ACTIVE);

        if (hold != null)
        {
            hold.Status = HoldStatus.RELEASED;
            hold.ReleasedAt = DateTime.UtcNow;
            hold.ReleaseReason = reason;
        }

        balance.LockedBalance -= amount;
        balance.UpdatedAt = DateTime.UtcNow;

        var entryType = reason switch
        {
            HoldReleaseReason.SETTLED_WIN => LedgerEntryType.BET_HOLD_RELEASE,
            HoldReleaseReason.SETTLED_LOSS => LedgerEntryType.BET_HOLD_RELEASE,
            HoldReleaseReason.CANCELLED => LedgerEntryType.BET_HOLD_RELEASE,
            _ => LedgerEntryType.BET_HOLD_RELEASE
        };

        _db.LedgerEntries.Add(new LedgerEntry
        {
            FourCastoWlId = fourCastoWlId,
            AccountType = LedgerAccountType.TRADING_ACCOUNT,
            AccountId = tradingAccountId,
            BalanceType = BalanceType.REAL,
            EntryType = entryType,
            Amount = amount,
            RefType = "Bet",
            RefId = betId,
            BalanceTotalAfter = balance.TotalBalance,
            BalanceAvailableAfter = balance.AvailableBalance,
            BalanceLockedAfter = balance.LockedBalance
        });

        await _db.SaveChangesAsync();
    }

    public async Task CreditWinningsAsync(Guid tradingAccountId, decimal amount, Guid betId, Guid fourCastoWlId)
    {
        var balance = await _db.TradingAccountBalances
            .FirstOrDefaultAsync(b => b.TradingAccountId == tradingAccountId)
            ?? throw new InvalidOperationException($"Balance not found");

        balance.AvailableBalance += amount;
        balance.TotalBalance += amount;
        balance.WithdrawableBalance += amount;
        balance.UpdatedAt = DateTime.UtcNow;

        _db.LedgerEntries.Add(new LedgerEntry
        {
            FourCastoWlId = fourCastoWlId,
            AccountType = LedgerAccountType.TRADING_ACCOUNT,
            AccountId = tradingAccountId,
            BalanceType = BalanceType.PNL,
            EntryType = LedgerEntryType.SETTLEMENT_WIN,
            Amount = amount,
            RefType = "Bet",
            RefId = betId,
            BalanceTotalAfter = balance.TotalBalance,
            BalanceAvailableAfter = balance.AvailableBalance,
            BalanceLockedAfter = balance.LockedBalance
        });

        await _db.SaveChangesAsync();
    }

    public async Task DebitPenaltyAsync(Guid tradingAccountId, decimal amount, Guid betId, Guid fourCastoWlId)
    {
        var balance = await _db.TradingAccountBalances
            .FirstOrDefaultAsync(b => b.TradingAccountId == tradingAccountId)
            ?? throw new InvalidOperationException($"Balance not found");

        balance.TotalBalance -= amount;
        balance.UpdatedAt = DateTime.UtcNow;

        _db.LedgerEntries.Add(new LedgerEntry
        {
            FourCastoWlId = fourCastoWlId,
            AccountType = LedgerAccountType.TRADING_ACCOUNT,
            AccountId = tradingAccountId,
            BalanceType = BalanceType.REAL,
            EntryType = LedgerEntryType.CANCELLATION_PENALTY,
            Amount = -amount,
            RefType = "Bet",
            RefId = betId,
            BalanceTotalAfter = balance.TotalBalance,
            BalanceAvailableAfter = balance.AvailableBalance,
            BalanceLockedAfter = balance.LockedBalance
        });

        await _db.SaveChangesAsync();
    }

    public async Task TransferToTradingAccountAsync(Guid walletId, Guid tradingAccountId, decimal amount, Guid fourCastoWlId, Guid userId)
    {
        var walletBalance = await _db.WalletBalances
            .FirstOrDefaultAsync(b => b.WalletId == walletId)
            ?? throw new InvalidOperationException("Wallet balance not found");

        var tradingBalance = await _db.TradingAccountBalances
            .FirstOrDefaultAsync(b => b.TradingAccountId == tradingAccountId)
            ?? throw new InvalidOperationException("Trading account balance not found");

        if (walletBalance.AvailableBalance < amount)
            throw new InvalidOperationException("Insufficient wallet funds");

        walletBalance.AvailableBalance -= amount;
        walletBalance.TotalBalance -= amount;
        walletBalance.UpdatedAt = DateTime.UtcNow;

        tradingBalance.AvailableBalance += amount;
        tradingBalance.TotalBalance += amount;
        tradingBalance.UpdatedAt = DateTime.UtcNow;

        var transferId = Guid.NewGuid();

        _db.LedgerEntries.Add(new LedgerEntry
        {
            FourCastoWlId = fourCastoWlId,
            AccountType = LedgerAccountType.WALLET,
            AccountId = walletId,
            BalanceType = BalanceType.REAL,
            EntryType = LedgerEntryType.TRANSFER_OUT,
            Amount = -amount,
            RefType = "WalletTransfer",
            RefId = transferId,
            BalanceTotalAfter = walletBalance.TotalBalance,
            BalanceAvailableAfter = walletBalance.AvailableBalance,
            BalanceLockedAfter = walletBalance.LockedBalance
        });

        _db.LedgerEntries.Add(new LedgerEntry
        {
            FourCastoWlId = fourCastoWlId,
            AccountType = LedgerAccountType.TRADING_ACCOUNT,
            AccountId = tradingAccountId,
            BalanceType = BalanceType.REAL,
            EntryType = LedgerEntryType.TRANSFER_IN,
            Amount = amount,
            RefType = "WalletTransfer",
            RefId = transferId,
            BalanceTotalAfter = tradingBalance.TotalBalance,
            BalanceAvailableAfter = tradingBalance.AvailableBalance,
            BalanceLockedAfter = tradingBalance.LockedBalance
        });

        await _db.SaveChangesAsync();
    }

    public async Task TransferToWalletAsync(Guid tradingAccountId, Guid walletId, decimal amount, Guid fourCastoWlId, Guid userId)
    {
        var tradingBalance = await _db.TradingAccountBalances
            .FirstOrDefaultAsync(b => b.TradingAccountId == tradingAccountId)
            ?? throw new InvalidOperationException("Trading account balance not found");

        var walletBalance = await _db.WalletBalances
            .FirstOrDefaultAsync(b => b.WalletId == walletId)
            ?? throw new InvalidOperationException("Wallet balance not found");

        if (tradingBalance.AvailableBalance < amount)
            throw new InvalidOperationException("Insufficient trading account funds");

        tradingBalance.AvailableBalance -= amount;
        tradingBalance.TotalBalance -= amount;
        tradingBalance.UpdatedAt = DateTime.UtcNow;

        walletBalance.AvailableBalance += amount;
        walletBalance.TotalBalance += amount;
        walletBalance.UpdatedAt = DateTime.UtcNow;

        var transferId = Guid.NewGuid();

        _db.LedgerEntries.Add(new LedgerEntry
        {
            FourCastoWlId = fourCastoWlId,
            AccountType = LedgerAccountType.TRADING_ACCOUNT,
            AccountId = tradingAccountId,
            BalanceType = BalanceType.REAL,
            EntryType = LedgerEntryType.TRANSFER_OUT,
            Amount = -amount,
            RefType = "WalletTransfer",
            RefId = transferId,
            BalanceTotalAfter = tradingBalance.TotalBalance,
            BalanceAvailableAfter = tradingBalance.AvailableBalance,
            BalanceLockedAfter = tradingBalance.LockedBalance
        });

        _db.LedgerEntries.Add(new LedgerEntry
        {
            FourCastoWlId = fourCastoWlId,
            AccountType = LedgerAccountType.WALLET,
            AccountId = walletId,
            BalanceType = BalanceType.REAL,
            EntryType = LedgerEntryType.TRANSFER_IN,
            Amount = amount,
            RefType = "WalletTransfer",
            RefId = transferId,
            BalanceTotalAfter = walletBalance.TotalBalance,
            BalanceAvailableAfter = walletBalance.AvailableBalance,
            BalanceLockedAfter = walletBalance.LockedBalance
        });

        await _db.SaveChangesAsync();
    }
}
