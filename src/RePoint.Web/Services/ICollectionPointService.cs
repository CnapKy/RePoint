using RePoint.Web.Models;
namespace RePoint.Web.Services;
public interface ICollectionPointService
{
    Task<IReadOnlyList<CollectionPoint>> SearchAsync(string? city, int? materialId, CancellationToken ct);
}
