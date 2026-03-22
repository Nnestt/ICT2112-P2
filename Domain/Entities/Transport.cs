using System;
using System.Collections.Generic;
using ProRental.Domain.Enums;

namespace ProRental.Domain.Entities;

public partial class Transport
{
    private int _transportId;
    private int TransportId { get => _transportId; set => _transportId = value; }

    private double? _maxLoadKg;
    private double? MaxLoadKg { get => _maxLoadKg; set => _maxLoadKg = value; }

    private double? _vehicleSizeM2;
    private double? VehicleSizeM2 { get => _vehicleSizeM2; set => _vehicleSizeM2 = value; }

    private bool? _isAvailable;
    private bool? IsAvailable { get => _isAvailable; set => _isAvailable = value; }

    public virtual Plane? Plane { get; private set; }

    public virtual ICollection<RouteLeg> RouteLegs { get; private set; } = new List<RouteLeg>();

    public virtual Ship? Ship { get; private set; }

    public virtual Train? Train { get; private set; }

    public virtual Truck? Truck { get; private set; }

    private int getTransportId() => _transportId;

    private string getTransportationType() => (_transportMode ?? default).ToString();

    private void setTransportationType(string transportationType)
    {
        if (Enum.TryParse<TransportMode>(transportationType, true, out var mode))
        {
            _transportMode = mode;
        }
    }

    private float getMaxLoadKG() => (float)(_maxLoadKg ?? 0d);

    private void setMaxLoadKG(float maxLoad)
    {
        _maxLoadKg = maxLoad;
    }

    private float getVehicleSizem2() => (float)(_vehicleSizeM2 ?? 0d);

    private void setVehicleSizem2(float vehicleSizem2)
    {
        _vehicleSizeM2 = vehicleSizem2;
    }

    private bool getIsAvailable() => _isAvailable ?? false;

    private void setIsAvailable(bool isAvailable)
    {
        _isAvailable = isAvailable;
    }

    public int ReadTransportId() => getTransportId();

    public string ReadTransportationType() => getTransportationType();

    public float ReadMaxLoadKg() => getMaxLoadKG();

    public float ReadVehicleSizeM2() => getVehicleSizem2();

    public bool ReadIsAvailable() => getIsAvailable();
}
