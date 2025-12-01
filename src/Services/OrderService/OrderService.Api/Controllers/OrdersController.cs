using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Api.Application.Orders.Commands;
using OrderService.Api.Application.Orders.Queries.GetOrders;

namespace OrderService.Api.Controllers;


[Route("api/[controller]")]
[ApiController]
public class OrdersController(IMediator _mediator, ILogger<OrdersController> _logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<OrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetOrdersQuery();
            var result = await _mediator.Send(query, cancellationToken);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OrdersController: Failed to retrieve orders");
            throw;
        }
    }

    [HttpPost]
    public async Task<ActionResult<CreateOrderResult>> Create([FromBody] CreateOrderCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OrdersController: Failed to create order for customer {CustomerId}", command.CustomerId);
            throw;
        }
    }
}