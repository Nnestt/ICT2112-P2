using ProRental.Data.Module3.P2_1.Interfaces;
using ProRental.Domain.Entities;
using ProRental.Domain.Enums;
using ProRental.Domain.Module3.P2_1.Controls;
using ProRental.Models.Module3.P2_1;

namespace ProRental.Testing;

internal static class TransportCarbonManagerTests
{
    public static IReadOnlyList<PhaseTest> All { get; } =
    [
        new("TransportCarbonManager calculates leg carbon", CalculateLegCarbon_ReturnsExpectedValue),
        new("TransportCarbonManager calculates truck surcharge", CalculateLegCarbonSurcharge_Truck_ReturnsExpectedValue),
        new("TransportCarbonManager calculates ship surcharge", CalculateLegCarbonSurcharge_Ship_ReturnsExpectedValue),
        new("TransportCarbonManager calculates plane surcharge", CalculateLegCarbonSurcharge_Plane_ReturnsExpectedValue),
        new("TransportCarbonManager calculates train surcharge", CalculateLegCarbonSurcharge_Train_ReturnsExpectedValue),
        new("TransportCarbonManager totals leg surcharges", CalculateTotalCarbonSurcharge_ReturnsExpectedValue),
        new("TransportCarbonManager calculates a route-level quote", CalculateRouteQuote_ReturnsExpectedValue)
    ];

    private static void CalculateLegCarbon_ReturnsExpectedValue()
    {
        var manager = CreateManager();

        var result = manager.CalculateLegCarbon(2, 5.0, 10.0, 3.0);

        TestAssertions.AssertEqual(103d, result);
    }

    private static void CalculateLegCarbonSurcharge_Truck_ReturnsExpectedValue()
    {
        var manager = CreateManager();

        var result = manager.CalculateLegCarbonSurcharge(2, 5.0, 10.0, 3.0, TransportMode.TRUCK);

        TestAssertions.AssertEqual(5.15d, result);
    }

    private static void CalculateLegCarbonSurcharge_Ship_ReturnsExpectedValue()
    {
        var manager = CreateManager();

        var result = manager.CalculateLegCarbonSurcharge(2, 5.0, 10.0, 3.0, TransportMode.SHIP);

        TestAssertions.AssertEqual(3.09d, result);
    }

    private static void CalculateLegCarbonSurcharge_Plane_ReturnsExpectedValue()
    {
        var manager = CreateManager();

        var result = manager.CalculateLegCarbonSurcharge(2, 5.0, 10.0, 3.0, TransportMode.PLANE);

        TestAssertions.AssertEqual(12.36d, result);
    }

    private static void CalculateLegCarbonSurcharge_Train_ReturnsExpectedValue()
    {
        var manager = CreateManager();

        var result = manager.CalculateLegCarbonSurcharge(2, 5.0, 10.0, 3.0, TransportMode.TRAIN);

        TestAssertions.AssertEqual(4.12d, result);
    }

    private static void CalculateTotalCarbonSurcharge_ReturnsExpectedValue()
    {
        var manager = CreateManager();

        var result = manager.CalculateTotalCarbonSurcharge([5.15d, 3.09d, 12.36d, 4.12d]);

        TestAssertions.AssertTrue(Math.Abs(result - 24.72d) < 0.0000001d, $"Expected 24.72 but got {result}.");
    }

    private static void CalculateRouteQuote_ReturnsExpectedValue()
    {
        var manager = CreateManager();
        var route = new DeliveryRoute();
        route.SetOriginAddress("Warehouse");
        route.SetDestinationAddress("Customer");
        route.SetIsValid(true);

        var planeLeg = new RouteLeg();
        planeLeg.ConfigureLeg(1, "Warehouse", "Airport Hub", 18d, TransportMode.PLANE, true, false);
        route.RouteLegs.Add(planeLeg);

        var truckLeg = new RouteLeg();
        truckLeg.ConfigureLeg(2, "Airport Hub", "Customer", 8d, TransportMode.TRUCK, false, true);
        route.RouteLegs.Add(truckLeg);

        var quote = manager.CalculateRouteQuote(route, 1, 2d);

        TestAssertions.AssertEqual(new RouteQuoteResult(93.12m, 52d), quote);
    }

    private static TransportCarbonManager CreateManager()
    {
        return new TransportCarbonManager(new StubPricingRuleGateway());
    }

    private sealed class StubPricingRuleGateway : IPricingRuleGateway
    {
        public List<PricingRule> FindActiveRules()
        {
            return [CreateRule(TransportMode.TRUCK, 1.0m, 0.05m)];
        }

        public List<PricingRule> FindByTransportMode(TransportMode mode)
        {
            return
            [
                mode switch
                {
                    TransportMode.PLANE => CreateRule(mode, 2.0m, 0.12m),
                    TransportMode.SHIP => CreateRule(mode, 0.6m, 0.03m),
                    TransportMode.TRAIN => CreateRule(mode, 0.9m, 0.04m),
                    _ => CreateRule(mode, 1.0m, 0.05m)
                }
            ];
        }

        public void Save(PricingRule rule)
        {
        }

        public void Update(PricingRule rule)
        {
        }

        private static PricingRule CreateRule(TransportMode transportMode, decimal baseRatePerKm, decimal surchargeRate)
        {
            var rule = new PricingRule();
            SetPrivateField(rule, "_transportMode", transportMode);
            SetPrivateField(rule, "_baseRatePerKm", baseRatePerKm);
            SetPrivateField(rule, "_carbonSurcharge", surchargeRate);
            SetPrivateField(rule, "_isActive", true);
            return rule;
        }

        private static void SetPrivateField<TTarget, TValue>(TTarget target, string fieldName, TValue value)
        {
            var field = typeof(TTarget).GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?? throw new InvalidOperationException($"Field '{fieldName}' was not found on {typeof(TTarget).Name}.");
            field.SetValue(target, value);
        }
    }
}
