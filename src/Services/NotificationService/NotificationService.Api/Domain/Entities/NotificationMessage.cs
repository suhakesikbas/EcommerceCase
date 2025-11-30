using EcommerceApp.Shared.Domain;

namespace NotificationService.Api.Domain.Entities;

public class NotificationMessage : AuditableEntityBase, IEntityBase<int>
{
    public int Id { get; set; }
    public int OrderId { get; private set; }
    public string Channel { get; private set; } = default!;
    public string Recipient { get; private set; } = default!;
    public string Subject { get; private set; } = default!;
    public string Body { get; private set; } = default!;
    public bool Success { get; private set; }
    public string? Error { get; private set; }

    private NotificationMessage() { }

    public NotificationMessage(int orderId, string channel, string recipient, string subject, string body)
    {
        OrderId = orderId;
        Channel = channel;
        Recipient = recipient;
        Subject = subject;
        Body = body;
    }

    public void MarkSuccess()
    {
        Success = true;
        Touch();
    }

    public void MarkFailed(string error)
    {
        Success = false;
        Error = error;
        Touch();
    }
}