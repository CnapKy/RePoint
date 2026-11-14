using RePoint.Web.Models;
namespace RePoint.Web.ViewModels;
public class PointSearchViewModel
{
    public string? City { get; set; }
    public int? MaterialId { get; set; }
    public IReadOnlyList<CollectionPoint> Points { get; set; } = Array.Empty<CollectionPoint>();
    public IReadOnlyList<Material> Materials { get; set; } = Array.Empty<Material>();
}
