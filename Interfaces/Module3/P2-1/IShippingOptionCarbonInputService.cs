using ProRental.Domain.Module3.P2_1.Models;

namespace ProRental.Interfaces.Module3.P2_1;

/// <summary>
/// Temporary bridge used by the transport-carbon test endpoint while Feature 1 moves to
/// a deferred-selection flow that no longer exposes route fixtures on its main service.
/// by: ernest
/// </summary>
public interface IShippingOptionCarbonInputService
{
    ShippingOptionCarbonInput GetRouteCarbonInput(int shippingOptionId);
}
