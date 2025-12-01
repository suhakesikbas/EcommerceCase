using MediatR;

namespace OrderService.Api.Application.Orders.Queries.GetOrders;

public record OrderDto(
    int OrderId,
    int CustomerId,
    string Status,
    decimal TotalAmount,
    IReadOnlyCollection<OrderItemDto> Items
);

public record OrderItemDto(
    int OrderItemId,
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice
);

public record GetOrdersQuery() : IRequest<IReadOnlyCollection<OrderDto>>;