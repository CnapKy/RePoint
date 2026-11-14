using RePoint.Web.Models;
namespace RePoint.Web.Services;
public interface IHandoverPolicy
{
    bool IsValidEstimate(decimal kg);
    bool CanConfirm(HandoverStatus status, bool hasConfirmation);
}
