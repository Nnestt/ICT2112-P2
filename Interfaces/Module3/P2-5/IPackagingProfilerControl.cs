using ProRental.Domain.Entities;

namespace ProRental.Interfaces.Module3.P2_5;

public interface IPackagingProfilerControl
{
    Packagingprofile CreatePackagingProfile(int orderId, float volume, string fragilityLevel);
    Packagingconfiguration CreatePackagingConfiguration(Packagingprofile profile);
    List<dynamic> GetAllPackagingFootprints();
}
