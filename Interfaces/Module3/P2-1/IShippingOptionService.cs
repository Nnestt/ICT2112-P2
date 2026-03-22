using ProRental.Domain.Module3.P2_1.Models;

namespace ProRental.Interfaces.Module3.P2_1;

public interface IShippingOptionService
{
    ShippingOptionCarbonInput GetRouteCarbonInput(int shippingOptionId);
}
