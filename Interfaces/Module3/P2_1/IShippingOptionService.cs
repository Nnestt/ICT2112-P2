using ProRental.Models.Module3.P2_1;

namespace ProRental.Interfaces.Module3.P2_1;

public interface IShippingOptionService
{
    Task<IReadOnlyList<ShippingOptionSummary>> GetShippingOptionsForOrderAsync(
        int orderId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShippingOptionSummary>> BuildOptionSetAsync(
        OrderShippingContext context,
        CancellationToken cancellationToken = default);

    Task<ShippingSelectionResult> ApplyCustomerSelectionAsync(
        SelectShippingOptionRequest request,
        CancellationToken cancellationToken = default);
}
