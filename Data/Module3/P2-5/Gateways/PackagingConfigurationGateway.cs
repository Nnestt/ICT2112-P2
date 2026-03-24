using Microsoft.EntityFrameworkCore;
using ProRental.Data.Module3.P2_5.Interfaces;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;

namespace ProRental.Data.Module3.P2_5.Gateways;

public class PackagingConfigurationGateway : IPackagingConfigurationGateway
{
    private readonly AppDbContext _db;

    public PackagingConfigurationGateway(AppDbContext db)
    {
        _db = db;
    }

    public void SaveConfiguration(int profileId)
    {
        var config = new Packagingconfiguration();
        _db.Packagingconfigurations.Add(config);
        _db.Entry(config).Property("Profileid").CurrentValue = profileId;
        _db.SaveChanges();
    }

    public void SaveMaterials(int configurationId, List<Packagingmaterial> items, string category, int quantity)
    {
        foreach (var material in items)
        {
            var materialId = _db.Entry(material).Property<int>("Materialid").CurrentValue;

            var configMaterial = new Packagingconfigmaterial();
            _db.Packagingconfigmaterials.Add(configMaterial);
            _db.Entry(configMaterial).CurrentValues.SetValues(new Dictionary<string, object>
            {
                ["Configurationid"] = configurationId,
                ["Materialid"] = materialId,
                ["Category"] = category,
                ["Quantity"] = quantity
            });
            _db.SaveChanges();
        }
    }

    public Packagingconfiguration FindByConfigurationId(int configurationId)
    {
        var config = _db.Packagingconfigurations
            .Where(c => EF.Property<int>(c, "Configurationid") == configurationId)
            .Include(c => c.Packagingconfigmaterials)
                .ThenInclude(cm => cm.Material)
            .FirstOrDefault();

        return config!;
    }

    public int GetConfigurationId(Packagingconfiguration config) => _db.Entry(config).Property<int>("Configurationid").CurrentValue;
}
