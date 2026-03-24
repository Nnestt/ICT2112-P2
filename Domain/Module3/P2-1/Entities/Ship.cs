namespace ProRental.Domain.Entities;

public partial class Ship
{
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

    public int ReadShipId() => getShipID();

    public string ReadVesselType() => getVesselType();

    public string ReadVesselNumber() => getVesselNumber();

    public string ReadMaxVesselSize() => getMaxVesselSize();
}
