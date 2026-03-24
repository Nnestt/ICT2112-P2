namespace ProRental.Domain.Entities;

public partial class Plane
{
    private int getPlaneID() => _planeId;

    private void setPlaneID(int planeId)
    {
        _planeId = planeId;
    }

    private string getPlaneType() => _planeType ?? string.Empty;

    private void setPlaneType(string planeType)
    {
        _planeType = planeType;
    }

    private string getPlaneCallsign() => _planeCallsign ?? string.Empty;

    private void setPlaneCallsign(string planeCallsign)
    {
        _planeCallsign = planeCallsign;
    }

    public int ReadPlaneId() => getPlaneID();

    public string ReadPlaneType() => getPlaneType();

    public string ReadPlaneCallsign() => getPlaneCallsign();
}
