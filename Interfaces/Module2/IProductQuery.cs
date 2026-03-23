namespace ProRental.Interfaces;

/// <summary>
/// Stub interface for product lookups (implemented by another group).
/// AnalyticsControl uses this to resolve product names for trend analytics.
/// NOTE: This is a cross-group dependency — only stub methods used by analytics are defined here.
/// </summary>
public interface IProductQuery
{
    Task<ProductDto?> GetProductByIdAsync(int productID);
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
}

public class ProductDto
{
    public int ProductID { get; set; }
    public string Name { get; set; } = string.Empty;
}
