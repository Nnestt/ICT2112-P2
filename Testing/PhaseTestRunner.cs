using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using ProRental.Configuration.Module3.P2_1;
using ProRental.Controllers;
using ProRental.Data.UnitOfWork;
using ProRental.Data.Module3.P2_1;
using ProRental.Data.Module3.P2_1.Gateways;
using ProRental.Data.Module3.P2_1.Interfaces;
using ProRental.Domain.Controls;
using ProRental.Domain.Entities;
using ProRental.Domain.Enums;
using ProRental.Interfaces.Module3.P2_1;
using ProRental.Models.Module3.P2_1;
using System.Reflection;

namespace ProRental.Testing;

internal static class PhaseTestRunner
{
    public static Task<int> RunAsync(string[] args)
    {
        var phase = (args.FirstOrDefault() ?? "phase0").Trim().ToLowerInvariant();

        var tests = phase switch
        {
            "transportcarbon" => TransportCarbonManagerTests.All,
            "phase0" => Phase0Tests.All,
            "phase1" => Phase1Tests.All,
            "phase2" => Phase2Tests.All,
            "phase3" => Phase3Tests.All,
            "phase4" => Phase4Tests.All,
            "phase5" => Phase5Tests.All,
            "phase6" => Phase6Tests.All,
            "phase7" => Phase7Tests.All,
            _ => throw new InvalidOperationException($"Unknown phase '{phase}'.")
        };

        var failures = new List<string>();

        foreach (var test in tests)
        {
            try
            {
                test.Action();
                Console.WriteLine($"PASS {test.Name}");
            }
            catch (Exception ex)
            {
                failures.Add($"{test.Name}: {FormatException(ex)}");
                Console.WriteLine($"FAIL {test.Name}");
            }
        }

        if (failures.Count == 0)
        {
            Console.WriteLine($"All {tests.Count} tests passed for {phase}.");
            return Task.FromResult(0);
        }

        Console.WriteLine($"Phase {phase} failed with {failures.Count} test failure(s):");
        foreach (var failure in failures)
        {
            Console.WriteLine(failure);
        }

        return Task.FromResult(1);
    }

    private static string FormatException(Exception exception)
    {
        var parts = new List<string>();
        Exception? current = exception;

        while (current is not null)
        {
            parts.Add(current.Message);
            current = current.InnerException;
        }

        return string.Join(" | ", parts);
    }
}

internal sealed record PhaseTest(string Name, Action Action);

// Phase 0 locks down the grouped entity accessors and EF mappings that Feature 1 relies on.
internal static class Phase0Tests
{
    public static IReadOnlyList<PhaseTest> All { get; } =
    [
        new("ShippingOption grouped methods round-trip values", ShippingOptionAccessorsRoundTrip),
        new("Checkout grouped methods round-trip values", CheckoutSelectionAccessorsRoundTrip),
        new("Order grouped methods round-trip values", OrderAccessorsRoundTrip),
        new("DeliveryRoute accessors round-trip values", DeliveryRouteAccessorsRoundTrip),
        new("Feature1 enum mappings use snake_case column names", Feature1EnumMappingsUseSnakeCase)
    ];

    private static void ShippingOptionAccessorsRoundTrip()
    {
        var option = new ShippingOption();
        option.ConfigureGeneratedOption(11, 22, PreferenceType.GREEN, "Green Express", 42.50m, 12.75, 4, TransportMode.TRAIN);

        var summary = option.GetSummary();
        var selection = option.GetSelectionResult();

        TestAssertions.AssertEqual("Green Express", summary.DisplayName);
        TestAssertions.AssertEqual(42.50m, summary.Cost);
        TestAssertions.AssertEqual(12.75, summary.CarbonFootprintKg);
        TestAssertions.AssertEqual(4, summary.DeliveryDays);
        TestAssertions.AssertEqual(11, summary.OrderId);
        TestAssertions.AssertEqual(22, summary.RouteId);
        TestAssertions.AssertEqual(PreferenceType.GREEN, summary.PreferenceType);
        TestAssertions.AssertEqual(TransportMode.TRAIN, summary.TransportMode);
        TestAssertions.AssertEqual(summary.OptionId, selection.OptionId);
    }

    private static void CheckoutSelectionAccessorsRoundTrip()
    {
        var checkout = new Checkout();
        var createdAt = new DateTime(2026, 03, 22, 10, 30, 00, DateTimeKind.Utc);

        checkout.Initialize(2, 5, createdAt);
        checkout.SelectShippingOption(9);

        var context = checkout.GetCheckoutContext();
        var selection = checkout.GetSelectionState();

        TestAssertions.AssertEqual(2, context.CustomerId);
        TestAssertions.AssertEqual(5, context.CartId);
        TestAssertions.AssertEqual(createdAt, context.CreatedAt);
        TestAssertions.AssertEqual(9, selection.SelectedOptionId);

        checkout.ClearSelectedShippingOption();

        TestAssertions.AssertNull(checkout.GetSelectionState().SelectedOptionId);
    }

    private static void OrderAccessorsRoundTrip()
    {
        var order = new Order();
        var orderDate = new DateTime(2026, 03, 22, 12, 00, 00, DateTimeKind.Utc);

        order.InitializeForCheckout(3, 7, orderDate, 199.99m, 8);

        var context = order.GetOrderContext();
        var snapshot = order.GetOrderSnapshot();

        TestAssertions.AssertEqual(3, context.CustomerId);
        TestAssertions.AssertEqual(7, context.CheckoutId);
        TestAssertions.AssertEqual(8, snapshot.TransactionId);
        TestAssertions.AssertEqual(orderDate, snapshot.OrderDate);
        TestAssertions.AssertEqual(199.99m, snapshot.TotalAmount);
    }

    private static void DeliveryRouteAccessorsRoundTrip()
    {
        var route = new DeliveryRoute();

        route.SetOriginAddress("Warehouse A");
        route.SetDestinationAddress("Customer B");
        route.SetTotalDistanceKm(128.5);
        route.SetIsValid(true);
        route.SetOriginHubId(14);
        route.SetDestinationHubId(15);

        TestAssertions.AssertEqual("Warehouse A", route.GetOriginAddress());
        TestAssertions.AssertEqual("Customer B", route.GetDestinationAddress());
        TestAssertions.AssertEqual(128.5, route.GetTotalDistanceKm());
        TestAssertions.AssertEqual(true, route.GetIsValid());
        TestAssertions.AssertEqual(14, route.GetOriginHubId());
        TestAssertions.AssertEqual(15, route.GetDestinationHubId());
    }

    private static void Feature1EnumMappingsUseSnakeCase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Database=testrental;Username=test;Password=test")
            .Options;

        using var context = new AppDbContext(options);
        var shippingOption = context.Model.FindEntityType(typeof(ShippingOption))
            ?? throw new InvalidOperationException("ShippingOption entity metadata was not found.");

        var shippingTable = StoreObjectIdentifier.Table("shipping_option", null);

        TestAssertions.AssertEqual(
            "preference_type",
            shippingOption.FindProperty("PreferenceType")?.GetColumnName(shippingTable));
        TestAssertions.AssertEqual(
            "transport_mode",
            shippingOption.FindProperty("TransportMode")?.GetColumnName(shippingTable));
    }

}

// Phase 1 verifies that the public contracts exposed by Feature 1 match the intended service boundaries.
internal static class Phase1Tests
{
    public static IReadOnlyList<PhaseTest> All { get; } =
    [
        new("Shipping option summary carries the full checkout payload", ShippingOptionSummaryCarriesFullCheckoutPayload),
        new("Feature1 service contract exposes the expected async operations", ShippingOptionServiceContractIsStable),
        new("Ranking contract is keyed to shipping-option summary models", RankingContractsUseShippingOptionSummary),
        new("Repository and dependency contracts expose the next-phase entry points", RepositoryAndDependencyContractsArePresent)
    ];

    private static void ShippingOptionSummaryCarriesFullCheckoutPayload()
    {
        var summary = new ShippingOptionSummary(
            OptionId: 1,
            OrderId: 2,
            PreferenceType: PreferenceType.FAST,
            DisplayName: "Fastest",
            Cost: 25.00m,
            CarbonFootprintKg: 14.2,
            DeliveryDays: 2,
            RouteId: 8,
            TransportMode: TransportMode.PLANE,
            TransportModeLabel: "Plane + Truck");

        TestAssertions.AssertEqual(1, summary.OptionId);
        TestAssertions.AssertEqual(2, summary.OrderId);
        TestAssertions.AssertEqual(PreferenceType.FAST, summary.PreferenceType);
        TestAssertions.AssertEqual("Fastest", summary.DisplayName);
        TestAssertions.AssertEqual(25.00m, summary.Cost);
        TestAssertions.AssertEqual(14.2, summary.CarbonFootprintKg);
        TestAssertions.AssertEqual(2, summary.DeliveryDays);
        TestAssertions.AssertEqual(8, summary.RouteId);
        TestAssertions.AssertEqual(TransportMode.PLANE, summary.TransportMode);
        TestAssertions.AssertEqual("Plane + Truck", summary.TransportModeLabel);
    }

    private static void ShippingOptionServiceContractIsStable()
    {
        var contract = typeof(IShippingOptionService);

        AssertMethod(contract, "GetShippingOptionsForOrderAsync", typeof(Task<IReadOnlyList<ShippingOptionSummary>>), typeof(int), typeof(CancellationToken));
        AssertMethod(contract, "BuildOptionSetAsync", typeof(Task<IReadOnlyList<ShippingOptionSummary>>), typeof(OrderShippingContext), typeof(CancellationToken));
        AssertMethod(contract, "ApplyCustomerSelectionAsync", typeof(Task<ShippingSelectionResult>), typeof(SelectShippingOptionRequest), typeof(CancellationToken));
    }

    private static void RankingContractsUseShippingOptionSummary()
    {
        var rankingService = typeof(IRankingService);
        var rankingStrategy = typeof(IRankingStrategy);

        AssertMethod(rankingService, "RankBySpeed", typeof(IReadOnlyList<ShippingOptionSummary>), typeof(IEnumerable<ShippingOptionSummary>));
        AssertMethod(rankingService, "RankByCost", typeof(IReadOnlyList<ShippingOptionSummary>), typeof(IEnumerable<ShippingOptionSummary>));
        AssertMethod(rankingService, "RankByCarbon", typeof(IReadOnlyList<ShippingOptionSummary>), typeof(IEnumerable<ShippingOptionSummary>));

        var preferenceProperty = rankingStrategy.GetProperty(nameof(IRankingStrategy.PreferenceType))
            ?? throw new InvalidOperationException("IRankingStrategy.PreferenceType was not found.");
        TestAssertions.AssertEqual(typeof(PreferenceType), preferenceProperty.PropertyType);

        AssertMethod(rankingStrategy, "Rank", typeof(IReadOnlyList<ShippingOptionSummary>), typeof(IEnumerable<ShippingOptionSummary>));
    }

    private static void RepositoryAndDependencyContractsArePresent()
    {
        var mapper = typeof(IShippingOptionMapper);
        var orderService = typeof(IOrderService);
        var routingService = typeof(IRoutingService);
        var transportCarbonService = typeof(ITransportCarbonService);

        AssertMethod(mapper, "FindOrderWithCheckoutAsync", typeof(Task<Order?>), typeof(int), typeof(CancellationToken));
        AssertMethod(mapper, "FindByOrderIdAsync", typeof(Task<IReadOnlyList<ShippingOption>>), typeof(int), typeof(CancellationToken));
        AssertMethod(mapper, "FindByIdAsync", typeof(Task<ShippingOption?>), typeof(int), typeof(CancellationToken));
        AssertMethod(mapper, "AddAsync", typeof(Task), typeof(ShippingOption), typeof(CancellationToken));
        AssertMethod(mapper, "AddRangeAsync", typeof(Task), typeof(IEnumerable<ShippingOption>), typeof(CancellationToken));
        AssertMethod(mapper, "UpdateAsync", typeof(Task), typeof(ShippingOption), typeof(CancellationToken));
        AssertMethod(mapper, "SetCheckoutSelectedOptionAsync", typeof(Task), typeof(int), typeof(int), typeof(CancellationToken));
        AssertMethod(mapper, "SaveChangesAsync", typeof(Task), typeof(CancellationToken));

        AssertMethod(orderService, "GetShippingContextAsync", typeof(Task<OrderShippingContext?>), typeof(int), typeof(CancellationToken));
        AssertMethod(routingService, "CreateRouteAsync", typeof(Task<DeliveryRoute>), typeof(RoutingRequest), typeof(CancellationToken));
        AssertMethod(transportCarbonService, "CalculateLegCarbon", typeof(double), typeof(int), typeof(double), typeof(double), typeof(double));
        AssertMethod(transportCarbonService, "CalculateRouteCarbon", typeof(double), typeof(IReadOnlyList<double>));
        AssertMethod(transportCarbonService, "CalculateLegCarbonSurcharge", typeof(double), typeof(int), typeof(double), typeof(double), typeof(double), typeof(TransportMode));
        AssertMethod(transportCarbonService, "CalculateTotalCarbonSurcharge", typeof(double), typeof(IReadOnlyList<double>));
    }

    private static void AssertMethod(Type type, string name, Type returnType, params Type[] parameterTypes)
    {
        var method = type.GetMethod(name, parameterTypes)
            ?? throw new InvalidOperationException($"{type.Name}.{name} was not found.");

        TestAssertions.AssertEqual(returnType, method.ReturnType);
    }
}

// Phase 2 covers the ranking subsystem in isolation so strategy behavior stays deterministic.
internal static class Phase2Tests
{
    public static IReadOnlyList<PhaseTest> All { get; } =
    [
        new("Fastest strategy ranks by delivery days with deterministic tie-breakers", FastestStrategyUsesDeterministicOrdering),
        new("Cheapest strategy ranks by cost with deterministic tie-breakers", CheapestStrategyUsesDeterministicOrdering),
        new("Eco-friendly strategy ranks by carbon with deterministic tie-breakers", EcoFriendlyStrategyUsesDeterministicOrdering),
        new("Ranking manager routes each criterion to the matching strategy", RankingManagerUsesRegisteredStrategies),
        new("Ranking manager rejects missing strategies", RankingManagerRejectsMissingStrategies)
    ];

    private static void FastestStrategyUsesDeterministicOrdering()
    {
        var strategy = new FastestStrategy();
        var ranked = strategy.Rank(CreateSampleOptions());

        TestAssertions.AssertSequence(new[] { 4, 2, 1, 3 }, ranked.Select(option => option.OptionId));
    }

    private static void CheapestStrategyUsesDeterministicOrdering()
    {
        var strategy = new CheapestStrategy();
        var ranked = strategy.Rank(CreateSampleOptions());

        TestAssertions.AssertSequence(new[] { 4, 3, 2, 1 }, ranked.Select(option => option.OptionId));
    }

    private static void EcoFriendlyStrategyUsesDeterministicOrdering()
    {
        var strategy = new EcoFriendlyStrategy();
        var ranked = strategy.Rank(CreateSampleOptions());

        TestAssertions.AssertSequence(new[] { 4, 3, 1, 2 }, ranked.Select(option => option.OptionId));
    }

    private static void RankingManagerUsesRegisteredStrategies()
    {
        var manager = new RankingManager(
        [
            new FastestStrategy(),
            new CheapestStrategy(),
            new EcoFriendlyStrategy()
        ]);

        TestAssertions.AssertSequence(new[] { 4, 2, 1, 3 }, manager.RankBySpeed(CreateSampleOptions()).Select(option => option.OptionId));
        TestAssertions.AssertSequence(new[] { 4, 3, 2, 1 }, manager.RankByCost(CreateSampleOptions()).Select(option => option.OptionId));
        TestAssertions.AssertSequence(new[] { 4, 3, 1, 2 }, manager.RankByCarbon(CreateSampleOptions()).Select(option => option.OptionId));
    }

    private static void RankingManagerRejectsMissingStrategies()
    {
        var manager = new RankingManager([new FastestStrategy()]);

        try
        {
            _ = manager.RankByCarbon(CreateSampleOptions());
            throw new InvalidOperationException("Expected RankByCarbon to reject a missing GREEN strategy.");
        }
        catch (InvalidOperationException ex)
        {
            if (!ex.Message.Contains("GREEN", StringComparison.Ordinal))
            {
                throw;
            }
        }
    }

    private static IReadOnlyList<ShippingOptionSummary> CreateSampleOptions() =>
    [
        new ShippingOptionSummary(1, 10, PreferenceType.FAST, "Option 1", 20.00m, 9.0, 3, 100, TransportMode.TRUCK, "Truck"),
        new ShippingOptionSummary(2, 10, PreferenceType.CHEAP, "Option 2", 18.00m, 12.0, 2, 101, TransportMode.PLANE, "Plane"),
        new ShippingOptionSummary(3, 10, PreferenceType.GREEN, "Option 3", 15.00m, 8.0, 4, 102, TransportMode.SHIP, "Ship"),
        new ShippingOptionSummary(4, 10, PreferenceType.GREEN, "Option 4", 15.00m, 6.0, 2, 103, TransportMode.TRAIN, "Train")
    ];
}

// Phase 3 verifies the EF-backed mapper against a real PostgreSQL test fixture.
internal static class Phase3Tests
{
    public static IReadOnlyList<PhaseTest> All { get; } =
    [
        new("Repository can load an order with checkout state", RepositoryCanLoadOrderWithCheckout),
        new("Repository can insert and reload shipping options for an order", RepositoryCanInsertAndReloadShippingOption),
        new("Repository can persist checkout.option_id selection", RepositoryCanPersistCheckoutSelection)
    ];

    private static void RepositoryCanLoadOrderWithCheckout()
    {
        using var context = CreateDbContext();
        using var transaction = context.Database.BeginTransaction();

        var snapshot = CreateOrderFixture(context);
        var repository = new ShippingOptionMapper(context);

        var order = repository.FindOrderWithCheckoutAsync(snapshot.OrderId).GetAwaiter().GetResult()
            ?? throw new InvalidOperationException("Expected repository to return an order.");

        var orderContext = order.GetOrderContext();
        TestAssertions.AssertEqual(snapshot.OrderId, orderContext.OrderId);
        TestAssertions.AssertEqual(snapshot.CheckoutId, orderContext.CheckoutId);
        TestAssertions.AssertTrue(order.Checkout is not null, "Expected order checkout navigation to be loaded.");

        transaction.Rollback();
    }

    private static void RepositoryCanInsertAndReloadShippingOption()
    {
        using var context = CreateDbContext();
        using var transaction = context.Database.BeginTransaction();

        var snapshot = CreateOrderFixture(context);
        var repository = new ShippingOptionMapper(context);
        var route = CreateRoute(context);

        var option = new ShippingOption();
        option.ConfigureGeneratedOption(snapshot.OrderId, route.GetRouteId(), PreferenceType.GREEN, "Phase 3 Test Option", 31.25m, 7.5, 4, TransportMode.TRAIN);

        repository.AddAsync(option).GetAwaiter().GetResult();
        repository.SaveChangesAsync().GetAwaiter().GetResult();

        var insertedSummary = option.GetSummary();
        TestAssertions.AssertTrue(insertedSummary.OptionId > 0, "Expected inserted shipping option to receive an identity value.");

        context.ChangeTracker.Clear();

        var reloaded = repository.FindByIdAsync(insertedSummary.OptionId).GetAwaiter().GetResult()
            ?? throw new InvalidOperationException("Expected inserted shipping option to be queryable by id.");
        var orderOptions = repository.FindByOrderIdAsync(snapshot.OrderId).GetAwaiter().GetResult();
        var reloadedSummary = reloaded.GetSummary();

        TestAssertions.AssertEqual("Phase 3 Test Option", reloadedSummary.DisplayName);
        TestAssertions.AssertEqual(PreferenceType.GREEN, reloadedSummary.PreferenceType);
        TestAssertions.AssertTrue(orderOptions.Any(item => item.GetSummary().OptionId == insertedSummary.OptionId), "Expected inserted option to be returned by order lookup.");

        transaction.Rollback();
    }

    private static void RepositoryCanPersistCheckoutSelection()
    {
        using var context = CreateDbContext();
        using var transaction = context.Database.BeginTransaction();

        var snapshot = CreateOrderFixture(context);
        var repository = new ShippingOptionMapper(context);
        var route = CreateRoute(context);
        var option = CreateShippingOption(snapshot.OrderId, route.GetRouteId(), PreferenceType.CHEAP, TransportMode.TRUCK, "Selected Option");

        repository.AddAsync(option).GetAwaiter().GetResult();
        repository.SaveChangesAsync().GetAwaiter().GetResult();
        repository.SetCheckoutSelectedOptionAsync(snapshot.CheckoutId, option.GetSummary().OptionId).GetAwaiter().GetResult();
        repository.SaveChangesAsync().GetAwaiter().GetResult();

        context.ChangeTracker.Clear();

        var checkout = context.Checkouts
            .First(entity => EF.Property<int>(entity, "Checkoutid") == snapshot.CheckoutId);

        TestAssertions.AssertEqual(option.GetSummary().OptionId, checkout.GetSelectionState().SelectedOptionId);

        transaction.Rollback();
    }

    internal static AppDbContext CreateDbContext()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' was not found.");

        var translator = new Npgsql.NameTranslation.NpgsqlNullNameTranslator();
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.MapEnum<CartStatus>("cart_status_enum", translator);
        dataSourceBuilder.MapEnum<CheckoutStatus>("checkout_status_enum", translator);
        dataSourceBuilder.MapEnum<DeliveryDuration>("delivery_duration_enum", translator);
        dataSourceBuilder.MapEnum<PaymentMethod>("payment_method_enum", translator);
        dataSourceBuilder.MapEnum<OrderStatus>("order_status_enum", translator);
        dataSourceBuilder.MapEnum<UserRole>("user_role_enum", translator);
        dataSourceBuilder.MapEnum<PreferenceType>("preference_type", translator);
        dataSourceBuilder.MapEnum<TransportMode>("transport_mode", translator);

        var dataSource = dataSourceBuilder.Build();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(dataSource, builder =>
            {
                builder.MapEnum<CartStatus>("cart_status_enum");
                builder.MapEnum<CheckoutStatus>("checkout_status_enum");
                builder.MapEnum<DeliveryDuration>("delivery_duration_enum");
                builder.MapEnum<PaymentMethod>("payment_method_enum");
                builder.MapEnum<OrderStatus>("order_status_enum");
                builder.MapEnum<UserRole>("user_role_enum");
                builder.MapEnum<PreferenceType>("preference_type");
                builder.MapEnum<TransportMode>("transport_mode");
            })
            .Options;

        return new AppDbContext(options);
    }

    internal static (int OrderId, int CustomerId, int CheckoutId) CreateOrderFixture(AppDbContext context)
    {
        var suffix = Guid.NewGuid().ToString("N")[..12];

        var user = new User(
            name: $"Phase3 User {suffix}",
            email: $"phase3-{suffix}@example.test",
            passwordHash: "test-password-hash",
            role: UserRole.CUSTOMER);
        context.Users.Add(user);
        context.SaveChanges();

        var customer = new Customer();
        customer.SetUserId(user.GetUserId());
        customer.SetAddress("Phase 3 Address");
        customer.SetCustomerType(1);
        context.Customers.Add(customer);
        context.SaveChanges();

        var cart = new Cart();
        cart.SetCustomerId(customer.GetCustomerId());
        cart.SetRentalStart(DateTime.UtcNow.Date);
        cart.SetRentalEnd(DateTime.UtcNow.Date.AddDays(3));
        context.Carts.Add(cart);
        context.SaveChanges();

        var checkout = new Checkout();
        checkout.Initialize(customer.GetCustomerId(), cart.GetCartId(), DateTime.UtcNow);
        context.Checkouts.Add(checkout);
        context.SaveChanges();

        var order = new Order();
        order.InitializeForCheckout(
            customer.GetCustomerId(),
            checkout.GetSelectionState().CheckoutId,
            DateTime.UtcNow,
            120.00m);
        context.Orders.Add(order);
        context.SaveChanges();

        return (order.GetOrderContext().OrderId, customer.GetCustomerId(), checkout.GetSelectionState().CheckoutId);
    }

    private static DeliveryRoute CreateRoute(AppDbContext context)
    {
        var route = new DeliveryRoute();
        route.SetOriginAddress("Phase 3 Warehouse");
        route.SetDestinationAddress("Phase 3 Destination");
        route.SetTotalDistanceKm(12.5);
        route.SetIsValid(true);

        context.DeliveryRoutes.Add(route);
        context.SaveChanges();

        return route;
    }

    private static ShippingOption CreateShippingOption(
        int orderId,
        int routeId,
        PreferenceType preferenceType,
        TransportMode transportMode,
        string displayName)
    {
        var option = new ShippingOption();
        option.ConfigureGeneratedOption(orderId, routeId, preferenceType, displayName, 19.99m, 5.5, 3, transportMode);
        return option;
    }

}

// Phase 4 exercises ShippingOptionManager with in-memory test doubles to validate orchestration rules.
internal static class Phase4Tests
{
    public static IReadOnlyList<PhaseTest> All { get; } =
    [
        new("TransportCarbonManager calculates leg surcharge by transport type", TransportCarbonManagerCalculatesLegSurchargeByTransportType),
        new("TransportCarbonManager sums leg surcharges into route total", TransportCarbonManagerSumsLegSurcharges),
        new("ShippingOptionManager builds and persists one option per preference", ShippingOptionManagerBuildsAndPersistsOptions),
        new("ShippingOptionManager reuses persisted options before rebuilding", ShippingOptionManagerReusesExistingOptions),
        new("ShippingOptionManager selection writes through checkout.option_id only", ShippingOptionManagerAppliesSelectionThroughRepository),
        new("ShippingOptionManager rejects mismatched order selections", ShippingOptionManagerRejectsMismatchedSelection)
    ];

    private static void TransportCarbonManagerCalculatesLegSurchargeByTransportType()
    {
        var manager = new ProRental.Domain.Module3.P2_1.Controls.TransportCarbonManager(new StubPricingRuleGateway());

        var truckSurcharge = manager.CalculateLegCarbonSurcharge(2, 5.0, 10.0, 3.0, TransportMode.TRUCK);
        var shipSurcharge = manager.CalculateLegCarbonSurcharge(2, 5.0, 10.0, 3.0, TransportMode.SHIP);
        var planeSurcharge = manager.CalculateLegCarbonSurcharge(2, 5.0, 10.0, 3.0, TransportMode.PLANE);
        var trainSurcharge = manager.CalculateLegCarbonSurcharge(2, 5.0, 10.0, 3.0, TransportMode.TRAIN);

        TestAssertions.AssertEqual(5.15d, truckSurcharge);
        TestAssertions.AssertEqual(3.09d, shipSurcharge);
        TestAssertions.AssertEqual(12.36d, planeSurcharge);
        TestAssertions.AssertEqual(4.12d, trainSurcharge);
    }

    private static void TransportCarbonManagerSumsLegSurcharges()
    {
        var manager = new ProRental.Domain.Module3.P2_1.Controls.TransportCarbonManager(new StubPricingRuleGateway());

        var truckSurcharge = manager.CalculateLegCarbonSurcharge(2, 5.0, 10.0, 3.0, TransportMode.TRUCK);
        var shipSurcharge = manager.CalculateLegCarbonSurcharge(2, 5.0, 10.0, 3.0, TransportMode.SHIP);
        var planeSurcharge = manager.CalculateLegCarbonSurcharge(2, 5.0, 10.0, 3.0, TransportMode.PLANE);

        var total = manager.CalculateTotalCarbonSurcharge([truckSurcharge, shipSurcharge, planeSurcharge]);

        TestAssertions.AssertEqual(20.6d, total);
    }

    private static void ShippingOptionManagerBuildsAndPersistsOptions()
    {
        var repository = new InMemoryShippingOptionMapper();
        var orderService = new StubOrderService(new OrderShippingContext(10, 20, 30, "Singapore", 5.5, 2));
        var routingService = new StubRoutingService();
        var transportCarbonService = new StubTransportCarbonService();
        var pricingRuleGateway = new StubPricingRuleGateway();
        var manager = new ShippingOptionManager(repository, orderService, routingService, transportCarbonService, pricingRuleGateway);

        var result = manager.BuildOptionSetAsync(orderService.Context).GetAwaiter().GetResult();

        TestAssertions.AssertEqual(3, result.Count);
        TestAssertions.AssertSequence(
            new[] { PreferenceType.FAST, PreferenceType.CHEAP, PreferenceType.GREEN },
            result.Select(option => option.PreferenceType));
        TestAssertions.AssertEqual(3, repository.StoredOptions.Count);
        TestAssertions.AssertEqual(3, routingService.Requests.Count);
        TestAssertions.AssertEqual(3, transportCarbonService.LegRequests.Count);
        TestAssertions.AssertEqual(3, transportCarbonService.RouteRequests.Count);
        TestAssertions.AssertEqual(3, transportCarbonService.LegSurchargeRequests.Count);
        TestAssertions.AssertEqual(3, transportCarbonService.TotalSurchargeRequests.Count);
        TestAssertions.AssertEqual(3, pricingRuleGateway.Requests.Count);
    }

    private static void ShippingOptionManagerReusesExistingOptions()
    {
        var repository = new InMemoryShippingOptionMapper();
        repository.Seed(CreatePersistedOption(1, 10, PreferenceType.FAST, "Persisted Fast", 22m, 8.0, 2, TransportMode.PLANE));

        var orderService = new StubOrderService(new OrderShippingContext(10, 20, 30, "Singapore", 5.5, 2));
        var routingService = new StubRoutingService();
        var transportCarbonService = new StubTransportCarbonService();
        var pricingRuleGateway = new StubPricingRuleGateway();
        var manager = new ShippingOptionManager(repository, orderService, routingService, transportCarbonService, pricingRuleGateway);

        var result = manager.GetShippingOptionsForOrderAsync(10).GetAwaiter().GetResult();

        TestAssertions.AssertEqual(1, result.Count);
        TestAssertions.AssertEqual("Persisted Fast", result[0].DisplayName);
        TestAssertions.AssertEqual(0, orderService.CallCount);
        TestAssertions.AssertEqual(0, routingService.Requests.Count);
        TestAssertions.AssertEqual(0, transportCarbonService.LegRequests.Count);
        TestAssertions.AssertEqual(0, transportCarbonService.RouteRequests.Count);
        TestAssertions.AssertEqual(0, transportCarbonService.LegSurchargeRequests.Count);
        TestAssertions.AssertEqual(0, transportCarbonService.TotalSurchargeRequests.Count);
        TestAssertions.AssertEqual(0, pricingRuleGateway.Requests.Count);
    }

    private static void ShippingOptionManagerAppliesSelectionThroughRepository()
    {
        var repository = new InMemoryShippingOptionMapper();
        repository.Order = CreateOrder(10, 30, 20);
        repository.Seed(CreatePersistedOption(5, 10, PreferenceType.GREEN, "Greenest", 18m, 4.2, 4, TransportMode.TRAIN));

        var manager = new ShippingOptionManager(
            repository,
            new StubOrderService(new OrderShippingContext(10, 20, 30, "Singapore", 5.5, 2)),
            new StubRoutingService(),
            new StubTransportCarbonService(),
            new StubPricingRuleGateway());

        var selection = manager.ApplyCustomerSelectionAsync(new SelectShippingOptionRequest(10, 5)).GetAwaiter().GetResult();

        TestAssertions.AssertEqual(30, repository.LastSelectedCheckoutId);
        TestAssertions.AssertEqual(5, repository.LastSelectedOptionId);
        TestAssertions.AssertEqual(PreferenceType.GREEN, selection.PreferenceType);
        TestAssertions.AssertEqual("TRAIN", selection.TransportModeLabel);
    }

    private static void ShippingOptionManagerRejectsMismatchedSelection()
    {
        var repository = new InMemoryShippingOptionMapper();
        repository.Order = CreateOrder(10, 30, 20);
        repository.Seed(CreatePersistedOption(7, 99, PreferenceType.CHEAP, "Wrong Order Option", 12m, 3.5, 5, TransportMode.TRUCK));

        var manager = new ShippingOptionManager(
            repository,
            new StubOrderService(new OrderShippingContext(10, 20, 30, "Singapore", 5.5, 2)),
            new StubRoutingService(),
            new StubTransportCarbonService(),
            new StubPricingRuleGateway());

        try
        {
            _ = manager.ApplyCustomerSelectionAsync(new SelectShippingOptionRequest(10, 7)).GetAwaiter().GetResult();
            throw new InvalidOperationException("Expected mismatched order selection to be rejected.");
        }
        catch (InvalidOperationException ex)
        {
            TestAssertions.AssertTrue(ex.Message.Contains("does not belong", StringComparison.Ordinal), ex.Message);
        }
    }

    private static ShippingOption CreatePersistedOption(
        int optionId,
        int orderId,
        PreferenceType preferenceType,
        string displayName,
        decimal cost,
        double carbonFootprintKg,
        int deliveryDays,
        TransportMode transportMode)
    {
        var option = new ShippingOption();
        option.ConfigureGeneratedOption(orderId, null, preferenceType, displayName, cost, carbonFootprintKg, deliveryDays, transportMode);
        SetPrivateField(option, "_optionId", optionId);
        return option;
    }

    private static Order CreateOrder(int orderId, int checkoutId, int customerId)
    {
        var order = new Order();
        order.InitializeForCheckout(customerId, checkoutId, DateTime.UtcNow, 100m);
        SetPrivateField(order, "_orderid", orderId);
        return order;
    }

    private static void SetPrivateField<TTarget, TValue>(TTarget target, string fieldName, TValue value)
    {
        var field = typeof(TTarget).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException($"Field '{fieldName}' was not found on {typeof(TTarget).Name}.");
        field.SetValue(target, value);
    }

    // In-memory repository double used to verify manager behavior without EF or PostgreSQL.
    private sealed class InMemoryShippingOptionMapper : IShippingOptionMapper
    {
        public List<ShippingOption> StoredOptions { get; } = [];
        public Order? Order { get; set; }
        public int? LastSelectedCheckoutId { get; private set; }
        public int? LastSelectedOptionId { get; private set; }
        private int _nextOptionId = 1;

        public Task<Order?> FindOrderWithCheckoutAsync(int orderId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Order is not null && Order.GetOrderContext().OrderId == orderId ? Order : null);
        }

        public Task<IReadOnlyList<ShippingOption>> FindByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<ShippingOption> options = StoredOptions.Where(option => option.BelongsToOrder(orderId)).ToArray();
            return Task.FromResult(options);
        }

        public Task<ShippingOption?> FindByIdAsync(int optionId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(StoredOptions.FirstOrDefault(option => option.GetSummary().OptionId == optionId));
        }

        public Task AddAsync(ShippingOption option, CancellationToken cancellationToken = default)
        {
            if (option.GetSummary().OptionId == 0)
            {
                SetPrivateField(option, "_optionId", _nextOptionId++);
            }

            StoredOptions.Add(option);
            return Task.CompletedTask;
        }

        public Task AddRangeAsync(IEnumerable<ShippingOption> options, CancellationToken cancellationToken = default)
        {
            foreach (var option in options)
            {
                AddAsync(option, cancellationToken).GetAwaiter().GetResult();
            }

            return Task.CompletedTask;
        }

        public Task UpdateAsync(ShippingOption option, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task SetCheckoutSelectedOptionAsync(int checkoutId, int optionId, CancellationToken cancellationToken = default)
        {
            LastSelectedCheckoutId = checkoutId;
            LastSelectedOptionId = optionId;
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Seed(ShippingOption option)
        {
            StoredOptions.Add(option);
            _nextOptionId = Math.Max(_nextOptionId, option.GetSummary().OptionId + 1);
        }
    }

    // Stub Module 1 adapter that returns a fixed order context for manager tests.
    private sealed class StubOrderService : IOrderService
    {
        public StubOrderService(OrderShippingContext context)
        {
            Context = context;
        }

        public OrderShippingContext Context { get; }
        public int CallCount { get; private set; }

        public Task<OrderShippingContext?> GetShippingContextAsync(int orderId, CancellationToken cancellationToken = default)
        {
            CallCount++;
            return Task.FromResult<OrderShippingContext?>(Context.OrderId == orderId ? Context : null);
        }
    }

    // Stub Feature 3 adapter that returns predictable routes for each preference.
    private sealed class StubRoutingService : IRoutingService
    {
        public List<RoutingRequest> Requests { get; } = [];

        public Task<DeliveryRoute> CreateRouteAsync(RoutingRequest request, CancellationToken cancellationToken = default)
        {
            Requests.Add(request);
            var route = new DeliveryRoute();
            route.SetOriginAddress("Warehouse");
            route.SetDestinationAddress(request.DestinationAddress);
            route.SetTotalDistanceKm(10 + Requests.Count);
            route.SetIsValid(true);
            return Task.FromResult(route);
        }
    }

    // Stub Feature 2 adapter that exposes the incoming shared carbon-calculation contract.
    private sealed class StubTransportCarbonService : ITransportCarbonService
    {
        public List<(int Quantity, double WeightKg, double DistanceKm, double StorageCo2)> LegRequests { get; } = [];
        public List<IReadOnlyList<double>> RouteRequests { get; } = [];
        public List<(int Quantity, double WeightKg, double DistanceKm, double StorageCo2, TransportMode TransportMode)> LegSurchargeRequests { get; } = [];
        public List<IReadOnlyList<double>> TotalSurchargeRequests { get; } = [];

        public double CalculateLegCarbon(int quantity, double weightKg, double distanceKm, double storageCo2)
        {
            LegRequests.Add((quantity, weightKg, distanceKm, storageCo2));
            return (quantity * weightKg * distanceKm) + storageCo2;
        }

        public double CalculateRouteCarbon(IReadOnlyList<double> legCarbonValues)
        {
            RouteRequests.Add(legCarbonValues.ToArray());
            return legCarbonValues.Sum();
        }

        public double CalculateLegCarbonSurcharge(int quantity, double weightKg, double distanceKm, double storageCo2, TransportMode transportMode)
        {
            LegSurchargeRequests.Add((quantity, weightKg, distanceKm, storageCo2, transportMode));

            var legCarbon = (quantity * weightKg * distanceKm) + storageCo2;
            var surchargeRate = transportMode switch
            {
                TransportMode.PLANE => 0.12d,
                TransportMode.SHIP => 0.03d,
                TransportMode.TRAIN => 0.04d,
                _ => 0.05d
            };

            return legCarbon * surchargeRate;
        }

        public double CalculateTotalCarbonSurcharge(IReadOnlyList<double> legSurcharges)
        {
            TotalSurchargeRequests.Add(legSurcharges.ToArray());
            return legSurcharges.Sum();
        }
    }

    // Pricing-rule stub used so Feature 1 can assemble checkout options without relying on EF.
    private sealed class StubPricingRuleGateway : IPricingRuleGateway
    {
        public List<TransportMode> Requests { get; } = [];

        public List<PricingRule> FindActiveRules()
        {
            return [CreateRule(TransportMode.TRAIN, 1.0m, 0.05m)];
        }

        public List<PricingRule> FindByTransportMode(TransportMode mode)
        {
            Requests.Add(mode);

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
    }
}

// Phase 5 keeps the MVC boundary thin by testing controller coordination separately from the domain layer.
internal static class Phase5Tests
{
    public static IReadOnlyList<PhaseTest> All { get; } =
    [
        new("GetShippingOptions returns the option list model and order id", GetShippingOptionsReturnsOptionList),
        new("CompareOptions returns ranked option lists in ViewData", CompareOptionsReturnsRankedLists),
        new("SelectShippingOption returns the confirmed selection result", SelectShippingOptionReturnsSelectionResult)
    ];

    private static void GetShippingOptionsReturnsOptionList()
    {
        var options = CreateControllerOptions();
        var service = new ControllerShippingOptionService(options, CreateSelectionResult());
        var controller = new ShippingOptionsController(service, new ControllerRankingService());

        var result = controller.GetShippingOptions(42, CancellationToken.None).GetAwaiter().GetResult();
        var viewResult = AssertViewResult(result);
        var model = AssertModel<IReadOnlyList<ShippingOptionSummary>>(viewResult);

        TestAssertions.AssertEqual(42, viewResult.ViewData["OrderId"]);
        TestAssertions.AssertEqual(3, model.Count);
        TestAssertions.AssertEqual(42, service.LastGetOrderId);
    }

    private static void CompareOptionsReturnsRankedLists()
    {
        var options = CreateControllerOptions();
        var service = new ControllerShippingOptionService(options, CreateSelectionResult());
        var rankingService = new ControllerRankingService();
        var controller = new ShippingOptionsController(service, rankingService);

        var result = controller.CompareOptions(42, CancellationToken.None).GetAwaiter().GetResult();
        var viewResult = AssertViewResult(result);
        var model = AssertModel<IReadOnlyList<ShippingOptionSummary>>(viewResult);

        TestAssertions.AssertEqual(42, viewResult.ViewData["OrderId"]);
        TestAssertions.AssertEqual(3, model.Count);
        TestAssertions.AssertTrue(viewResult.ViewData["SpeedRanked"] is IReadOnlyList<ShippingOptionSummary>, "Expected SpeedRanked view data.");
        TestAssertions.AssertTrue(viewResult.ViewData["CostRanked"] is IReadOnlyList<ShippingOptionSummary>, "Expected CostRanked view data.");
        TestAssertions.AssertTrue(viewResult.ViewData["CarbonRanked"] is IReadOnlyList<ShippingOptionSummary>, "Expected CarbonRanked view data.");
        TestAssertions.AssertEqual(1, rankingService.SpeedCalls);
        TestAssertions.AssertEqual(1, rankingService.CostCalls);
        TestAssertions.AssertEqual(1, rankingService.CarbonCalls);
    }

    private static void SelectShippingOptionReturnsSelectionResult()
    {
        var expectedSelection = CreateSelectionResult();
        var service = new ControllerShippingOptionService(CreateControllerOptions(), expectedSelection);
        var controller = new ShippingOptionsController(service, new ControllerRankingService());

        var result = controller.SelectShippingOption(42, 7, CancellationToken.None).GetAwaiter().GetResult();
        var viewResult = AssertViewResult(result);
        var model = AssertModel<ShippingSelectionResult>(viewResult);

        TestAssertions.AssertEqual(42, service.LastSelectionRequest?.OrderId);
        TestAssertions.AssertEqual(7, service.LastSelectionRequest?.OptionId);
        TestAssertions.AssertEqual(expectedSelection, model);
    }

    private static ViewResult AssertViewResult(IActionResult actionResult)
    {
        return actionResult as ViewResult
            ?? throw new InvalidOperationException($"Expected ViewResult but got {actionResult.GetType().Name}.");
    }

    private static T AssertModel<T>(ViewResult viewResult)
    {
        return viewResult.Model is T model
            ? model
            : throw new InvalidOperationException($"Expected model of type {typeof(T).Name}.");
    }

    private static IReadOnlyList<ShippingOptionSummary> CreateControllerOptions() =>
    [
        new ShippingOptionSummary(1, 42, PreferenceType.FAST, "Fastest", 25m, 11.2, 1, null, TransportMode.PLANE, "Plane"),
        new ShippingOptionSummary(2, 42, PreferenceType.CHEAP, "Cheapest", 12m, 9.8, 5, null, TransportMode.SHIP, "Ship"),
        new ShippingOptionSummary(3, 42, PreferenceType.GREEN, "Greenest", 18m, 4.4, 4, null, TransportMode.TRAIN, "Train")
    ];

    private static ShippingSelectionResult CreateSelectionResult() =>
        new(42, 7, PreferenceType.GREEN, 18m, 4.4, 4, "TRAIN");

    // Controller-facing service double that captures input parameters and returns fixed view models.
    private sealed class ControllerShippingOptionService : IShippingOptionService
    {
        private readonly IReadOnlyList<ShippingOptionSummary> _options;
        private readonly ShippingSelectionResult _selectionResult;

        public ControllerShippingOptionService(
            IReadOnlyList<ShippingOptionSummary> options,
            ShippingSelectionResult selectionResult)
        {
            _options = options;
            _selectionResult = selectionResult;
        }

        public int? LastGetOrderId { get; private set; }
        public SelectShippingOptionRequest? LastSelectionRequest { get; private set; }

        public ProRental.Domain.Module3.P2_1.Models.ShippingOptionCarbonInput GetRouteCarbonInput(int shippingOptionId)
        {
            return new ProRental.Domain.Module3.P2_1.Models.ShippingOptionCarbonInput
            {
                Quantity = 1,
                ProductId = "TEST",
                HubId = "HUB-TEST",
                RouteLegs = []
            };
        }

        public Task<IReadOnlyList<ShippingOptionSummary>> GetShippingOptionsForOrderAsync(int orderId, CancellationToken cancellationToken = default)
        {
            LastGetOrderId = orderId;
            return Task.FromResult(_options);
        }

        public Task<IReadOnlyList<ShippingOptionSummary>> BuildOptionSetAsync(OrderShippingContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_options);
        }

        public Task<ShippingSelectionResult> ApplyCustomerSelectionAsync(SelectShippingOptionRequest request, CancellationToken cancellationToken = default)
        {
            LastSelectionRequest = request;
            return Task.FromResult(_selectionResult);
        }
    }

    // Ranking double used to verify that CompareOptions delegates each ranking request exactly once.
    private sealed class ControllerRankingService : IRankingService
    {
        public int SpeedCalls { get; private set; }
        public int CostCalls { get; private set; }
        public int CarbonCalls { get; private set; }

        public IReadOnlyList<ShippingOptionSummary> RankBySpeed(IEnumerable<ShippingOptionSummary> options)
        {
            SpeedCalls++;
            return options.OrderBy(option => option.DeliveryDays).ToArray();
        }

        public IReadOnlyList<ShippingOptionSummary> RankByCost(IEnumerable<ShippingOptionSummary> options)
        {
            CostCalls++;
            return options.OrderBy(option => option.Cost).ToArray();
        }

        public IReadOnlyList<ShippingOptionSummary> RankByCarbon(IEnumerable<ShippingOptionSummary> options)
        {
            CarbonCalls++;
            return options.OrderBy(option => option.CarbonFootprintKg).ToArray();
        }
    }
}

// Phase 6 checks container wiring and the temporary home-page entry point into Feature 1.
internal static class Phase6Tests
{
    public static IReadOnlyList<PhaseTest> All { get; } =
    [
        new("Feature1 DI registration resolves the shipping options stack", Feature1RegistrationResolvesExpectedServices),
        new("Home page exposes a temporary shipping options entry form", HomePageExposesShippingOptionsEntryForm)
    ];

    private static void Feature1RegistrationResolvesExpectedServices()
    {
        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql("Host=localhost;Database=testrental;Username=test;Password=test"));
        services.AddFeature1Services();

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        TestAssertions.AssertTrue(scopedProvider.GetService<IShippingOptionMapper>() is ShippingOptionMapper, "Expected shipping option mapper registration.");
        TestAssertions.AssertTrue(scopedProvider.GetService<IOrderService>() is ShippingOrderContextService, "Expected order context service registration.");
        TestAssertions.AssertTrue(scopedProvider.GetService<IRoutingService>() is ShippingRoutingService, "Expected routing service registration.");
        TestAssertions.AssertTrue(scopedProvider.GetService<ITransportCarbonService>() is ProRental.Domain.Module3.P2_1.Controls.TransportCarbonManager, "Expected transport carbon service registration.");
        TestAssertions.AssertTrue(scopedProvider.GetService<IShippingOptionService>() is ShippingOptionManager, "Expected shipping option manager registration.");
        TestAssertions.AssertTrue(scopedProvider.GetService<IRankingService>() is RankingManager, "Expected ranking manager registration.");

        var strategies = scopedProvider.GetServices<IRankingStrategy>().Select(strategy => strategy.PreferenceType).OrderBy(value => value).ToArray();
        TestAssertions.AssertSequence(new[] { PreferenceType.FAST, PreferenceType.CHEAP, PreferenceType.GREEN }.OrderBy(value => value), strategies);
    }

    private static void HomePageExposesShippingOptionsEntryForm()
    {
        var homeIndexPath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Home", "Index.cshtml");
        var content = File.ReadAllText(homeIndexPath);

        TestAssertions.AssertTrue(content.Contains("asp-controller=\"ShippingOptions\"", StringComparison.Ordinal), "Expected home page to target ShippingOptionsController.");
        TestAssertions.AssertTrue(content.Contains("asp-action=\"GetShippingOptions\"", StringComparison.Ordinal), "Expected home page to post into GetShippingOptions.");
        TestAssertions.AssertTrue(content.Contains("name=\"orderId\"", StringComparison.Ordinal), "Expected home page to collect an orderId.");
    }
}

// Phase 7 runs the full Feature 1 flow against a real database fixture.
internal static class Phase7Tests
{
    public static IReadOnlyList<PhaseTest> All { get; } =
    [
        new("Feature1 builds and persists three shipping options end-to-end", Feature1BuildsAndPersistsOptionsEndToEnd),
        new("Feature1 selection writes checkout.option_id end-to-end", Feature1AppliesSelectionEndToEnd)
    ];

    private static void Feature1BuildsAndPersistsOptionsEndToEnd()
    {
        using var context = Phase3Tests.CreateDbContext();
        using var transaction = context.Database.BeginTransaction();

        var snapshot = Phase3Tests.CreateOrderFixture(context);
        var manager = CreateManager(context);

        var options = manager.GetShippingOptionsForOrderAsync(snapshot.OrderId).GetAwaiter().GetResult();
        var persistedOptions = new ShippingOptionMapper(context).FindByOrderIdAsync(snapshot.OrderId).GetAwaiter().GetResult();

        TestAssertions.AssertEqual(3, options.Count);
        TestAssertions.AssertSequence(new[] { PreferenceType.FAST, PreferenceType.CHEAP, PreferenceType.GREEN }, options.Select(option => option.PreferenceType));
        TestAssertions.AssertTrue(options.All(option => option.RouteId is > 0), "Expected all generated options to reference a persisted route.");
        TestAssertions.AssertEqual(3, persistedOptions.Count);

        transaction.Rollback();
    }

    private static void Feature1AppliesSelectionEndToEnd()
    {
        using var context = Phase3Tests.CreateDbContext();
        using var transaction = context.Database.BeginTransaction();

        var snapshot = Phase3Tests.CreateOrderFixture(context);
        var manager = CreateManager(context);
        var options = manager.GetShippingOptionsForOrderAsync(snapshot.OrderId).GetAwaiter().GetResult();
        var selectedOption = options.Single(option => option.PreferenceType == PreferenceType.GREEN);

        var result = manager.ApplyCustomerSelectionAsync(
            new SelectShippingOptionRequest(snapshot.OrderId, selectedOption.OptionId)).GetAwaiter().GetResult();

        context.ChangeTracker.Clear();

        var checkout = context.Checkouts
            .First(entity => EF.Property<int>(entity, "Checkoutid") == snapshot.CheckoutId);

        TestAssertions.AssertEqual(selectedOption.OptionId, checkout.GetSelectionState().SelectedOptionId);
        TestAssertions.AssertEqual(PreferenceType.GREEN, result.PreferenceType);
        TestAssertions.AssertEqual(selectedOption.OptionId, result.OptionId);

        transaction.Rollback();
    }

    private static ShippingOptionManager CreateManager(AppDbContext context)
    {
        return new ShippingOptionManager(
            new ShippingOptionMapper(context),
            new ShippingOrderContextService(context),
            new ShippingRoutingService(context),
            new ProRental.Domain.Module3.P2_1.Controls.TransportCarbonManager(new PricingRuleGateway(context)),
            new PricingRuleGateway(context));
    }
}

internal static class TestAssertions
{
    public static void AssertEqual<T>(T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new InvalidOperationException($"Expected '{expected}' but got '{actual}'.");
        }
    }

    public static void AssertNull(object? value)
    {
        if (value is not null)
        {
            throw new InvalidOperationException($"Expected null but got '{value}'.");
        }
    }

    public static void AssertSequence<T>(IEnumerable<T> expected, IEnumerable<T> actual)
    {
        var expectedItems = expected.ToArray();
        var actualItems = actual.ToArray();

        if (!expectedItems.SequenceEqual(actualItems))
        {
            throw new InvalidOperationException(
                $"Expected sequence [{string.Join(", ", expectedItems)}] but got [{string.Join(", ", actualItems)}].");
        }
    }

    public static void AssertTrue(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
