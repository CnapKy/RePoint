using RePoint.Web.Models;
namespace RePoint.Web.ViewModels;
public class CampaignListViewModel
{
    public IReadOnlyList<Campaign> Campaigns { get; set; } = Array.Empty<Campaign>();
}
