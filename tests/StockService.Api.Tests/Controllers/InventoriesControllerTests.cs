using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StockService.Api.Application.Inventories.Commands;
using StockService.Api.Controllers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace StockService.Api.Tests.Controllers;

public class InventoriesControllerTests
{
    [Fact]
    public async Task Upsert_ReturnsOk_WithMediatorResult()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var loggerMock = new Mock<ILogger<InventoriesController>>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<UpsertInventoryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var controller = new InventoriesController(mediatorMock.Object, loggerMock.Object);
        var command = new UpsertInventoryCommand(1, 5);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.Upsert(command, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!);
        mediatorMock.Verify(m => m.Send(command, cancellationToken), Times.Once);
    }
}