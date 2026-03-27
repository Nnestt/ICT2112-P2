using ProRental.Domain.Entities;
using ProRental.Domain.Module3.P2_5;

namespace ProRental.Data.Module3.P2_5.Interfaces;

public interface IPackagingProfileGateway
{
    void Save(int orderId, double volume, string fragilityLevel);
    Packagingprofile FindByProfileId(int profileId);
    Packagingprofile FindByOrderId(int orderId);
    List<PackagingFootprintProfileDto> GetAllFootprintProfiles();
    int GetProfileId(Packagingprofile profile);
    string GetFragilityLevel(Packagingprofile profile);
    double GetVolume(Packagingprofile profile);
}
