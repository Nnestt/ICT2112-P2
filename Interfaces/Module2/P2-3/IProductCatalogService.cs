using ProRental.Domain.Module3.P2_5.Entities;

namespace ProRental.Interfaces.Module2.P2_3;

public interface IProductCatalogService
{
    List<ProductDropdownItem> GetProductDropdownItems();
    bool ProductExists(int productId);
}
