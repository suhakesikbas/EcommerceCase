using EcommerceApp.Shared.Domain;

namespace OrderService.Api.Domain.Entities;

public class OrderItem : IEntityBase<int>
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;

    private OrderItem() { }

    public OrderItem(int productId,string productName, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        ProductName = productName;
    }
}