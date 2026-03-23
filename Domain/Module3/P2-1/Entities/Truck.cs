namespace ProRental.Domain.Entities;

public partial class Truck
{
    public int ReadTransportId() => getTransportId();

    public int ReadTruckId() => getTruckID();

    public string ReadTruckType() => getTruckType();

    public string ReadLicensePlate() => getLicensePlate();
}
