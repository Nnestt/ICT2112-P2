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
using ProRental.Domain.Controls;
using ProRental.Domain.Entities;
using ProRental.Domain.Enums;
using ProRental.Interfaces.Data;
using ProRental.Interfaces.Domain;
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

internal static class Phase0Tests
{
    public static IReadOnlyList<PhaseTest> All { get; } =
    [
        new("ShippingOption accessors round-trip values", ShippingOptionAccessorsRoundTrip),
        new("Checkout selection accessors round-trip values", CheckoutSelectionAccessorsRoundTrip),
        new("Order accessors round-trip values", OrderAccessorsRoundTrip),
        new("DeliveryRoute accessors round-trip values", DeliveryRouteAccessorsRoundTrip),
        new("Feature1 enum mappings use snake_case column names", Feature1EnumMappingsUseSnakeCase)
    ];

    private static void ShippingOptionAccessorsRoundTrip()
    {
        var option = new ShippingOption();

        option.SetDisplayName("Green Express");
        option.SetCost(42.50m);
        option.SetCarbonFootprintKg(12.75);
        option.SetDeliveryDays(4);
        option.SetOrderId(11);
        option.SetRouteId(22);
        option.UpdatePreferenceType(PreferenceType.GREEN);
        option.UpdateTransportMode(TransportMode.TRAIN);

        TestAssertions.AssertEqual("Green Express", option.GetDisplayName());
        TestAssertions.AssertEqual(42.50m, option.GetCost());
        TestAssertions.AssertEqual(12.75, option.GetCarbonFootprintKg());
        TestAssertions.AssertEqual(4, option.GetDeliveryDays());
        TestAssertions.AssertEqual(11, option.GetOrderId());
        TestAssertions.AssertEqual(22, option.GetRouteId());
        TestAssertions.AssertEqual(PreferenceType.GREEN, option.GetPreferenceType());
        TestAssertions.AssertEqual(TransportMode.TRAIN, option.GetTransportMode());
    }

    private static void CheckoutSelectionAccessorsRoundTrip()
    {
        var checkout = new Checkout();
        var createdAt = new DateTime(2026, 03, 22, 10, 30, 00, DateTimeKind.Utc);

        checkout.SetCustomerId(2);
        checkout.SetCartId(5);
        checkout.SetCreatedAt(createdAt);
        checkout.SelectShippingOption(9);

        TestAssertions.AssertEqual(2, checkout.GetCustomerId());
        TestAssertions.AssertEqual(5, checkout.GetCartId());
        TestAssertions.AssertEqual(createdAt, checkout.GetCreatedAt());
        TestAssertions.AssertEqual(9, checkout.GetSelectedOptionId());

        checkout.ClearSelectedShippingOption();

        TestAssertions.AssertNull(checkout.GetSelectedOptionId());
    }

    private static void OrderAccessorsRoundTrip()
    {
        var order = new Order();
        var orderDate = new DateTime(2026, 03, 22, 12, 00, 00, DateTimeKind.Utc);

        order.SetCustomerId(3);
        order.SetCheckoutId(7);
        order.SetTransactionId(8);
        order.SetOrderDate(orderDate);
        order.SetTotalAmount(199.99m);

        TestAssertions.AssertEqual(3, order.GetCustomerId());
        TestAssertions.AssertEqual(7, order.GetCheckoutId());
        TestAssertions.AssertEqual(8, order.GetTransactionId());
        TestAssertions.AssertEqual(orderDate, order.GetOrderDate());
        TestAssertions.AssertEqual(199.99m, order.GetTotalAmount());
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
        var customerChoice = context.Model.FindEntityType(typeof(CustomerChoice))
            ?? throw new InvalidOperationException("CustomerChoice entity metadata was not found.");

        var shippingTable = StoreObjectIdentifier.Table("shipping_option", null);
        var customerChoiceTable = StoreObjectIdentifier.Table("customer_choice", null);

        TestAssertions.AssertEqual(
            "preference_type",
            shippingOption.FindProperty("PreferenceType")?.GetColumnName(shippingTable));
        TestAssertions.AssertEqual(
            "transport_mode",
            shippingOption.FindProperty("TransportMode")?.GetColumnName(shippingTable));
        TestAssertions.AssertEqual(
            "preference_type",
            customerChoice.FindProperty("PreferenceType")?.GetColumnName(customerChoiceTable));
    }

}

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
        var repository = typeof(IShippingOptionRepository);
        var orderService = typeof(IOrderService);
        var routingService = typeof(IRoutingService);
        var transportCarbonService = typeof(ITransportCarbonService);

        AssertMethod(repository, "FindOrderWithCheckoutAsync", typeof(Task<Order?>), typeof(int), typeof(CancellationToken));
        AssertMethod(repository, "FindByOrderIdAsync", typeof(Task<IReadOnlyList<ShippingOption>>), typeof(int), typeof(CancellationToken));
        AssertMethod(repository, "FindByIdAsync", typeof(Task<ShippingOption?>), typeof(int), typeof(CancellationToken));
        AssertMethod(repository, "AddAsync", typeof(Task), typeof(ShippingOption), typeof(CancellationToken));
        AssertMethod(repository, "AddRangeAsync", typeof(Task), typeof(IEnumerable<ShippingOption>), typeof(CancellationToken));
        AssertMethod(repository, "UpdateAsync", typeof(Task), typeof(ShippingOption), typeof(CancellationToken));
        AssertMethod(repository, "SetCheckoutSelectedOptionAsync", typeof(Task), typeof(int), typeof(int), typeof(CancellationToken));
        AssertMethod(repository, "SaveChangesAsync", typeof(Task), typeof(CancellationToken));

        AssertMethod(orderService, "GetShippingContextAsync", typeof(Task<OrderShippingContext?>), typeof(int), typeof(CancellationToken));
        AssertMethod(routingService, "CreateRouteAsync", typeof(Task<DeliveryRoute>), typeof(RoutingRequest), typeof(CancellationToken));
        AssertMethod(transportCarbonService, "QuoteAsync", typeof(Task<TransportQuote>), typeof(DeliveryRoute), typeof(OrderShippingContext), typeof(PreferenceType), typeof(CancellationToken));
    }

    private static void AssertMethod(Type type, string name, Type returnType, params Type[] parameterTypes)
    {
        var method = type.GetMethod(name, parameterTypes)
            ?? throw new InvalidOperationException($"{type.Name}.{name} was not found.");

        TestAssertions.AssertEqual(returnType, method.ReturnType);
    }
}

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
        var repository = new ShippingOptionRepository(context);

        var order = repository.FindOrderWithCheckoutAsync(snapshot.OrderId).GetAwaiter().GetResult()
            ?? throw new InvalidOperationException("Expected repository to return an order.");

        TestAssertions.AssertEqual(snapshot.OrderId, order.GetOrderId());
        TestAssertions.AssertEqual(snapshot.CheckoutId, order.GetCheckoutId());
        TestAssertions.AssertTrue(order.Checkout is not null, "Expected order checkout navigation to be loaded.");

        transaction.Rollback();
    }

    private static void RepositoryCanInsertAndReloadShippingOption()
    {
        using var context = CreateDbContext();
        using var transaction = context.Database.BeginTransaction();

        var snapshot = CreateOrderFixture(context);
        var repository = new ShippingOptionRepository(context);
        var route = CreateRoute(context);

        var option = new ShippingOption();
        option.SetDisplayName("Phase 3 Test Option");
        option.SetCost(31.25m);
        option.SetCarbonFootprintKg(7.5);
        option.SetDeliveryDays(4);
        option.SetOrderId(snapshot.OrderId);
        option.SetRouteId(route.GetRouteId());
        option.UpdatePreferenceType(PreferenceType.GREEN);
        option.UpdateTransportMode(TransportMode.TRAIN);

        repository.AddAsync(option).GetAwaiter().GetResult();
        repository.SaveChangesAsync().GetAwaiter().GetResult();

        TestAssertions.AssertTrue(option.GetOptionId() > 0, "Expected inserted shipping option to receive an identity value.");

        context.ChangeTracker.Clear();

        var reloaded = repository.FindByIdAsync(option.GetOptionId()).GetAwaiter().GetResult()
            ?? throw new InvalidOperationException("Expected inserted shipping option to be queryable by id.");
        var orderOptions = repository.FindByOrderIdAsync(snapshot.OrderId).GetAwaiter().GetResult();

        TestAssertions.AssertEqual("Phase 3 Test Option", reloaded.GetDisplayName());
        TestAssertions.AssertEqual(PreferenceType.GREEN, reloaded.GetPreferenceType());
        TestAssertions.AssertTrue(orderOptions.Any(item => item.GetOptionId() == option.GetOptionId()), "Expected inserted option to be returned by order lookup.");

        transaction.Rollback();
    }

    private static void RepositoryCanPersistCheckoutSelection()
    {
        using var context = CreateDbContext();
        using var transaction = context.Database.BeginTransaction();

        var snapshot = CreateOrderFixture(context);
        var repository = new ShippingOptionRepository(context);
        var route = CreateRoute(context);
        var option = CreateShippingOption(snapshot.OrderId, route.GetRouteId(), PreferenceType.CHEAP, TransportMode.TRUCK, "Selected Option");

        repository.AddAsync(option).GetAwaiter().GetResult();
        repository.SaveChangesAsync().GetAwaiter().GetResult();
        repository.SetCheckoutSelectedOptionAsync(snapshot.CheckoutId, option.GetOptionId()).GetAwaiter().GetResult();
        repository.SaveChangesAsync().GetAwaiter().GetResult();

        context.ChangeTracker.Clear();

        var checkout = context.Checkouts
            .First(entity => EF.Property<int>(entity, "Checkoutid") == snapshot.CheckoutId);

        TestAssertions.AssertEqual(option.GetOptionId(), checkout.GetSelectedOptionId());

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
        checkout.SetCustomerId(customer.GetCustomerId());
        checkout.SetCartId(cart.GetCartId());
        checkout.SetCreatedAt(DateTime.UtcNow);
        context.Checkouts.Add(checkout);
        context.SaveChanges();

        var order = new Order();
        order.SetCustomerId(customer.GetCustomerId());
        order.SetCheckoutId(checkout.GetCheckoutId());
        order.SetOrderDate(DateTime.UtcNow);
        order.SetTotalAmount(120.00m);
        context.Orders.Add(order);
        context.SaveChanges();

        return (order.GetOrderId(), customer.GetCustomerId(), checkout.GetCheckoutId());
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
        option.SetDisplayName(displayName);
        option.SetCost(19.99m);
        option.SetCarbonFootprintKg(5.5);
        option.SetDeliveryDays(3);
        option.SetOrderId(orderId);
        option.SetRouteId(routeId);
        option.UpdatePreferenceType(preferenceType);
        option.UpdateTransportMode(transportMode);
        return option;
    }

}

internal static class Phase4Tests
{
    public static IReadOnlyList<PhaseTest> All { get; } =
    [
        new("ShippingOptionManager builds and persists one option per preference", ShippingOptionManagerBuildsAndPersistsOptions),
        new("ShippingOptionManager reuses persisted options before rebuilding", ShippingOptionManagerReusesExistingOptions),
        new("ShippingOptionManager selection writes through checkout.option_id only", ShippingOptionManagerAppliesSelectionThroughRepository),
        new("ShippingOptionManager rejects mismatched order selections", ShippingOptionManagerRejectsMismatchedSelection)
    ];

    private static void ShippingOptionManagerBuildsAndPersistsOptions()
    {
        var repository = new InMemoryShippingOptionRepository();
        var orderService = new StubOrderService(new OrderShippingContext(10, 20, 30, "Singapore", 5.5, 2));
        var routingService = new StubRoutingService();
        var quoteService = new StubTransportCarbonService();
        var manager = new ShippingOptionManager(repository, orderService, routingService, quoteService);

        var result = manager.BuildOptionSetAsync(orderService.Context).GetAwaiter().GetResult();

        TestAssertions.AssertEqual(3, result.Count);
        TestAssertions.AssertSequence(
            new[] { PreferenceType.FAST, PreferenceType.CHEAP, PreferenceType.GREEN },
            result.Select(option => option.PreferenceType));
        TestAssertions.AssertEqual(3, repository.StoredOptions.Count);
        TestAssertions.AssertEqual(3, routingService.Requests.Count);
        TestAssertions.AssertEqual(3, quoteService.Requests.Count);
    }

    private static void ShippingOptionManagerReusesExistingOptions()
    {
        var repository = new InMemoryShippingOptionRepository();
        repository.Seed(CreatePersistedOption(1, 10, PreferenceType.FAST, "Persisted Fast", 22m, 8.0, 2, TransportMode.PLANE));

        var orderService = new StubOrderService(new OrderShippingContext(10, 20, 30, "Singapore", 5.5, 2));
        var routingService = new StubRoutingService();
        var quoteService = new StubTransportCarbonService();
        var manager = new ShippingOptionManager(repository, orderService, routingService, quoteService);

        var result = manager.GetShippingOptionsForOrderAsync(10).GetAwaiter().GetResult();

        TestAssertions.AssertEqual(1, result.Count);
        TestAssertions.AssertEqual("Persisted Fast", result[0].DisplayName);
        TestAssertions.AssertEqual(0, orderService.CallCount);
        TestAssertions.AssertEqual(0, routingService.Requests.Count);
        TestAssertions.AssertEqual(0, quoteService.Requests.Count);
    }

    private static void ShippingOptionManagerAppliesSelectionThroughRepository()
    {
        var repository = new InMemoryShippingOptionRepository();
        repository.Order = CreateOrder(10, 30, 20);
        repository.Seed(CreatePersistedOption(5, 10, PreferenceType.GREEN, "Greenest", 18m, 4.2, 4, TransportMode.TRAIN));

        var manager = new ShippingOptionManager(
            repository,
            new StubOrderService(new OrderShippingContext(10, 20, 30, "Singapore", 5.5, 2)),
            new StubRoutingService(),
            new StubTransportCarbonService());

        var selection = manager.ApplyCustomerSelectionAsync(new SelectShippingOptionRequest(10, 5)).GetAwaiter().GetResult();

        TestAssertions.AssertEqual(30, repository.LastSelectedCheckoutId);
        TestAssertions.AssertEqual(5, repository.LastSelectedOptionId);
        TestAssertions.AssertEqual(PreferenceType.GREEN, selection.PreferenceType);
        TestAssertions.AssertEqual("TRAIN", selection.TransportModeLabel);
    }

    private static void ShippingOptionManagerRejectsMismatchedSelection()
    {
        var repository = new InMemoryShippingOptionRepository();
        repository.Order = CreateOrder(10, 30, 20);
        repository.Seed(CreatePersistedOption(7, 99, PreferenceType.CHEAP, "Wrong Order Option", 12m, 3.5, 5, TransportMode.TRUCK));

        var manager = new ShippingOptionManager(
            repository,
            new StubOrderService(new OrderShippingContext(10, 20, 30, "Singapore", 5.5, 2)),
            new StubRoutingService(),
            new StubTransportCarbonService());

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
        option.SetOrderId(orderId);
        option.SetDisplayName(displayName);
        option.SetCost(cost);
        option.SetCarbonFootprintKg(carbonFootprintKg);
        option.SetDeliveryDays(deliveryDays);
        option.UpdatePreferenceType(preferenceType);
        option.UpdateTransportMode(transportMode);
        SetPrivateField(option, "_optionId", optionId);
        return option;
    }

    private static Order CreateOrder(int orderId, int checkoutId, int customerId)
    {
        var order = new Order();
        order.SetCheckoutId(checkoutId);
        order.SetCustomerId(customerId);
        order.SetOrderDate(DateTime.UtcNow);
        order.SetTotalAmount(100m);
        SetPrivateField(order, "_orderid", orderId);
        return order;
    }

    private static void SetPrivateField<TTarget, TValue>(TTarget target, string fieldName, TValue value)
    {
        var field = typeof(TTarget).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException($"Field '{fieldName}' was not found on {typeof(TTarget).Name}.");
        field.SetValue(target, value);
    }

    private sealed class InMemoryShippingOptionRepository : IShippingOptionRepository
    {
        public List<ShippingOption> StoredOptions { get; } = [];
        public Order? Order { get; set; }
        public int? LastSelectedCheckoutId { get; private set; }
        public int? LastSelectedOptionId { get; private set; }
        private int _nextOptionId = 1;

        public Task<Order?> FindOrderWithCheckoutAsync(int orderId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Order is not null && Order.GetOrderId() == orderId ? Order : null);
        }

        public Task<IReadOnlyList<ShippingOption>> FindByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<ShippingOption> options = StoredOptions.Where(option => option.GetOrderId() == orderId).ToArray();
            return Task.FromResult(options);
        }

        public Task<ShippingOption?> FindByIdAsync(int optionId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(StoredOptions.FirstOrDefault(option => option.GetOptionId() == optionId));
        }

        public Task AddAsync(ShippingOption option, CancellationToken cancellationToken = default)
        {
            if (option.GetOptionId() == 0)
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
            _nextOptionId = Math.Max(_nextOptionId, option.GetOptionId() + 1);
        }
    }

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

    private sealed class StubTransportCarbonService : ITransportCarbonService
    {
        public List<(PreferenceType PreferenceType, int OrderId)> Requests { get; } = [];

        public Task<TransportQuote> QuoteAsync(
            DeliveryRoute route,
            OrderShippingContext context,
            PreferenceType preferenceType,
            CancellationToken cancellationToken = default)
        {
            Requests.Add((preferenceType, context.OrderId));

            var quote = preferenceType switch
            {
                PreferenceType.FAST => new TransportQuote(30m, 12.5, 1, TransportMode.PLANE, "Fastest", "Plane"),
                PreferenceType.CHEAP => new TransportQuote(15m, 8.0, 5, TransportMode.SHIP, "Cheapest", "Ship"),
                _ => new TransportQuote(18m, 4.5, 4, TransportMode.TRAIN, "Greenest", "Train")
            };

            return Task.FromResult(quote);
        }
    }
}

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

        TestAssertions.AssertTrue(scopedProvider.GetService<IShippingOptionRepository>() is ShippingOptionRepository, "Expected shipping option repository registration.");
        TestAssertions.AssertTrue(scopedProvider.GetService<IOrderService>() is ShippingOrderContextService, "Expected order context service registration.");
        TestAssertions.AssertTrue(scopedProvider.GetService<IRoutingService>() is ShippingRoutingService, "Expected routing service registration.");
        TestAssertions.AssertTrue(scopedProvider.GetService<ITransportCarbonService>() is ShippingTransportCarbonService, "Expected transport carbon service registration.");
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
        var persistedOptions = new ShippingOptionRepository(context).FindByOrderIdAsync(snapshot.OrderId).GetAwaiter().GetResult();

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

        TestAssertions.AssertEqual(selectedOption.OptionId, checkout.GetSelectedOptionId());
        TestAssertions.AssertEqual(PreferenceType.GREEN, result.PreferenceType);
        TestAssertions.AssertEqual(selectedOption.OptionId, result.OptionId);

        transaction.Rollback();
    }

    private static ShippingOptionManager CreateManager(AppDbContext context)
    {
        return new ShippingOptionManager(
            new ShippingOptionRepository(context),
            new ShippingOrderContextService(context),
            new ShippingRoutingService(context),
            new ShippingTransportCarbonService());
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
