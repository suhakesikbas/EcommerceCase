using EcommerceApp.Shared.Contracts.Orders.Events;
using EcommerceApp.Shared.Infrastructure.EntityFramework.Interfaces;
using MassTransit;
using OrderService.Api.Domain.Entities;
using OrderService.Api.Infrastructure.Persistence;

namespace OrderService.Api.Infrastructure.Messaging.Consumers;

public class OrderStockReservedConsumer(
    IQueryRepository<OrderDbContext, Order, int> _queryRepository,
    ICommandRepository<OrderDbContext, Order, int> _commandRepository,
    ILogger<OrderStockReservedConsumer> _logger
    ) : IConsumer<OrderStockReservedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderStockReservedIntegrationEvent> context)
    {
        _logger.LogInformation("OrderService: Processing OrderStockReservedEvent for Order {OrderId}", context.Message.OrderId);
        
        var orders = await _queryRepository.GetAsync(o => o.Id == context.Message.OrderId);
        var order = orders.FirstOrDefault();
        
        if (order is not null)
        {
            order.Status = OrderStatus.StockReserved;
            _commandRepository.Update(order);
            await _commandRepository.SaveAsync();
            
            _logger.LogInformation("OrderService: Updated Order {OrderId} status to StockReserved", context.Message.OrderId);
        }
        else
        {
            _logger.LogWarning("OrderService: Order {OrderId} not found", context.Message.OrderId);
        }
    }
}