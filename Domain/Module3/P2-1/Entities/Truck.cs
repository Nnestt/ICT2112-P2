namespace ProRental.Domain.Entities;

public partial class Truck
{
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

    public int ReadTruckId() => getTruckID();

    public string ReadTruckType() => getTruckType();

    public string ReadLicensePlate() => getLicensePlate();
}
