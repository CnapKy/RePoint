using System.ComponentModel.DataAnnotations;
namespace RePoint.Web.Models;
public class CollectionPoint
{
    public int Id { get; set; }
    [Required, MaxLength(120)] public string Name { get; set; } = string.Empty;
    [Required, MaxLength(120)] public string City { get; set; } = string.Empty;
    [Required, MaxLength(250)] public string Address { get; set; } = string.Empty;
    [Required, MaxLength(250)] public string OpeningHours { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<PointMaterial> AcceptedMaterials { get; set; } = new List<PointMaterial>();
}
