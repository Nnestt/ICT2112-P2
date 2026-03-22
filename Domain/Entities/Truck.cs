using System;
using System.Collections.Generic;

namespace ProRental.Domain.Entities;

public partial class Truck
{
    private int _transportId;
    private int TransportId { get => _transportId; set => _transportId = value; }

    private int _truckId;
    private int TruckId { get => _truckId; set => _truckId = value; }

    private string? _truckType;
    private string? TruckType { get => _truckType; set => _truckType = value; }

    private string? _licensePlate;
    private string? LicensePlate { get => _licensePlate; set => _licensePlate = value; }

    public virtual Transport Transport { get; private set; } = null!;

    private int getTransportId() => _transportId;

    private int getTruckID() => _truckId;

    private void setTruckID(int truckId)
    {
        _truckId = truckId;
    }

    private string getTruckType() => _truckType ?? string.Empty;

    private void setTruckType(string truckType)
    {
        _truckType = truckType;
    }

    private string getLicensePlate() => _licensePlate ?? string.Empty;

    private void setLicensePlate(string licensePlate)
    {
        _licensePlate = licensePlate;
    }

    public int ReadTransportId() => getTransportId();

    public int ReadTruckId() => getTruckID();

    public string ReadTruckType() => getTruckType();

    public string ReadLicensePlate() => getLicensePlate();
}
