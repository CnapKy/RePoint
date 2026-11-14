using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RePoint.Web.Data;
namespace RePoint.Web.Controllers;
public class MaterialsController : Controller
{
    private readonly ApplicationDbContext _db;
    public MaterialsController(ApplicationDbContext db) => _db = db;
    public async Task<IActionResult> Index(CancellationToken ct)
        => View(await _db.Materials.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct));
}
