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
}
