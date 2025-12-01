namespace EcommerceApp.Shared.Domain;

public abstract class AuditableEntityBase
{
    public DateTime CreatedAtUtc { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; protected set; }

    public void Touch() => UpdatedAtUtc = DateTime.UtcNow;
}