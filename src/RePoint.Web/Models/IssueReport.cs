using System.ComponentModel.DataAnnotations;
namespace RePoint.Web.Models;
public class IssueReport
{
    public int Id { get; set; }
    public int CollectionPointId { get; set; }
    public CollectionPoint CollectionPoint { get; set; } = null!;
    public string? SubmittedByUserId { get; set; }
    [Required, MaxLength(120)] public string Subject { get; set; } = string.Empty;
    [Required, MaxLength(1000)] public string Description { get; set; } = string.Empty;
    [MaxLength(20)] public string Status { get; set; } = "Open";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
