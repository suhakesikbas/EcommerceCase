using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Api.Application.Notifications.Queries.GetNotifications;

namespace NotificationService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController(IMediator _mediator, ILogger<NotificationsController> _logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<NotificationDto>>> GetAll(CancellationToken cancellationToken)
    {
        
        try
        {
            var items = await _mediator.Send(new GetNotificationsQuery(), cancellationToken);
                        
            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "NotificationsController: Failed to retrieve notifications");
            throw;
        }
    }
}