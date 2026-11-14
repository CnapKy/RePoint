using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RePoint.Web.Data;
using RePoint.Web.ViewModels;
namespace RePoint.Web.Controllers;
public class CampaignsController : Controller
{
    private readonly ApplicationDbContext _db;
    public CampaignsController(ApplicationDbContext db) => _db = db;
    public async Task<IActionResult> Index(CancellationToken ct)
        => View(new CampaignListViewModel { Campaigns = await _db.Campaigns.AsNoTracking()
            .OrderByDescending(x => x.StartsAtUtc).ToListAsync(ct) });
}
