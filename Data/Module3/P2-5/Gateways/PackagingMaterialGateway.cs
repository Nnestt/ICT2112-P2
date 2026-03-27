using Microsoft.EntityFrameworkCore;
using ProRental.Data.Module3.P2_5.Interfaces;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;

namespace ProRental.Data.Module3.P2_5.Gateways;

public class PackagingMaterialGateway : IPackagingMaterialGateway
{
    private readonly AppDbContext _db;

    public PackagingMaterialGateway(AppDbContext db)
    {
        _db = db;
    }

    public Packagingmaterial FindById(int materialId)
    {
        var mat = _db.Packagingmaterials
            .Where(m => EF.Property<int>(m, "Materialid") == materialId)
            .FirstOrDefault();

        return mat!;
    }

    public List<Packagingmaterial> FindAll()
    {
        return _db.Packagingmaterials
            .ToList();
    }

    public void Save(Packagingmaterial material)
    {
        _db.Packagingmaterials.Add(material);
        _db.SaveChanges();
    }
}
