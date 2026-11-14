using RePoint.Web.Models;
using RePoint.Web.Services;
using Xunit;
namespace RePoint.Tests;
public class HandoverPolicyTests
{
    private readonly HandoverPolicy _policy = new();
    [Theory]
    [InlineData("0", false)]
    [InlineData("-1", false)]
    [InlineData("0.001", true)]
    [InlineData("0.0001", false)]
    [InlineData("1.2345", false)]
    [InlineData("25", true)]
    [InlineData("500", true)]
    [InlineData("500.001", false)]
    [InlineData("501", false)]
    public void IsValidEstimate_EnforcesBounds(string rawKg, bool result)
        => Assert.Equal(result, _policy.IsValidEstimate(decimal.Parse(rawKg,
            System.Globalization.CultureInfo.InvariantCulture)));
    [Fact]
    public void CannotConfirmTwice() => Assert.False(_policy.CanConfirm(HandoverStatus.Submitted, true));
    [Fact]
    public void CannotConfirmCancelled() => Assert.False(_policy.CanConfirm(HandoverStatus.Cancelled, false));
    [Fact]
    public void CanConfirmPending() => Assert.True(_policy.CanConfirm(HandoverStatus.Submitted, false));
}
