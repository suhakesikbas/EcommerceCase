using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Api.Application.Orders.Commands;

namespace OrderService.Api.Controllers;


[Route("api/[controller]")]
[ApiController]
public class OrdersController(IMediator _mediator, ILogger<OrdersController> _logger) : ControllerBase
{

    [HttpPost]
    public async Task<ActionResult<CreateOrderResult>> Create([FromBody] CreateOrderCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("OrdersController: Received create order request for customer {CustomerId} with {ItemCount} items", 
            command.CustomerId, command.Items.Count);
        
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            
            _logger.LogInformation("OrdersController: Successfully created order {OrderId} for customer {CustomerId}", 
                result.OrderId, command.CustomerId);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OrdersController: Failed to create order for customer {CustomerId}", command.CustomerId);
            throw;
        }
    }
}