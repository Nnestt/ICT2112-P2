using ProRental.Interfaces.Module3.P2_5;

namespace ProRental.Domain.Module3.P2_5.Controls;

public class PackagingFootprintControl : IPackagingFootprintControl
{
    public float CalculatePackagingFootprint(List<MaterialFootprintDto> materials)
    {
        if (materials == null || !materials.Any())
            return 0f;

        var total = 0f;
        foreach (var material in materials)
        {
            var quantity = material.Quantity;
            var category = material.Category?.ToLowerInvariant() ?? "secondary";

            var categoryFactor = category switch
            {
                "primary" => 1.0f,
                "secondary" => 0.6f,
                _ => 0.7f
            };

            var type = material.MaterialType?.ToLowerInvariant() ?? "";
            var baseFactor = type switch
            {
                "paper" => 0.2f,
                "fabric" => 0.3f,
                "plastic" => 0.6f,
                "foam" => 0.8f,
                "chemical" => 1.0f,
                _ => 0.5f,
            };

            var processingFactor = 1.0f;
            if (material.Recyclable) processingFactor *= 0.85f;
            if (material.Reusable) processingFactor *= 0.80f;

            // footprint = base factor * category factor * processing factor
            var footprintPerUnit = baseFactor * categoryFactor * processingFactor;
            total += footprintPerUnit * quantity;
        }

        return total;
    }
}