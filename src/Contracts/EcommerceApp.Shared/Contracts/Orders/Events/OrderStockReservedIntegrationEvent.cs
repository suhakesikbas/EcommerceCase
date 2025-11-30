namespace EcommerceApp.Shared.Contracts.Orders.Events;

public record OrderStockReservedIntegrationEvent(
    int OrderId,
    string CustomerId,
    decimal TotalAmount,
    IReadOnlyCollection<OrderItemDto> Items
) : IntegrationEvent;