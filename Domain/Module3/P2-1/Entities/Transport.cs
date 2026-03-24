using ProRental.Domain.Enums;

namespace ProRental.Domain.Entities;

public partial class Transport
{
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

    private TransportMode? _transportMode;
    private TransportMode? TransportMode { get => _transportMode; set => _transportMode = value; }

    public int ReadTransportId() => getTransportId();

    public string ReadTransportationType() => getTransportationType();

    public float ReadMaxLoadKg() => getMaxLoadKG();

    public float ReadVehicleSizeM2() => getVehicleSizem2();

    public bool ReadIsAvailable() => getIsAvailable();
}
