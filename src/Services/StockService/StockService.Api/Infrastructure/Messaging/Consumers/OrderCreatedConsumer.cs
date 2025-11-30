using EcommerceApp.Shared.Contracts.Orders.Events;
using EcommerceApp.Shared.Infrastructure.EntityFramework.Interfaces;
using MassTransit;
using StockService.Api.Domain.Entities;
using StockService.Api.Infrastructure.Persistence;

namespace StockService.Api.Infrastructure.Messaging.Consumers;

public class OrderCreatedConsumer(
    IQueryRepository<StockDbContext, ProductInventory, int> _repository,
    ICommandRepository<StockDbContext, ProductInventory, int> _commandRepository,
    IPublishEndpoint _publishEndpoint,
    ILogger<OrderCreatedConsumer> _logger
    ) : IConsumer<OrderCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
    {
        _logger.LogInformation("StockService: Processing OrderCreatedEvent for Order {OrderId}", context.Message.OrderId);
        
        var failures = new List<string>();
        var touched = new List<ProductInventory>();

        foreach (var item in context.Message.Items)
        {
            var products = await _repository.GetAsync(g=>g.ProductId == item.ProductId);
            var product = products.FirstOrDefault();
            if (product is null)
            {
                failures.Add($"Product {item.ProductId} not found");
                continue;
            }

            if (!product.TryDebit(item.Quantity))
                failures.Add($"Insufficient stock for {item.ProductId}");
            else
                touched.Add(product);
        }

        foreach (var product in touched)
        {
            _commandRepository.Update(product);
        }

        if (failures.Count > 0)
        {
            _logger.LogWarning("StockService: Stock reservation failed for Order {OrderId}. Failures: {Failures}",context.Message.OrderId, string.Join(", ", failures));
            
            var failureEvent = new StockReservationFailedIntegrationEvent(
                context.Message.OrderId,
                context.Message.CustomerId,
                failures
            );

            await _publishEndpoint.Publish(failureEvent, context.CancellationToken);
            _logger.LogInformation("StockService: Published StockReservationFailedEvent for Order {OrderId}", context.Message.OrderId);
        }
        else
        {
            _logger.LogInformation("StockService: Stock successfully reserved for Order {OrderId}", context.Message.OrderId);
            
            var successEvent = new OrderStockReservedIntegrationEvent(
                context.Message.OrderId,
                context.Message.CustomerId,
                context.Message.TotalAmount,
                context.Message.Items
            );

            await _publishEndpoint.Publish(successEvent, context.CancellationToken);
            _logger.LogInformation("StockService: Published OrderStockReservedEvent for Order {OrderId}", context.Message.OrderId);
        }

        await _commandRepository.SaveAsync();

        var success = failures.Count == 0;
    }
}