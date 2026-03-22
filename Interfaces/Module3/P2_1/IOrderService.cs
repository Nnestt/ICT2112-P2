using ProRental.Models.Module3.P2_1;

namespace ProRental.Interfaces.Module3.P2_1;

public interface IOrderService
{
    Task<OrderShippingContext?> GetShippingContextAsync(
        int orderId,
        CancellationToken cancellationToken = default);
}
