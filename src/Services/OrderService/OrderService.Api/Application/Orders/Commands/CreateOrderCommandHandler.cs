using EcommerceApp.Shared.Contracts.Orders;
using EcommerceApp.Shared.Contracts.Orders.Events;
using EcommerceApp.Shared.Infrastructure.EntityFramework.Interfaces;
using MassTransit;
using MediatR;
using OrderService.Api.Domain.Entities;
using OrderService.Api.Infrastructure.Persistence;

namespace OrderService.Api.Application.Orders.Commands;

public class CreateOrderCommandHandler(
    ICommandRepository<OrderDbContext, Order, int> _commandRepository,
    IPublishEndpoint _publishEndpoint,
    ILogger<CreateOrderCommandHandler> _logger
    ) : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var items = request.Items.Select(i => new OrderItem(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice)).ToList();
        var order = new Order(request.CustomerId, items);

        await _commandRepository.InsertAsync(order);
        await _commandRepository.SaveAsync(cancellationToken);

        if (order.Id > 0)
        {
            var integrationEventItems = order.Items.Select(item => new OrderItemDto(item.ProductId, item.Quantity)).ToList();
            var integrationEvent = new OrderCreatedIntegrationEvent(
                order.Id,
                order.CustomerId.ToString(),
                order.TotalAmount,
                integrationEventItems
            );

            _logger.LogInformation("CreateOrderCommandHandler: Publishing OrderCreatedIntegrationEvent for order {OrderId}", order.Id);
            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            _logger.LogInformation("CreateOrderCommandHandler: Successfully published OrderCreatedIntegrationEvent for order {OrderId}", order.Id);
        }

        return new CreateOrderResult(order.Id);
    }
}