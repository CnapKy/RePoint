using RePoint.Web.Models;
namespace RePoint.Web.ViewModels;
public class MyRequestsViewModel
{
    public IReadOnlyList<HandoverRequest> Requests { get; set; } = Array.Empty<HandoverRequest>();
}
