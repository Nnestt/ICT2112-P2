using ProRental.Interfaces.Module3.P2_1;

namespace ProRental.Domain.Module3.P2_1.Controls;

public class HubCarbonService : IHubCarbonService
{
    private static readonly Dictionary<(string ProductId, string HubId), float> StorageCarbonByProductAndHub = new()
    {
        { ("P100", "HUB-A"), 4f },
        { ("P200", "HUB-B"), 0f },
        { ("P300", "HUB-C"), 1.5f }
    };

    public float CalculateProductStorageCarbon(string productId, string hubId)
    {
        return StorageCarbonByProductAndHub.TryGetValue((productId, hubId), out var storageCo2)
            ? storageCo2
            : 0f;
    }
}
