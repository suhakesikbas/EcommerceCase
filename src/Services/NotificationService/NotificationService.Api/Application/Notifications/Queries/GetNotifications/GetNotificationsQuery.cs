using MediatR;

namespace NotificationService.Api.Application.Notifications.Queries.GetNotifications;

public record NotificationDto(int NotificationId, int OrderId, string Channel, bool Success, string? Error);

public record GetNotificationsQuery() : IRequest<IReadOnlyCollection<NotificationDto>>;
