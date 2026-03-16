namespace FourCasto.Domain.Tests;

using FourCasto.Application.Services;
using FourCasto.Contracts.Enums;
using Xunit;

public class SignalProgressCalculatorTests
{
    private readonly SignalProgressCalculator _calc = new();

    [Fact]
    public void NoMovement_ReturnsNeutral()
    {
        var result = _calc.Calculate(1.0m, 1.1m, 0.9m, 1.0m, 90m);
        Assert.Equal(0m, result.ProgressPercent);
        Assert.Equal(ProgressDirection.NEUTRAL, result.Direction);
        Assert.True(result.IsAvailableForBetting);
    }

    [Fact]
    public void HalfwayToTarget_Returns50Percent()
    {
        var result = _calc.Calculate(1.0m, 1.1m, 0.9m, 1.05m, 90m);
        Assert.Equal(50m, result.ProgressPercent);
        Assert.Equal(ProgressDirection.TOWARD_TARGET, result.Direction);
        Assert.True(result.IsAvailableForBetting);
    }

    [Fact]
    public void AtTarget_Returns100Percent()
    {
        var result = _calc.Calculate(1.0m, 1.1m, 0.9m, 1.1m, 90m);
        Assert.Equal(100m, result.ProgressPercent);
        Assert.Equal(ProgressDirection.TOWARD_TARGET, result.Direction);
        Assert.False(result.IsAvailableForBetting); // 100 > 90
    }

    [Fact]
    public void Beyond90Percent_NotAvailable()
    {
        var result = _calc.Calculate(1.0m, 1.1m, 0.9m, 1.095m, 90m);
        Assert.Equal(95m, result.ProgressPercent);
        Assert.False(result.IsAvailableForBetting);
    }

    [Fact]
    public void TowardStop_ReturnsNegativeProgress()
    {
        var result = _calc.Calculate(1.0m, 1.1m, 0.9m, 0.95m, 90m);
        Assert.Equal(-50m, result.ProgressPercent);
        Assert.Equal(ProgressDirection.TOWARD_STOP, result.Direction);
        Assert.True(result.IsAvailableForBetting);
    }
}
