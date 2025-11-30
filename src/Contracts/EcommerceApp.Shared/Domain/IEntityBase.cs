namespace EcommerceApp.Shared.Domain;

public interface IEntityBase<TKey>
{
    TKey Id { get; }
}