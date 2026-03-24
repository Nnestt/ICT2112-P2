using ProRental.Domain.Entities;

namespace ProRental.Data.Module3.P2_5.Interfaces;

public interface IPackagingMaterialGateway
{
    Packagingmaterial FindById(int materialId);
    List<Packagingmaterial> FindAll();
    void Save(Packagingmaterial material);
}
