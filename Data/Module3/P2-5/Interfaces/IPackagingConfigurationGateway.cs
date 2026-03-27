using ProRental.Domain.Entities;

namespace ProRental.Data.Module3.P2_5.Interfaces;

public interface IPackagingConfigurationGateway
{
    void SaveConfiguration(int profileId);
    void SaveMaterials(int configurationId, List<Packagingmaterial> items, string category, int quantity);
    Packagingconfiguration FindByConfigurationId(int configurationId);
    int GetConfigurationId(Packagingconfiguration config);
}
