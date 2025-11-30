using EcommerceApp.Shared.Contracts.Orders.Events;
using EcommerceApp.Shared.Infrastructure.EntityFramework.Interfaces;
using MassTransit;
using OrderService.Api.Domain.Entities;
using OrderService.Api.Infrastructure.Persistence;

namespace OrderService.Api.Infrastructure.Messaging.Consumers;

public class StockReservationFailedConsumer(
    IQueryRepository<OrderDbContext, Order, int> _queryRepository,
    ICommandRepository<OrderDbContext, Order, int> _commandRepository,
    ILogger<StockReservationFailedConsumer> _logger
    ) : IConsumer<StockReservationFailedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<StockReservationFailedIntegrationEvent> context)
    {
        
        var orders = await _queryRepository.GetAsync(o => o.Id == context.Message.OrderId);
        var order = orders.FirstOrDefault();
        
        if (order is not null)
        {
            order.Status = OrderStatus.Failed;
            _commandRepository.Update(order);
            await _commandRepository.SaveAsync();
            
            _logger.LogInformation("StockReservationFailedConsumer: Successfully updated Order {OrderId} status to Failed", 
                context.Message.OrderId);
        }
        else
        {
            _logger.LogWarning("StockReservationFailedConsumer: Order {OrderId} not found", context.Message.OrderId);
        }
    }
}