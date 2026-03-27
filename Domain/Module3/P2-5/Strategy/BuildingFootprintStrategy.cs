using ProRental.Interfaces.Module3;

namespace ProRental.Domain.Entities.Module3;

public class BuildingFootprintStrategy : ICalculateCarbonStrategy
{
    private const double CalibrationConstant = 0.000729;

    private static readonly IReadOnlyDictionary<string, double> ZoneWeights =
        new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
        {
            { "North", 1.00 },
            { "South", 1.25 },
            { "East", 1.10 },
            { "West", 1.15 },
            { "Central", 1.35 }
        };

    private static readonly IReadOnlyDictionary<string, double> FloorWeights =
        new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
        {
            { "Level 1", 1.00 },
            { "Level 2", 1.20 },
            { "Level 3", 1.45 },
            { "Level 4", 1.60 },
            { "Level 5", 1.75 }
        };

    public double CalculateFootprint(params double[] values)
    {
        if (values.Length != 4)
        {
            throw new ArgumentException("Building footprint strategy requires room size, CO2 level, zone weight, and floor weight.");
        }

        var roomSize = values[0];
        var co2Level = values[1];
        var zoneWeight = values[2];
        var floorWeight = values[3];

        if (roomSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(values), "Room size must be a positive number.");
        }

        if (co2Level <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(values), "CO2 level must be a positive number.");
        }

        if (zoneWeight <= 0 || floorWeight <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(values), "Zone and floor weights must be positive numbers.");
        }

        return Math.Round(roomSize * co2Level * zoneWeight * floorWeight * CalibrationConstant, 2);
    }

    public double CalculateFootprint(double roomSize, double co2Level, string zone, string floor)
    {
        if (!ZoneWeights.TryGetValue(zone, out var zoneWeight))
        {
            throw new ArgumentException("zone must be one of: North, South, East, West, Central.", nameof(zone));
        }

        if (!FloorWeights.TryGetValue(floor, out var floorWeight))
        {
            throw new ArgumentException("floor must be one of: Level 1, Level 2, Level 3, Level 4, Level 5.", nameof(floor));
        }

        return CalculateFootprint(roomSize, co2Level, zoneWeight, floorWeight);
    }
}
