using ProRental.Data.Module3.P2_5.Interfaces;
using ProRental.Domain.Module3.P2_5.Entities;
using ProRental.Interfaces.Module2.P2_3;

namespace ProRental.Domain.Module2.P2_3.Controls;

public sealed class ProductCatalogControl : IProductCatalogService
{
    private readonly IProductCatalogMapper _productCatalogMapper;
    private readonly IProductCatalogGateway _productCatalogGateway;

    public ProductCatalogControl(
        IProductCatalogMapper productCatalogMapper,
        IProductCatalogGateway productCatalogGateway)
    {
        _productCatalogMapper = productCatalogMapper;
        _productCatalogGateway = productCatalogGateway;
    }

    public List<ProductDropdownItem> GetProductDropdownItems()
    {
        return _productCatalogGateway.GetProductsForDropdown()
            .Select(_productCatalogMapper.ToDropdownItem)
            .ToList();
    }

    public bool ProductExists(int productId)
    {
        return _productCatalogGateway.ProductExists(productId);
    }
}
