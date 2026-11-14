using Microsoft.EntityFrameworkCore;
using RePoint.Web.Data;
using RePoint.Web.Models;
namespace RePoint.Web.Services;
public sealed class CollectionPointService : ICollectionPointService
{
    private readonly ApplicationDbContext _db;
    public CollectionPointService(ApplicationDbContext db) => _db = db;
    public async Task<IReadOnlyList<CollectionPoint>> SearchAsync(string? city, int? materialId, CancellationToken ct)
    {
        var query = _db.CollectionPoints.AsNoTracking().Where(x => x.IsActive);
        if (!string.IsNullOrWhiteSpace(city))
        {
            var value = city.Trim();
            query = query.Where(x => x.City.Contains(value));
        }
        if (materialId is not null)
            query = query.Where(x => x.AcceptedMaterials.Any(m => m.MaterialId == materialId && m.IsAccepted));
        return await query.OrderBy(x => x.City).ThenBy(x => x.Name).ToListAsync(ct);
    }
}
