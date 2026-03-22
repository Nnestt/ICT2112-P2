namespace ProRental.Domain.Entities;

public partial class DeliveryRoute
{
    public int GetRouteId() => _routeId;
    public string GetOriginAddress() => _originAddress;
    public string GetDestinationAddress() => _destinationAddress;
    public double? GetTotalDistanceKm() => _totalDistanceKm;
    public bool? GetIsValid() => _isValid;
    public int? GetOriginHubId() => _originHubId;
    public int? GetDestinationHubId() => _destinationHubId;

    public void SetOriginAddress(string originAddress) => _originAddress = originAddress;
    public void SetDestinationAddress(string destinationAddress) => _destinationAddress = destinationAddress;
    public void SetTotalDistanceKm(double totalDistanceKm) => _totalDistanceKm = totalDistanceKm;
    public void SetIsValid(bool isValid) => _isValid = isValid;
    public void SetOriginHubId(int? originHubId) => _originHubId = originHubId;
    public void SetDestinationHubId(int? destinationHubId) => _destinationHubId = destinationHubId;
}
