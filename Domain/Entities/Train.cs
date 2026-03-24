using System;
using System.Collections.Generic;

namespace ProRental.Domain.Entities;

public partial class Train
{
    private int _transportId;
    private int TransportId { get => _transportId; set => _transportId = value; }

    private int _trainId;
    private int TrainId { get => _trainId; set => _trainId = value; }

    private string? _trainType;
    private string? TrainType { get => _trainType; set => _trainType = value; }

    private string? _trainNumber;
    private string? TrainNumber { get => _trainNumber; set => _trainNumber = value; }

    public virtual Transport Transport { get; private set; } = null!;
}
