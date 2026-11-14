namespace RePoint.Web.Models;
public class PointMaterial
{
    public int CollectionPointId { get; set; }
    public CollectionPoint CollectionPoint { get; set; } = null!;
    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;
    public bool IsAccepted { get; set; } = true;
}
