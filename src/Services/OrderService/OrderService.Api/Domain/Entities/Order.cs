using EcommerceApp.Shared.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Api.Domain.Entities;

public class Order : IEntityBase<int>
{
    private Order() { }

    public Order(int customerId, IEnumerable<OrderItem> items)
    {
        CustomerId = customerId;
        _items.AddRange(items);
    }

    private readonly List<OrderItem> _items = new();

    public int Id { get; set; }
    public int CustomerId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [NotMapped]
    public decimal TotalAmount => _items.Sum(i => i.Quantity * i.UnitPrice);
    public IReadOnlyCollection<OrderItem> Items => _items;
}

public enum OrderStatus
{
    Pending,
    Processing,
    StockReserved,
    Completed,
    Cancelled,
    Failed
}