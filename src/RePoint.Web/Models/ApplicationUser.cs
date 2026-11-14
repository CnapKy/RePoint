using Microsoft.AspNetCore.Identity;
namespace RePoint.Web.Models;
public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public ICollection<HandoverRequest> HandoverRequests { get; set; } = new List<HandoverRequest>();
}
