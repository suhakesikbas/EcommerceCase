using MediatR;

namespace OrderService.Api.Application.Orders.Commands;

public record CreateOrderCommand(
    int CustomerId,
    IReadOnlyCollection<CreateOrderItem> Items
) : IRequest<CreateOrderResult>;
public record CreateOrderItem(int ProductId,string ProductName, int Quantity, decimal UnitPrice);

public record CreateOrderResult(int OrderId);