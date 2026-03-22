using ProRental.Domain.Enums;

namespace ProRental.Domain.Module3.P2_1.Models;

public class TransportRouteLegInput
{
    public TransportMode Mode { get; init; }
    public string StartPoint { get; init; } = string.Empty;
    public string EndPoint { get; init; } = string.Empty;
}
