using System;
using System.Collections.Generic;

namespace ProRental.Domain.Entities;

public partial class Ship
{
    private int _transportId;
    private int TransportId { get => _transportId; set => _transportId = value; }

    private int _shipId;
    private int ShipId { get => _shipId; set => _shipId = value; }

    private string? _vesselType;
    private string? VesselType { get => _vesselType; set => _vesselType = value; }

    private string? _vesselNumber;
    private string? VesselNumber { get => _vesselNumber; set => _vesselNumber = value; }

    private string? _maxVesselSize;
    private string? MaxVesselSize { get => _maxVesselSize; set => _maxVesselSize = value; }

    public virtual Transport Transport { get; private set; } = null!;

    private int getTransportId() => _transportId;

    private int getShipID() => _shipId;

    private void setShipID(int shipId)
    {
        _shipId = shipId;
    }

    private string getVesselType() => _vesselType ?? string.Empty;

    private void setVesselType(string vesselType)
    {
        _vesselType = vesselType;
    }

    private string getVesselNumber() => _vesselNumber ?? string.Empty;

    private void setVesselNumber(string vesselNumber)
    {
        _vesselNumber = vesselNumber;
    }

    private string getMaxVesselSize() => _maxVesselSize ?? string.Empty;

    private void setMaxVesselSize(string maxVesselSize)
    {
        _maxVesselSize = maxVesselSize;
    }

    public int ReadTransportId() => getTransportId();

    public int ReadShipId() => getShipID();

    public string ReadVesselType() => getVesselType();

    public string ReadVesselNumber() => getVesselNumber();

    public string ReadMaxVesselSize() => getMaxVesselSize();
}
