using ProRental.Domain.Enums;

namespace ProRental.Domain.Entities;

/// <summary>
/// Feature 1 accessors for the shared RouteLeg entity used by deferred route quoting.
/// by: ernest
/// </summary>
public partial class RouteLeg
{
    public int GetLegId() => _legId;
    public int GetRouteId() => _routeId;
    public int? GetSequence() => _sequence;
    public string GetStartPoint() => _startPoint ?? string.Empty;
    public string GetEndPoint() => _endPoint ?? string.Empty;
    public double? GetDistanceKm() => _distanceKm;
    public bool? GetIsFirstMile() => _isFirstMile;
    public bool? GetIsLastMile() => _isLastMile;
    public TransportMode? GetTransportMode() => _transportMode;

    public void ConfigureLeg(
        int sequence,
        string startPoint,
        string endPoint,
        double distanceKm,
        TransportMode transportMode,
        bool isFirstMile,
        bool isLastMile)
    {
        _sequence = sequence;
        _startPoint = startPoint;
        _endPoint = endPoint;
        _distanceKm = distanceKm;
        _transportMode = transportMode;
        _isFirstMile = isFirstMile;
        _isLastMile = isLastMile;
    }
}
