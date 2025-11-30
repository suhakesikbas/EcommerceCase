using MediatR;
using Microsoft.AspNetCore.Mvc;
using StockService.Api.Application.Inventories.Commands;

namespace StockService.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class InventoriesController(IMediator _mediator, ILogger<InventoriesController> _logger) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Upsert([FromBody] UpsertInventoryCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "InventoriesController: Failed to process inventory upsert for product {ProductId}", command.ProductId);
            throw;
        }
    }
}