using ProRental.Domain.Module2.P2_3.Entities;
using ProRental.Domain.Module3.P2_5.Entities;
using ProRental.Interfaces.Module2.P2_3;

namespace ProRental.Domain.Module2.P2_3.Mappers;

public sealed class ProductCatalogMapper : IProductCatalogMapper
{
    public ProductDropdownItem ToDropdownItem(ProductCatalogRecord product)
    {
        return new ProductDropdownItem(product.ProductId, product.Sku);
    }
}
