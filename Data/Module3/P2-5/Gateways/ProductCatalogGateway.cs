using Microsoft.EntityFrameworkCore;
using ProRental.Data.Module3.P2_5.Interfaces;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Module2.P2_3.Entities;

namespace ProRental.Data.Module3.P2_5.Gateways;

public sealed class ProductCatalogGateway : IProductCatalogGateway
{
    private readonly AppDbContext _dbContext;

    public ProductCatalogGateway(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<ProductCatalogRecord> GetProductsForDropdown()
    {
        return _dbContext.Products
            .OrderBy(product => EF.Property<string>(product, "Sku"))
            .Select(product => new ProductCatalogRecord(
                EF.Property<int>(product, "Productid"),
                EF.Property<string>(product, "Sku")))
            .ToList();
    }

    public bool ProductExists(int productId)
    {
        return _dbContext.Products.Any(product => EF.Property<int>(product, "Productid") == productId);
    }
}
