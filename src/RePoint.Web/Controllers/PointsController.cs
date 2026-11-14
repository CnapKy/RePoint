using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RePoint.Web.Data;
using RePoint.Web.Services;
using RePoint.Web.ViewModels;
namespace RePoint.Web.Controllers;
public class PointsController : Controller
{
    private readonly ICollectionPointService _service;
    private readonly ApplicationDbContext _db;
    public PointsController(ICollectionPointService service, ApplicationDbContext db)
    { _service = service; _db = db; }
    public async Task<IActionResult> Index(string? city, int? materialId, CancellationToken ct)
        => View(new PointSearchViewModel
        {
            City = city, MaterialId = materialId,
            Points = await _service.SearchAsync(city, materialId, ct),
            Materials = await _db.Materials.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct)
        });
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var point = await _db.CollectionPoints.AsNoTracking()
            .Include(x => x.AcceptedMaterials).ThenInclude(x => x.Material)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        return point is null ? NotFound() : View(point);
    }
}
