namespace FourCasto.Domain.Tests;

using FourCasto.Application.Services;
using FourCasto.Contracts.Enums;
using Xunit;

public class BetZoneEvaluatorTests
{
    private readonly BetZoneEvaluator _evaluator = new();

    [Fact]
    public void Agree_UpSignal_PriceUp_IsPositive()
    {
        var result = _evaluator.Evaluate(BetDirection.AGREE, SignalDirection.UP, 1.00m, 1.05m);
        Assert.True(result.IsInPositiveZone);
    }

    [Fact]
    public void Agree_UpSignal_PriceDown_IsNegative()
    {
        var result = _evaluator.Evaluate(BetDirection.AGREE, SignalDirection.UP, 1.00m, 0.95m);
        Assert.False(result.IsInPositiveZone);
    }

    [Fact]
    public void Disagree_UpSignal_PriceDown_IsPositive()
    {
        var result = _evaluator.Evaluate(BetDirection.DISAGREE, SignalDirection.UP, 1.00m, 0.95m);
        Assert.True(result.IsInPositiveZone);
    }

    [Fact]
    public void Disagree_UpSignal_PriceUp_IsNegative()
    {
        var result = _evaluator.Evaluate(BetDirection.DISAGREE, SignalDirection.UP, 1.00m, 1.05m);
        Assert.False(result.IsInPositiveZone);
    }

    [Fact]
    public void Agree_DownSignal_PriceDown_IsPositive()
    {
        var result = _evaluator.Evaluate(BetDirection.AGREE, SignalDirection.DOWN, 1.00m, 0.95m);
        Assert.True(result.IsInPositiveZone);
    }

    [Fact]
    public void Agree_DownSignal_PriceUp_IsNegative()
    {
        var result = _evaluator.Evaluate(BetDirection.AGREE, SignalDirection.DOWN, 1.00m, 1.05m);
        Assert.False(result.IsInPositiveZone);
    }

    [Fact]
    public void PriceUnchanged_IsNotPositive()
    {
        var result = _evaluator.Evaluate(BetDirection.AGREE, SignalDirection.UP, 1.00m, 1.00m);
        Assert.False(result.IsInPositiveZone);
    }
}
