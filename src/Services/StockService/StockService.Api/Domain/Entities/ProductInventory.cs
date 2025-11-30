using EcommerceApp.Shared.Domain;

namespace StockService.Api.Domain.Entities;

public class ProductInventory : AuditableEntityBase, IEntityBase<int>
{
    public int Id { get; set; }
    public int ProductId { get; private set; }
    public int AvailableQuantity { get; private set; }

    private ProductInventory() { }

    public ProductInventory(int productId, int quantity)
    {
        ProductId = productId;
        AvailableQuantity = quantity;
    }

    public bool TryDebit(int quantity)
    {
        if (AvailableQuantity < quantity)
        {
            return false;
        }

        AvailableQuantity -= quantity;
        Touch();
        return true;
    }

    public void Credit(int quantity)
    {
        AvailableQuantity += quantity;
        Touch();
    }
}