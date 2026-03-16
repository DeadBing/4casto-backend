namespace FourCasto.Domain.Tests;

using Xunit;

public class BalanceInvariantTests
{
    // Simulates balance state
    private decimal _total = 1000m;
    private decimal _available = 1000m;
    private decimal _locked = 0m;

    private void AssertInvariant() => Assert.Equal(_total, _available + _locked);

    private void LockFunds(decimal amount)
    {
        _available -= amount;
        _locked += amount;
    }

    private void ReleaseFundsAndConsume(decimal amount)
    {
        _locked -= amount;
        _total -= amount;
    }

    private void CreditWinnings(decimal amount)
    {
        _available += amount;
        _total += amount;
    }

    [Fact]
    public void Settlement_Win_Maintains_Invariant()
    {
        // Bet: stake 100, payout 80% -> totalReturn = 180
        var stake = 100m;
        var totalReturn = 180m;

        LockFunds(stake);
        AssertInvariant(); // 1000 = 900 + 100

        ReleaseFundsAndConsume(stake);
        AssertInvariant(); // 900 = 900 + 0

        CreditWinnings(totalReturn);
        AssertInvariant(); // 1080 = 1080 + 0

        Assert.Equal(1080m, _total);
        Assert.Equal(1080m, _available);
        Assert.Equal(0m, _locked);
    }

    [Fact]
    public void Settlement_Loss_Maintains_Invariant()
    {
        var stake = 100m;

        LockFunds(stake);
        AssertInvariant();

        ReleaseFundsAndConsume(stake);
        AssertInvariant();
        // No credit for loss

        Assert.Equal(900m, _total);
        Assert.Equal(900m, _available);
        Assert.Equal(0m, _locked);
    }

    [Fact]
    public void Cancellation_With_Penalty_Maintains_Invariant()
    {
        var stake = 100m;
        var penaltyPercent = 30m;
        var penaltyAmount = stake * penaltyPercent / 100m; // 30
        var refund = stake - penaltyAmount; // 70

        LockFunds(stake);
        AssertInvariant();

        ReleaseFundsAndConsume(stake);
        AssertInvariant(); // 900 = 900 + 0

        CreditWinnings(refund); // credit back 70
        AssertInvariant(); // 970 = 970 + 0

        Assert.Equal(970m, _total);
        Assert.Equal(970m, _available);
        Assert.Equal(0m, _locked);
        // Net loss = penalty = 30. Correct.
    }

    [Fact]
    public void Cancellation_Full_Refund_Maintains_Invariant()
    {
        var stake = 100m;

        LockFunds(stake);
        AssertInvariant();

        ReleaseFundsAndConsume(stake);
        AssertInvariant();

        CreditWinnings(stake); // full refund, 0 penalty
        AssertInvariant();

        Assert.Equal(1000m, _total); // back to starting balance
    }

    [Fact]
    public void Multiple_Bets_Concurrent_Maintains_Invariant()
    {
        LockFunds(100m);
        LockFunds(200m);
        AssertInvariant(); // 1000 = 700 + 300

        // Settle first bet as win (100 stake, 180 return)
        ReleaseFundsAndConsume(100m);
        CreditWinnings(180m);
        AssertInvariant(); // 1080 = 880 + 200

        // Settle second bet as loss (200 stake)
        ReleaseFundsAndConsume(200m);
        AssertInvariant(); // 880 = 880 + 0

        Assert.Equal(880m, _total); // +80 from first, -200 from second
    }
}
