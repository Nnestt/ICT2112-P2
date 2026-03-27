using ProRental.Domain.Module2.P2_3.Entities;

namespace ProRental.Data.Module3.P2_5.Interfaces;

public interface IProductCatalogGateway
{
    List<ProductCatalogRecord> GetProductsForDropdown();
    bool ProductExists(int productId);
}
