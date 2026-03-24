using Microsoft.EntityFrameworkCore;

namespace ProRental.Domain.Entities;

public partial class Packagingprofile
{
    public Packagingmaterial? DeterminePrimaryPackaging(string? fragilityLevel, List<Packagingmaterial> candidateMaterials)
    {
        if (candidateMaterials == null || !candidateMaterials.Any())
            return null;

        var fragility = fragilityLevel?.ToLowerInvariant() ?? "low";

        if (fragility.Contains("high"))
            return candidateMaterials.FirstOrDefault(m => EF.Property<string?>(m, "Type")?.Equals("cushion", StringComparison.OrdinalIgnoreCase) == true)
                ?? candidateMaterials.First();

        if (fragility.Contains("medium"))
            return candidateMaterials.FirstOrDefault(m => EF.Property<string?>(m, "Type")?.Equals("bubble", StringComparison.OrdinalIgnoreCase) == true)
                ?? candidateMaterials.First();

        return candidateMaterials.FirstOrDefault(m => EF.Property<string?>(m, "Type")?.Equals("box", StringComparison.OrdinalIgnoreCase) == true)
            ?? candidateMaterials.First();
    }
    
    public List<Packagingmaterial> DetermineSecondaryPackaging(double volume, List<Packagingmaterial> candidateMaterials)
    {
        if (candidateMaterials == null || !candidateMaterials.Any())
            return new List<Packagingmaterial>();

        var target = volume > 3.0 ? 3 : 2;
        target = Math.Min(target, candidateMaterials.Count);
        
        var ordered = candidateMaterials
            .OrderBy(m => {
                var t = EF.Property<string?>(m, "Type")?.ToLowerInvariant() ?? "";
                return t switch
                {
                    "plastic" => 1,
                    "fabric" => 2,
                    "paper" => 3,
                    "foam" => 4,
                    "chemical" => 5,
                    _ => 6
                };
            })
            .ThenBy(m => EF.Property<string?>(m, "Name"))
            .ToList();

        return ordered.Take(target).ToList();
    }
}
