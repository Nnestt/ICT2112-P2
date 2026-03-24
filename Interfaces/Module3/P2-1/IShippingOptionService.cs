using ProRental.Models.Module3.P2_1;

namespace ProRental.Interfaces.Module3.P2_1;

/// <summary>
/// Feature 1 application-service boundary used by the MVC controller and checkout flow.
/// by: ernest
/// </summary>
public interface IShippingOptionService
{
    Task<IReadOnlyList<ShippingPreferenceCard>> GetPreferenceChoicesForOrderAsync(
        int orderId,
        CancellationToken cancellationToken = default);

    Task<ShippingSelectionResult> ConfirmPreferenceSelectionAsync(
        SelectShippingPreferenceRequest request,
        CancellationToken cancellationToken = default);
}
