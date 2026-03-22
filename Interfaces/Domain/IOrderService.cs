using ProRental.Models.Module3.P2_1;

namespace ProRental.Interfaces.Domain;

public interface IOrderService
{
    Task<OrderShippingContext?> GetShippingContextAsync(
        int orderId,
        CancellationToken cancellationToken = default);
}
