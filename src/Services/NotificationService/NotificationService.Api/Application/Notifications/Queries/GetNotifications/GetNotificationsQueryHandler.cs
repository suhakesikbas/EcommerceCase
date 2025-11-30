using EcommerceApp.Shared.Infrastructure.EntityFramework.Interfaces;
using MediatR;
using NotificationService.Api.Infrastructure.Persistence;

namespace NotificationService.Api.Application.Notifications.Queries.GetNotifications;

public class GetNotificationsQueryHandler(
    IQueryRepository<NotificationDbContext, Domain.Entities.NotificationMessage, int> _repository,
    ILogger<GetNotificationsQueryHandler> _logger
    ) : IRequestHandler<GetNotificationsQuery, IReadOnlyCollection<NotificationDto>>
{

    public async Task<IReadOnlyCollection<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var notifications = await _repository.GetAsync(null);
            
            var result = notifications.Select(n => new NotificationDto(n.Id, n.OrderId, n.Channel, n.Success, n.Error)).ToArray();
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetNotificationsQueryHandler: Failed to retrieve notifications from database");
            throw;
        }
    }
}