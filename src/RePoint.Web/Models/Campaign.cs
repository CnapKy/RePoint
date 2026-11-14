using System.ComponentModel.DataAnnotations;
namespace RePoint.Web.Models;
public class Campaign
{
    public int Id { get; set; }
    [Required, MaxLength(160)] public string Title { get; set; } = string.Empty;
    [MaxLength(1000)] public string? Description { get; set; }
    public DateTime StartsAtUtc { get; set; }
    public DateTime EndsAtUtc { get; set; }
    public int? CollectionPointId { get; set; }
    public CollectionPoint? CollectionPoint { get; set; }
}
