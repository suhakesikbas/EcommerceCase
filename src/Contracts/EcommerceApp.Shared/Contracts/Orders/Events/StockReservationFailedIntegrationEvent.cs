namespace EcommerceApp.Shared.Contracts.Orders.Events;

public record StockReservationFailedIntegrationEvent(
    int OrderId,
    string CustomerId,
    IReadOnlyCollection<string> FailureReasons
) : IntegrationEvent;