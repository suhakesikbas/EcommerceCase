using EcommerceApp.Shared.Infrastructure.EntityFramework.Interfaces;
using MediatR;
using StockService.Api.Domain.Entities;
using StockService.Api.Infrastructure.Persistence;

namespace StockService.Api.Application.Inventories.Commands;

public class UpsertInventoryCommandHandler(
    IQueryRepository<StockDbContext, ProductInventory, int> _queryRepository,
    ICommandRepository<StockDbContext, ProductInventory, int> _commandRepository,
    ILogger<UpsertInventoryCommandHandler> _logger
    ) : IRequestHandler<UpsertInventoryCommand,bool>
{
    public async Task<bool> Handle(UpsertInventoryCommand request, CancellationToken cancellationToken)
    {
        var existing = await _queryRepository.GetAsync(g=> g.ProductId == request.ProductId);
        var item = existing.FirstOrDefault();
        
        if (item is null)
        {
            var inventory = new ProductInventory(request.ProductId, request.Quantity);
            await _commandRepository.InsertAsync(inventory, cancellationToken);
        }
        else
        {
            if (request.Quantity >= 0)
            {
                item.Credit(request.Quantity);
            }
            else
            {
                var debitResult = item.TryDebit(Math.Abs(request.Quantity));
            }

            _commandRepository.Update(item);
        }

        var result = await _commandRepository.SaveAsync(cancellationToken);

        if (result == 0)
        {
            _logger.LogWarning("UpsertInventoryCommandHandler: Failed to save inventory changes for product {ProductId}", request.ProductId);
            return false;
        }

        _logger.LogInformation("UpsertInventoryCommandHandler: Successfully processed inventory upsert for product {ProductId}", request.ProductId);
        return true;
    }
}