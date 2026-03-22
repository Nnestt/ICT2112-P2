using ProRental.Domain.Enums;
using ProRental.Domain.Module3.P2_1.Models;
using ProRental.Interfaces.Module3.P2_1;

namespace ProRental.Domain.Module3.P2_1.Controls;

public class ShippingOptionService : IShippingOptionService
{
    private static readonly Dictionary<int, ShippingOptionCarbonInput> TestRoutes = new()
    {
        {
            1,
            new ShippingOptionCarbonInput
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
            }
        },
        {
            2,
            new ShippingOptionCarbonInput
            {
                Quantity = 1,
                ProductId = "P200",
                HubId = "HUB-B",
                RouteLegs =
                [
                    new TransportRouteLegInput { Mode = TransportMode.PLANE, StartPoint = "Airport Origin", EndPoint = "Airport Destination" }
                ]
            }
        },
        {
            3,
            new ShippingOptionCarbonInput
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
        }
    };

    public ShippingOptionCarbonInput GetRouteCarbonInput(int shippingOptionId)
    {
        if (!TestRoutes.TryGetValue(shippingOptionId, out var route))
        {
            throw new KeyNotFoundException($"No test route configured for shipping option ID {shippingOptionId}.");
        }

        return route;
    }
}
