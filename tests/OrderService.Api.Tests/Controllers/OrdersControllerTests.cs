using MediatR;
using Moq;
using OrderService.Api.Application.Orders.Commands;
using OrderService.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xunit;

namespace OrderService.Api.Tests.Controllers;

public class OrdersControllerTests
{
    [Fact]
    public async Task Create_ReturnsOk_WithResult()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var loggerMock = new Mock<ILogger<OrdersController>>();
        var command = new CreateOrderCommand(1, new List<CreateOrderItem>{ new(10, "Test Product", 2, 5.5m) });
        var expectedResult = new CreateOrderResult(123);
        mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        var controller = new OrdersController(mediatorMock.Object, loggerMock.Object);

        // Act
        var actionResult = await controller.Create(command, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var value = Assert.IsType<CreateOrderResult>(okResult.Value);
        Assert.Equal(expectedResult.OrderId, value.OrderId);
        mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }
}