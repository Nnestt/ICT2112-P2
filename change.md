# Feature 1 Deferred-Selection Change Notes

## Plan Match Summary

The current implementation matches the deferred-selection plan in the main areas:

- The initial Feature 1 screen shows three preference cards only: `FAST`, `CHEAP`, and `GREEN`.
- `IShippingOptionService` now uses a two-step flow:
  - `GetPreferenceChoicesForOrderAsync(...)`
  - `ConfirmPreferenceSelectionAsync(...)`
- No routing, transport-carbon, or shipping-option persistence happens during the initial GET flow.
- After the customer selects a preference, Feature 1 performs:
  1. one call to `IRoutingService.CreateMultiModalRoute(...)`
  2. one call to `ITransportCarbonService.CalculateRouteQuote(...)`
  3. one persisted `ShippingOption` update/insert plus checkout selection
- `PreferenceTypeModes.AllowedModes` is the source of truth for:
  - `FAST -> [PLANE, TRUCK]`
  - `CHEAP -> [SHIP, TRUCK]`
  - `GREEN -> [TRAIN, TRUCK]`
- The temporary `GetRouteCarbonInput(...)` bridge no longer lives on `IShippingOptionService`.

## Current Caveats

- The public contracts are aligned, but routing is still backed by a local prototype implementation. The contract is correct; the live Feature 3 implementation is not in place yet.
- Reselecting a preference reuses the existing persisted `ShippingOption` for the order instead of always inserting a new row. This still preserves the intended outcome of a single confirmed option per order.

## Class Diagram Scope

This list is intentionally limited to the current classes and interfaces that should appear in the updated diagram for the deferred-selection Feature 1 and Feature 2 flow.

Excluded from this document:

- stub or adapter implementations
- temporary bridges
- controllers
- Razor views
- test doubles
- legacy eager-ranking classes that are no longer on the primary checkout path

## Feature 1

### Service Contract

**`IShippingOptionService`**

- `GetPreferenceChoicesForOrderAsync(int orderId, CancellationToken cancellationToken = default)`
- `ConfirmPreferenceSelectionAsync(SelectShippingPreferenceRequest request, CancellationToken cancellationToken = default)`

### Application Service

**`ShippingOptionManager`**

- `GetPreferenceChoicesForOrderAsync(int orderId, CancellationToken cancellationToken = default)`
- `ConfirmPreferenceSelectionAsync(SelectShippingPreferenceRequest request, CancellationToken cancellationToken = default)`

Role:

- returns the three preference cards for the initial screen
- resolves allowed modes from `PreferenceTypeModes`
- calls routing once after preference selection
- calls transport-carbon once after route creation
- persists the final `ShippingOption`
- writes the selected option to checkout

### Persistence Contract

**`IShippingOptionMapper`**

- `FindOrderWithCheckoutAsync(int orderId, CancellationToken cancellationToken = default)`
- `FindByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)`
- `FindByIdAsync(int optionId, CancellationToken cancellationToken = default)`
- `AddAsync(ShippingOption option, CancellationToken cancellationToken = default)`
- `AddRangeAsync(IEnumerable<ShippingOption> options, CancellationToken cancellationToken = default)`
- `UpdateAsync(ShippingOption option, CancellationToken cancellationToken = default)`
- `SetCheckoutSelectedOptionAsync(int checkoutId, int optionId, CancellationToken cancellationToken = default)`
- `SaveChangesAsync(CancellationToken cancellationToken = default)`

**`ShippingOptionMapper`**

- `FindOrderWithCheckoutAsync(int orderId, CancellationToken cancellationToken = default)`
- `FindByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)`
- `FindByIdAsync(int optionId, CancellationToken cancellationToken = default)`
- `AddAsync(ShippingOption option, CancellationToken cancellationToken = default)`
- `AddRangeAsync(IEnumerable<ShippingOption> options, CancellationToken cancellationToken = default)`
- `UpdateAsync(ShippingOption option, CancellationToken cancellationToken = default)`
- `SetCheckoutSelectedOptionAsync(int checkoutId, int optionId, CancellationToken cancellationToken = default)`
- `SaveChangesAsync(CancellationToken cancellationToken = default)`

### Domain Entity Methods Used By Feature 1

**`ShippingOption`**

- `ConfigureGeneratedOption(int orderId, int? routeId, PreferenceType preferenceType, string displayName, decimal cost, double carbonFootprintKg, int deliveryDays, TransportMode transportMode)`
- `BelongsToOrder(int orderId)`
- `GetSummary()`
- `GetSelectionResult()`

**`DeliveryRoute`**

- `GetRouteId()`
- `GetOriginAddress()`
- `GetDestinationAddress()`
- `GetTotalDistanceKm()`
- `GetIsValid()`
- `GetOriginHubId()`
- `GetDestinationHubId()`
- `GetOrderedRouteLegs()`
- `SetOriginAddress(string originAddress)`
- `SetDestinationAddress(string destinationAddress)`
- `SetTotalDistanceKm(double totalDistanceKm)`
- `SetIsValid(bool isValid)`
- `SetOriginHubId(int? originHubId)`
- `SetDestinationHubId(int? destinationHubId)`

**`RouteLeg`**

- `GetLegId()`
- `GetRouteId()`
- `GetSequence()`
- `GetStartPoint()`
- `GetEndPoint()`
- `GetDistanceKm()`
- `GetIsFirstMile()`
- `GetIsLastMile()`
- `GetTransportMode()`
- `ConfigureLeg(int sequence, string startPoint, string endPoint, double distanceKm, TransportMode transportMode, bool isFirstMile, bool isLastMile)`

### Feature 1 Models

**`ShippingPreferenceCard`**

- data record for the initial three-card preference screen

**`SelectShippingPreferenceRequest`**

- data record for the confirmed customer selection

**`ShippingSelectionResult`**

- data record returned after the final `ShippingOption` is confirmed and persisted

**`ShippingOptionSummary`**

- summary read model returned from `ShippingOption.GetSummary()`

**`PreferenceTypeModes`**

- `AllowedModes`

## Feature 2

### Service Contract

**`ITransportCarbonService`**

- `CalculateLegCarbon(int quantity, double weightKg, double distanceKm, double storageCo2)`
- `CalculateRouteCarbon(IReadOnlyList<double> legCarbonValues)`
- `CalculateLegCarbonSurcharge(int quantity, double weightKg, double distanceKm, double storageCo2, TransportMode transportMode)`
- `CalculateTotalCarbonSurcharge(IReadOnlyList<double> legSurcharges)`
- `CalculateRouteQuote(DeliveryRoute route, int quantity, double weightKg)`

### Application Service

**`TransportCarbonManager`**

- `CalculateLegCarbon(int quantity, double weightKg, double distanceKm, double storageCo2)`
- `CalculateRouteCarbon(IReadOnlyList<double> legCarbonValues)`
- `CalculateLegCarbonSurcharge(int quantity, double weightKg, double distanceKm, double storageCo2, TransportMode transportMode)`
- `CalculateTotalCarbonSurcharge(IReadOnlyList<double> legSurcharges)`
- `CalculateRouteQuote(DeliveryRoute route, int quantity, double weightKg)`

Role:

- iterates the route legs returned from routing
- calculates route carbon from the selected route
- applies per-mode surcharge rules
- returns a route-level quote result to Feature 1

### Pricing Support Used By Feature 2

**`IPricingRuleGateway`**

- `FindActiveRules()`
- `FindByTransportMode(TransportMode mode)`
- `Save(PricingRule rule)`
- `Update(PricingRule rule)`

**`PricingRule`**

- `ReadRuleId()`
- `ReadTransportMode()`
- `ReadBaseRatePerKm()`
- `ReadIsActive()`
- `ReadCarbonSurcharge()`

## External Dependency Used By Feature 1

### Feature 3 Routing Contract

**`IRoutingService`**

- `CreateMultiModalRoute(string origin, string destination, List<TransportMode> modes)`

Role:

- Feature 1 passes a single preference's allowed modes
- routing returns one `DeliveryRoute`
- Feature 1 then forwards that route into Feature 2 for quote calculation

## Recommended Diagram Chain

For the updated deferred-selection diagram, the main flow should read:

`IShippingOptionService / ShippingOptionManager`
-> `IRoutingService`
-> `ITransportCarbonService / TransportCarbonManager`
-> `IShippingOptionMapper / ShippingOptionMapper`
-> `ShippingOption`
-> `DeliveryRoute`
-> `RouteLeg`

The key supporting value objects and records are:

- `ShippingPreferenceCard`
- `SelectShippingPreferenceRequest`
- `ShippingSelectionResult`
- `ShippingOptionSummary`
- `RouteQuoteResult`
- `PreferenceTypeModes`

## Mock Services

The current implementation still contains test-oriented mock services that should stay out of the class diagram:

- `MockRoutingService`
- `MockShippingOptionCarbonInputService`
