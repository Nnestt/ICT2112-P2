using ProRental.Domain.Enums;
using ProRental.Domain.Module3.P2_1.Models;
using ProRental.Interfaces.Module3.P2_1;

namespace ProRental.Domain.Module3.P2_1.Mocks;

/// <summary>
/// Mock transport-carbon fixture bridge that keeps the test endpoint working while
/// Feature 1 no longer exposes route fixtures through its main service contract.
/// by: ernest
/// </summary>
public sealed class MockShippingOptionCarbonInputService : IShippingOptionCarbonInputService
{
    private static readonly IReadOnlyDictionary<int, ShippingOptionCarbonInput> CarbonInputFixtures =
        new Dictionary<int, ShippingOptionCarbonInput>
        {
            [1] = new()
            {
                Quantity = 3,
                ProductId = "P100",
                HubId = "HUB-A",
                RouteLegs =
                [
                    new TransportRouteLegInput { Mode = TransportMode.TRUCK, StartPoint = "Warehouse A", EndPoint = "Port Hub" },
                    new TransportRouteLegInput { Mode = TransportMode.SHIP, StartPoint = "Port Hub", EndPoint = "Destination Port" },
                    new TransportRouteLegInput { Mode = TransportMode.TRUCK, StartPoint = "Destination Port", EndPoint = "Customer" }
                ]
            },
            [2] = new()
            {
                Quantity = 1,
                ProductId = "P200",
                HubId = "HUB-B",
                RouteLegs =
                [
                    new TransportRouteLegInput { Mode = TransportMode.PLANE, StartPoint = "Airport Origin", EndPoint = "Airport Destination" }
                ]
            },
            [3] = new()
            {
                Quantity = 2,
                ProductId = "P300",
                HubId = "HUB-C",
                RouteLegs =
                [
                    new TransportRouteLegInput { Mode = TransportMode.TRAIN, StartPoint = "Rail Hub", EndPoint = "City Hub" },
                    new TransportRouteLegInput { Mode = TransportMode.TRUCK, StartPoint = "City Hub", EndPoint = "Customer" }
                ]
            }
        };

    public ShippingOptionCarbonInput GetRouteCarbonInput(int shippingOptionId)
    {
        if (!CarbonInputFixtures.TryGetValue(shippingOptionId, out var input))
        {
            throw new KeyNotFoundException($"No route carbon input configured for shipping option ID {shippingOptionId}.");
        }

        return input;
    }
}
