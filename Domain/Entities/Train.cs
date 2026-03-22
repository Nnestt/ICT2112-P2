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

    private int getTransportId() => _transportId;

    private int getTrainID() => _trainId;

    private void setTrainID(int trainId)
    {
        _trainId = trainId;
    }

    private string getTrainType() => _trainType ?? string.Empty;

    private void setTrainType(string trainType)
    {
        _trainType = trainType;
    }

    private string getTrainNumber() => _trainNumber ?? string.Empty;

    private void setTrainNumber(string trainNumber)
    {
        _trainNumber = trainNumber;
    }

    public int ReadTransportId() => getTransportId();

    public int ReadTrainId() => getTrainID();

    public string ReadTrainType() => getTrainType();

    public string ReadTrainNumber() => getTrainNumber();
}
