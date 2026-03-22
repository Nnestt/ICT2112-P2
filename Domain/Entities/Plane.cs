using System;
using System.Collections.Generic;

namespace ProRental.Domain.Entities;

public partial class Plane
{
    private int _transportId;
    private int TransportId { get => _transportId; set => _transportId = value; }

    private int _planeId;
    private int PlaneId { get => _planeId; set => _planeId = value; }

    private string? _planeType;
    private string? PlaneType { get => _planeType; set => _planeType = value; }

    private string? _planeCallsign;
    private string? PlaneCallsign { get => _planeCallsign; set => _planeCallsign = value; }

    public virtual Transport Transport { get; private set; } = null!;

    private int getTransportId() => _transportId;

    private int getPlaneID() => _planeId;

    private void setPlaneID(int planeId)
    {
        _planeId = planeId;
    }

    private string getPlaneType() => _planeType ?? string.Empty;

    private void setPlaneType(string planeType)
    {
        _planeType = planeType;
    }

    private string getPlaneCallsign() => _planeCallsign ?? string.Empty;

    private void setPlaneCallsign(string planeCallsign)
    {
        _planeCallsign = planeCallsign;
    }

    public int ReadTransportId() => getTransportId();

    public int ReadPlaneId() => getPlaneID();

    public string ReadPlaneType() => getPlaneType();

    public string ReadPlaneCallsign() => getPlaneCallsign();
}
