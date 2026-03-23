// using ProRental.Domain.Entities;
// using ProRental.Interfaces.Domain;
// using ProRental.Domain.Enums;

// namespace ProRental.Domain.Controls;

// public class InventoryService : IInventoryService
// {
//     public List<Product> GetAllProducts() => new();
    
//     public Product GetProduct(int productId) => new();
    
//     public List<Product> GetProductsByCategory(int categoryId) => new();
    
//     public int CheckProductQuantityByStatus(int productId, string status) 
//         => 0;
    
//     public ProductStatus CheckProductStatus(int productId) 
//         => ProductStatus.AVAILABLE;
    
//     public void ReserveItems(Order order, List<int> productIds) { }
    
//     public bool AddProductToOrder(Order order, int productId) => true;
    
//     public void ReleaseItems(Order order, List<int> inventoryItemIds) { }
    
//     public List<Inventoryitem> GetInventoryItemsByStatus(string status) 
//         => new();
    
//     public List<Product> GetProductsByStatus(string status) => new();
    
//     public bool TriggerReturnProcess(Order order) => true;
// }