namespace ProRental.Domain.Module3.P2_1.Models;

public class ShippingOptionCarbonInput
{
    public IReadOnlyList<TransportRouteLegInput> RouteLegs { get; init; } = [];
    public int Quantity { get; init; }
    public string ProductId { get; init; } = string.Empty;
    public string HubId { get; init; } = string.Empty;
}
