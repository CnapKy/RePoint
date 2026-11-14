namespace RePoint.Web.Models;
public class ConfirmedDelivery
{
    public int Id { get; set; }
    public int HandoverRequestId { get; set; }
    public HandoverRequest HandoverRequest { get; set; } = null!;
    public decimal ActualKg { get; set; }
    public string OperatorUserId { get; set; } = string.Empty;
    public ApplicationUser OperatorUser { get; set; } = null!;
    public DateTime ConfirmedAtUtc { get; set; } = DateTime.UtcNow;
}
