using Microsoft.EntityFrameworkCore;
using ProRental.Data.Module3.P2_5.Interfaces;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;
using ProRental.Domain.Module3.P2_5;

namespace ProRental.Data.Module3.P2_5.Gateways;

public class PackagingProfileGateway : IPackagingProfileGateway
{
    private readonly AppDbContext _db;

    public PackagingProfileGateway(AppDbContext db)
    {
        _db = db;
    }

    public void Save(int orderId, double volume, string fragilityLevel)
    {
        var profile = new Packagingprofile();
        _db.Packagingprofiles.Add(profile);
        _db.Entry(profile).CurrentValues.SetValues(new Dictionary<string, object>
        {
            ["Orderid"] = orderId,
            ["Volume"] = volume,
            ["Fragilitylevel"] = fragilityLevel.ToLowerInvariant()
        });
        _db.SaveChanges();
    }

    public Packagingprofile FindByProfileId(int profileId)
    {
        var profile = _db.Packagingprofiles
            .Where(p => EF.Property<int>(p, "Profileid") == profileId)
            .FirstOrDefault();
        
        return profile!;
    }

    public Packagingprofile FindByOrderId(int orderId)
    {
        var profile = _db.Packagingprofiles
            .Include(p => p.Packagingconfigurations)
                .ThenInclude(c => c.Packagingconfigmaterials)
                    .ThenInclude(cm => cm.Material)
            .Where(p => EF.Property<int>(p, "Orderid") == orderId)
            .FirstOrDefault();

        return profile!;
    }

    public List<PackagingFootprintProfileDto> GetAllFootprintProfiles()
    {
        return _db.Packagingprofiles
            .Select(p => new PackagingFootprintProfileDto
            {
                OrderId = EF.Property<int>(p, "Orderid"),
                ProfileId = EF.Property<int>(p, "Profileid"),
                FragilityLevel = EF.Property<string>(p, "Fragilitylevel"),
                Volume = EF.Property<double>(p, "Volume"),
                ProductName = EF.Property<string>(p.Order.Orderitems.FirstOrDefault()!.Product.Productdetail!, "Name") ?? "Unknown",
                Materials = p.Packagingconfigurations.FirstOrDefault()!.Packagingconfigmaterials
                    .Select(cm => new MaterialFootprintDto
                    {
                        MaterialName = EF.Property<string>(cm.Material, "Name"),
                        Quantity = EF.Property<int>(cm, "Quantity"),
                        Category = EF.Property<string>(cm, "Category"),
                        MaterialType = EF.Property<string>(cm.Material, "Type"),
                        Recyclable = EF.Property<bool>(cm.Material, "Recyclable"),
                        Reusable = EF.Property<bool>(cm.Material, "Reusable")
                    }).ToList()
            })
            .ToList();
    }

    public int GetProfileId(Packagingprofile profile) => _db.Entry(profile).Property<int>("Profileid").CurrentValue;
    public string GetFragilityLevel(Packagingprofile profile) => _db.Entry(profile).Property<string>("Fragilitylevel").CurrentValue ?? "low";
    public double GetVolume(Packagingprofile profile) => _db.Entry(profile).Property<double>("Volume").CurrentValue;
}
