using ProRental.Domain.Enums;

namespace ProRental.Domain.Entities;

public partial class TransportationHub
{
    // --- HubType (added by P2-1, mapped in AppDbContext.Custom.cs) ---
    private HubType? _hubType;
    private HubType? HubType { get => _hubType; set => _hubType = value; }
    public void UpdateHubType(HubType newValue) => _hubType = newValue;

    // --- Public getters for private scaffolded fields ---
    public int GetHubId() => _hubId;
    public double GetLongitude() => _longitude;
    public double GetLatitude() => _latitude;
    public string GetCountryCode() => _countryCode;
    public string GetAddress() => _address;
    public string? GetOperationalStatus() => _operationalStatus;
    public string? GetOperationTime() => _operationTime;
    public HubType? GetHubType() => _hubType;

    // --- Public setters for private scaffolded fields ---
    public void SetHubId(int hubId) => _hubId = hubId;
    public void SetHubType(HubType? hubType) => _hubType = hubType;
    public void SetLongitude(double longitude) => _longitude = longitude;
    public void SetLatitude(double latitude) => _latitude = latitude;
    public void SetCountryCode(string countryCode) => _countryCode = countryCode;
    public void SetAddress(string address) => _address = address;
    public void SetOperationalStatus(string? operationalStatus) => _operationalStatus = operationalStatus;
    public void SetOperationTime(string? operationTime) => _operationTime = operationTime;

    // --- RDM business methods ---
    public bool IsOperational() => _operationalStatus == "OPERATIONAL";
}