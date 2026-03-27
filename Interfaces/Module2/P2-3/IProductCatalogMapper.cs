using ProRental.Domain.Module2.P2_3.Entities;
using ProRental.Domain.Module3.P2_5.Entities;

namespace ProRental.Interfaces.Module2.P2_3;

public interface IProductCatalogMapper
{
    ProductDropdownItem ToDropdownItem(ProductCatalogRecord product);
}
