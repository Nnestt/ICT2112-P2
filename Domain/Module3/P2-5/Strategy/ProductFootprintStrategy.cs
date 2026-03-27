using ProRental.Interfaces.Module3;

namespace ProRental.Domain.Entities.Module3;

public class ProductFootprintStrategy : ICalculateCarbonStrategy
{
    public double CalculateFootprint(params double[] values)
    {
        if (values.Length != 2)
        {
            throw new ArgumentException("Product footprint strategy requires product mass and toxic percentage.");
        }

        var productMass = values[0];
        var toxicPercentage = values[1];

        if (productMass < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(values), "Product mass cannot be negative.");
        }

        if (toxicPercentage < 0 || toxicPercentage > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(values), "Toxic percentage must be between 0 and 100.");
        }

        return Math.Round(productMass * 0.5 * (1 + (toxicPercentage / 100.0)), 2);
    }
}
