using ProRental.Domain.Entities;

namespace ProRental.Interfaces.Module3.P2_1;

public interface IShippingOptionRepository
{
    Task<Order?> FindOrderWithCheckoutAsync(int orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ShippingOption>> FindByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);
    Task<ShippingOption?> FindByIdAsync(int optionId, CancellationToken cancellationToken = default);
    Task AddAsync(ShippingOption option, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<ShippingOption> options, CancellationToken cancellationToken = default);
    Task UpdateAsync(ShippingOption option, CancellationToken cancellationToken = default);
    Task SetCheckoutSelectedOptionAsync(int checkoutId, int optionId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
