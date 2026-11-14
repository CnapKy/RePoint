using System.ComponentModel.DataAnnotations;
namespace RePoint.Web.Models;
public class Material
{
    public int Id { get; set; }
    [Required, MaxLength(80)] public string Name { get; set; } = string.Empty;
    [Required, MaxLength(24)] public string Code { get; set; } = string.Empty;
    [MaxLength(500)] public string? Description { get; set; }
    public ICollection<PointMaterial> CollectionPoints { get; set; } = new List<PointMaterial>();
}
