using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RePoint.Web.Data;
using RePoint.Web.ViewModels;
namespace RePoint.Web.Controllers;
public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    public HomeController(ApplicationDbContext db) => _db = db;
    public async Task<IActionResult> Index(CancellationToken ct) => View(new HomeViewModel
    {
        ActivePoints = await _db.CollectionPoints.CountAsync(x => x.IsActive, ct),
        MaterialTypes = await _db.Materials.CountAsync(ct),
        Campaigns = await _db.Campaigns.CountAsync(ct)
    });
    [Route("Home/Error/{code:int}")]
    public IActionResult Error(int code)
    {
        if (code is < 400 or > 599)
            return NotFound();

        Response.StatusCode = code;
        return code switch
        {
            404 or 500 => View($"Error{code}"),
            400 => StatusCode(code, "Невалидна заявка."),
            403 => StatusCode(code, "Достъпът е отказан."),
            _ => StatusCode(code, "Заявката не може да бъде обработена.")
        };
    }
}
