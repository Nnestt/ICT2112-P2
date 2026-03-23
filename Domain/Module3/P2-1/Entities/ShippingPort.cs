namespace ProRental.Domain.Entities;

public partial class ShippingPort
{
    // --- Public getters for private scaffolded fields ---
    public int GetHubId() => _hubId;
    public string GetPortCode() => _portCode;
    public string GetPortName() => _portName;
    public string? GetPortType() => _portType;
    public int? GetVesselSize() => _vesselSize;

    // --- Public setters for private scaffolded fields ---
    public void SetPortCode(string portCode) => _portCode = portCode;
    public void SetPortName(string portName) => _portName = portName;
    public void SetPortType(string? portType) => _portType = portType;
    public void SetVesselSize(int? vesselSize) => _vesselSize = vesselSize;

    // --- RDM business methods ---
    public bool CanAccommodateVessel(int requiredSize)
    {
        return (_vesselSize ?? 0) >= requiredSize;
    }
}
