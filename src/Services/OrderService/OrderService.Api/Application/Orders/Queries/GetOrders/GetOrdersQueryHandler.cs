using EcommerceApp.Shared.Infrastructure.EntityFramework.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Api.Domain.Entities;
using OrderService.Api.Infrastructure.Persistence;

namespace OrderService.Api.Application.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler(
    OrderDbContext _context,
    ILogger<GetOrdersQueryHandler> _logger
    ) : IRequestHandler<GetOrdersQuery, IReadOnlyCollection<OrderDto>>
{
    public async Task<IReadOnlyCollection<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("GetOrdersQueryHandler: Retrieving all orders from database");
            
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ToListAsync(cancellationToken);
            
            var result = orders.Select(order => new OrderDto(
                order.Id,
                order.CustomerId,
                order.Status.ToString(),
                order.TotalAmount,
                order.Items.Select(item => new OrderItemDto(
                    item.Id,
                    item.ProductId,
                    item.ProductName,
                    item.Quantity,
                    item.UnitPrice,
                    item.TotalPrice
                )).ToArray()
            )).ToArray();
            
            _logger.LogInformation("GetOrdersQueryHandler: Successfully retrieved {Count} orders from database", result.Length);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetOrdersQueryHandler: Failed to retrieve orders from database");
            throw;
        }
    }
}