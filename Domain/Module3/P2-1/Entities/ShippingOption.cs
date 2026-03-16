using ProRental.Domain.Enums;

namespace ProRental.Domain.Entities;

public partial class ShippingOption
{
    public PreferenceType? PreferenceType { get; set; }
    public TransportMode? TransportMode { get; set; }
}
