using Microsoft.EntityFrameworkCore;
using ProRental.Data.Interfaces;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;
using ProRental.Domain.Enums;
using ProRental.Interfaces.Module3.P2_1;

namespace ProRental.Domain.Controls;

/// <summary>
/// Main routing orchestrator for Module 3 / P2-1. It creates a structured
/// multi-modal route for Feature 1 and exposes first-mile route lookup for Feature 6.
/// </summary>
public sealed class RouteManager : IRoutingService, IRouteQueryService
{
    private const string DefaultWarehouseAddress = "ProRental Warehouse";

    private readonly AppDbContext _context;
    private readonly ITransportationHubMapper _transportationHubMapper;
    private readonly IRouteLegBuilder _routeLegBuilder;

    public RouteManager(
        AppDbContext context,
        ITransportationHubMapper transportationHubMapper,
        IRouteLegBuilder routeLegBuilder)
    {
        _context = context;
        _transportationHubMapper = transportationHubMapper;
        _routeLegBuilder = routeLegBuilder;
    }

    public DeliveryRoute CreateMultiModalRoute(string origin, string destination, List<TransportMode> modes)
    {
        if (string.IsNullOrWhiteSpace(origin))
        {
            throw new ArgumentException("Origin is required.", nameof(origin));
        }

        if (string.IsNullOrWhiteSpace(destination))
        {
            throw new ArgumentException("Destination is required.", nameof(destination));
        }

        if (modes is null || modes.Count == 0)
        {
            throw new ArgumentException("At least one transport mode is required.", nameof(modes));
        }

        var routeContext = ResolveRouteContext(origin, destination, modes);

        var route = new DeliveryRoute();
        route.SetOriginAddress(routeContext.WarehouseAddress);
        route.SetDestinationAddress(destination);
        route.SetIsValid(true);

        var routeLegs = BuildRouteLegs(routeContext, destination, modes);
        foreach (var routeLeg in routeLegs)
        {
            route.RouteLegs.Add(routeLeg);
        }

        route.SetTotalDistanceKm(Math.Round(routeLegs.Sum(routeLeg => routeLeg.GetDistanceKm() ?? 0d), 2, MidpointRounding.AwayFromZero));

        _context.DeliveryRoutes.Add(route);
        _context.SaveChanges();

        return route;
    }

    public RouteLeg RetrieveFirstMileLeg(int routeId)
    {
        if (routeId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(routeId));
        }

        return _context.RouteLegs
            .AsNoTracking()
            .FirstOrDefault(routeLeg =>
                EF.Property<int>(routeLeg, "RouteId") == routeId &&
                (routeLeg.GetIsFirstMile() ?? false))
            ?? throw new InvalidOperationException($"First-mile route leg for route '{routeId}' was not found.");
    }

    private List<RouteLeg> BuildRouteLegs(RouteContext routeContext, string destination, IReadOnlyList<TransportMode> modes)
    {
        var routeLegs = new List<RouteLeg>();

        if (modes.Count == 1)
        {
            routeLegs.Add(_routeLegBuilder.BuildMainTransportLeg(1, routeContext.WarehouseAddress, destination, modes[0]));
            return routeLegs;
        }

        routeLegs.Add(_routeLegBuilder.BuildMainTransportLeg(
            1,
            routeContext.WarehouseAddress,
            routeContext.OriginHubAddress,
            TransportMode.TRUCK));

        var mainTransportMode = modes[0];
        routeLegs.Add(_routeLegBuilder.BuildMainTransportLeg(
            2,
            routeContext.OriginHubAddress,
            routeContext.DestinationHubAddress,
            mainTransportMode));

        routeLegs.Add(_routeLegBuilder.BuildMainTransportLeg(
            3,
            routeContext.DestinationHubAddress,
            destination,
            TransportMode.TRUCK));

        MarkRouteRoles(routeLegs, mainTransportMode);
        return routeLegs;
    }

    private RouteContext ResolveRouteContext(string origin, string destination, IReadOnlyList<TransportMode> modes)
    {
        var mainTransportMode = modes.FirstOrDefault();
        var warehouseAddress = ResolveWarehouseAddress(origin);
        var symbolicOriginHub = GetSymbolicHubLabel(mainTransportMode);
        var originHubAddress = ResolveHubAddress(mainTransportMode, symbolicOriginHub);
        var destinationHubAddress = ResolveDestinationHubAddress(mainTransportMode, originHubAddress, destination);

        return new RouteContext(warehouseAddress, originHubAddress, destinationHubAddress);
    }

    private string ResolveWarehouseAddress(string fallbackOrigin)
    {
        return _transportationHubMapper.FindByType(HubType.WAREHOUSE)
            .Select(hub => hub.GetAddress())
            .FirstOrDefault(address => !string.IsNullOrWhiteSpace(address))
            ?? (!string.IsNullOrWhiteSpace(fallbackOrigin) ? fallbackOrigin : DefaultWarehouseAddress);
    }

    private string ResolveHubAddress(TransportMode transportMode, string fallbackLabel)
    {
        var hubType = MapTransportModeToHubType(transportMode);
        if (hubType is null)
        {
            return fallbackLabel;
        }

        return _transportationHubMapper.FindByType(hubType.Value)
            .Select(hub => hub.GetAddress())
            .FirstOrDefault(address => !string.IsNullOrWhiteSpace(address))
            ?? fallbackLabel;
    }

    private string ResolveDestinationHubAddress(
        TransportMode transportMode,
        string originHubAddress,
        string fallbackDestination)
    {
        var hubType = MapTransportModeToHubType(transportMode);
        if (hubType is null)
        {
            return fallbackDestination;
        }

        var hubAddresses = _transportationHubMapper.FindByType(hubType.Value)
            .Select(hub => hub.GetAddress())
            .Where(address => !string.IsNullOrWhiteSpace(address))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var alternateAddress = hubAddresses
            .FirstOrDefault(address => !string.Equals(address, originHubAddress, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(alternateAddress))
        {
            return alternateAddress;
        }

        if (!string.IsNullOrWhiteSpace(originHubAddress))
        {
            return originHubAddress;
        }

        return GetSymbolicHubLabel(transportMode);
    }

    private static HubType? MapTransportModeToHubType(TransportMode transportMode)
    {
        return transportMode switch
        {
            TransportMode.PLANE => HubType.AIRPORT,
            TransportMode.SHIP => HubType.SHIPPING_PORT,
            _ => null
        };
    }

    private static string GetSymbolicHubLabel(TransportMode transportMode)
    {
        return transportMode switch
        {
            TransportMode.PLANE => "PLANE Hub",
            TransportMode.SHIP => "SHIP Hub",
            TransportMode.TRAIN => "TRAIN Hub",
            _ => "TRUCK Hub"
        };
    }

    private static void MarkRouteRoles(IReadOnlyList<RouteLeg> routeLegs, TransportMode mainTransportMode)
    {
        if (routeLegs.Count < 3)
        {
            return;
        }

        routeLegs[0].ConfigureLeg(
            routeLegs[0].GetSequence() ?? 1,
            routeLegs[0].GetStartPoint(),
            routeLegs[0].GetEndPoint(),
            routeLegs[0].GetDistanceKm() ?? 0d,
            TransportMode.TRUCK,
            isFirstMile: true,
            isLastMile: false);

        routeLegs[1].ConfigureLeg(
            routeLegs[1].GetSequence() ?? 2,
            routeLegs[1].GetStartPoint(),
            routeLegs[1].GetEndPoint(),
            routeLegs[1].GetDistanceKm() ?? 0d,
            mainTransportMode,
            isFirstMile: false,
            isLastMile: false);

        routeLegs[2].ConfigureLeg(
            routeLegs[2].GetSequence() ?? 3,
            routeLegs[2].GetStartPoint(),
            routeLegs[2].GetEndPoint(),
            routeLegs[2].GetDistanceKm() ?? 0d,
            TransportMode.TRUCK,
            isFirstMile: false,
            isLastMile: true);
    }

    private sealed record RouteContext(
        string WarehouseAddress,
        string OriginHubAddress,
        string DestinationHubAddress);
}
