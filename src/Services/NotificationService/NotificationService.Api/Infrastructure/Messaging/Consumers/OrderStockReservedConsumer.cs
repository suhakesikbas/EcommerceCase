using EcommerceApp.Shared.Contracts.Orders.Events;
using EcommerceApp.Shared.Infrastructure.EntityFramework.Interfaces;
using MassTransit;
using NotificationService.Api.Domain.Entities;
using NotificationService.Api.Infrastructure.Persistence;

namespace NotificationService.Api.Infrastructure.Messaging.Consumers;

public class OrderStockReservedConsumer(
    ICommandRepository<NotificationDbContext, NotificationMessage, int> _commandRepository,
    ILogger<OrderStockReservedConsumer> _logger
    ) : IConsumer<OrderStockReservedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderStockReservedIntegrationEvent> context)
    {
        var message = context.Message;
        
        _logger.LogInformation("NotificationService: Processing OrderStockReservedEvent for Order {OrderId}", message.OrderId);
        
        var emailNotification = new NotificationMessage(message.OrderId,"email",$"customer{message.CustomerId}@example.com","Order Confirmation",$"Your order #{message.OrderId} has been confirmed. Total amount: ${message.TotalAmount}");

        var smsNotification = new NotificationMessage(message.OrderId,"sms",$"+1234567890","Order Update",$"Order #{message.OrderId} confirmed. Amount: ${message.TotalAmount}");

        try
        {
            await SimulateSendEmailAsync(emailNotification);
            emailNotification.MarkSuccess();
            
            await SimulateSendSmsAsync(smsNotification);
            smsNotification.MarkSuccess();
            
            _logger.LogInformation("NotificationService: Successfully processed notifications for Order {OrderId}", message.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "NotificationService: Failed to process notifications for Order {OrderId}", message.OrderId);
            emailNotification.MarkFailed($"Email failed: {ex.Message}");
            smsNotification.MarkFailed($"SMS failed: {ex.Message}");
        }

        await _commandRepository.InsertAsync(emailNotification);
        await _commandRepository.InsertAsync(smsNotification);
        await _commandRepository.SaveAsync();
        
        _logger.LogInformation("NotificationService: Saved notification records for Order {OrderId}", message.OrderId);
    }

    private async Task SimulateSendEmailAsync(NotificationMessage notification)
    {
        await Task.Delay(100);
        var message = $"Email sent to {notification.Recipient}: {notification.Subject}";
        _logger.LogInformation(message);
    }

    private async Task SimulateSendSmsAsync(NotificationMessage notification)
    {
        await Task.Delay(50);
        var message = $"SMS sent to {notification.Recipient}: {notification.Body}";
        _logger.LogInformation(message);
    }
}