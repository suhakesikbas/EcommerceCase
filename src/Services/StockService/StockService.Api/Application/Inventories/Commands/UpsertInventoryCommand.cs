using MediatR;

namespace StockService.Api.Application.Inventories.Commands;

public record UpsertInventoryCommand(int ProductId, int Quantity) : IRequest<bool>;