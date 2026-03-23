namespace ProRental.Domain.Entities;

public partial class Airport
{
    // --- Public getters for private scaffolded fields ---
    public int GetHubId() => _hubId;
    public string GetAirportCode() => _airportCode;
    public string GetAirportName() => _airportName;
    public int? GetTerminal() => _terminal;
    public int? GetAircraftSize() => _aircraftSize;

    // --- Public setters for private scaffolded fields ---
    public void SetAirportCode(string airportCode) => _airportCode = airportCode;
    public void SetAirportName(string airportName) => _airportName = airportName;
    public void SetTerminal(int? terminal) => _terminal = terminal;
    public void SetAircraftSize(int? aircraftSize) => _aircraftSize = aircraftSize;

    // --- RDM business methods ---
    public bool CanAccommodateAircraft(int requiredSize)
    {
        return (_aircraftSize ?? 0) >= requiredSize;
    }
}
