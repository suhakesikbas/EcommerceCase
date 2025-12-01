namespace EcommerceApp.Shared.Contracts.Orders.Events;

public record OrderCreatedIntegrationEvent(
    int OrderId,
    string CustomerId,
    decimal TotalAmount,
    IReadOnlyCollection<OrderItemDto> Items
) : IntegrationEvent;