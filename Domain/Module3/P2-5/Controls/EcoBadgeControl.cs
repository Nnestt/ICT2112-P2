using ProRental.Domain.Module3.P2_5.Entities;
using ProRental.Interfaces.Module3.P2_5;

namespace ProRental.Domain.Module3.P2_5.Controls;

public class EcoBadgeControl : IEcoFriendlyService
{
    private readonly List<EcoTier> _tiers =
    [
        EcoTier.Gold,
        EcoTier.Silver,
        EcoTier.Bronze,
        EcoTier.Standard
    ];

    public bool IsEcoFriendly(decimal carbonG)
    {
        return AssignTier(carbonG) != EcoTier.Standard;
    }

    public EcoTier AssignTier(decimal carbonG)
    {
        return carbonG switch
        {
            <= 120m => EcoTier.Gold,
            <= 180m => EcoTier.Silver,
            <= 250m => EcoTier.Bronze,
            _ => _tiers.Last()
        };
    }

    public EcoBadge GetBadge(EcoTier tier)
    {
        return new EcoBadge(tier.ToString());
    }
}
