using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RePoint.Web.Data;
using RePoint.Web.Models;
using RePoint.Web.Services;
using RePoint.Web.ViewModels;

namespace RePoint.Web.Controllers;
[Authorize]
public class RequestsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IHandoverPolicy _policy;
    public RequestsController(ApplicationDbContext db, IHandoverPolicy policy)
    { _db = db; _policy = policy; }
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return View(new MyRequestsViewModel { Requests = await _db.HandoverRequests.AsNoTracking()
            .Include(x => x.CollectionPoint).Include(x => x.Material)
            .Where(x => x.UserId == userId).OrderByDescending(x => x.SubmittedAtUtc).ToListAsync(ct) });
    }
    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        await LoadOptions(ct);
        return View(new HandoverCreateViewModel());
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HandoverCreateViewModel vm, CancellationToken ct)
    {
        if (ModelState.IsValid && !_policy.IsValidEstimate(vm.EstimatedKg))
            ModelState.AddModelError(nameof(vm.EstimatedKg), "Посочи количество с най-много три знака след десетичния разделител.");

        if (ModelState.IsValid && !await _db.PointMaterials.AnyAsync(x => x.CollectionPointId == vm.CollectionPointId
            && x.MaterialId == vm.MaterialId && x.IsAccepted && x.CollectionPoint.IsActive, ct))
            ModelState.AddModelError(nameof(vm.MaterialId), "Избраният пункт не приема този материал.");

        if (!ModelState.IsValid)
        {
            await LoadOptions(ct);
            return View(vm);
        }
        var entity = new HandoverRequest
        {
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
            CollectionPointId = vm.CollectionPointId,
            MaterialId = vm.MaterialId,
            EstimatedKg = vm.EstimatedKg,
            SubmittedAtUtc = DateTime.UtcNow
        };
        _db.HandoverRequests.Add(entity);
        await _db.SaveChangesAsync(ct);
        TempData["Notice"] = "Заявката е създадена. Предаването още не е потвърдено.";
        return RedirectToAction(nameof(Index));
    }
    private async Task LoadOptions(CancellationToken ct)
    {
        ViewBag.Points = new SelectList(await _db.CollectionPoints.AsNoTracking()
            .Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync(ct), "Id", "Name");
        ViewBag.Materials = new SelectList(await _db.Materials.AsNoTracking()
            .OrderBy(x => x.Name).ToListAsync(ct), "Id", "Name");
    }
}
