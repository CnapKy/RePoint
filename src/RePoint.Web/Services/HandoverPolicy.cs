using RePoint.Web.Models;
namespace RePoint.Web.Services;
public sealed class HandoverPolicy : IHandoverPolicy
{
    public bool IsValidEstimate(decimal kg)
        => kg is >= 0.001m and <= 500m && decimal.Round(kg, 3) == kg;
    public bool CanConfirm(HandoverStatus status, bool hasConfirmation)
        => status == HandoverStatus.Submitted && !hasConfirmation;
}
