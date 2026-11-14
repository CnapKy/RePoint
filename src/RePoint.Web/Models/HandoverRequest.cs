using System.ComponentModel.DataAnnotations;
namespace RePoint.Web.Models;
public enum HandoverStatus { Submitted = 0, Confirmed = 1, Cancelled = 2 }
public class HandoverRequest
{
    public int Id { get; set; }
    [Required] public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public int CollectionPointId { get; set; }
    public CollectionPoint CollectionPoint { get; set; } = null!;
    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;
    public decimal EstimatedKg { get; set; }
    public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;
    public HandoverStatus Status { get; set; } = HandoverStatus.Submitted;
    public ConfirmedDelivery? ConfirmedDelivery { get; set; }
}
