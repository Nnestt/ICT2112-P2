// using ProRental.Domain.Entities;
// using ProRental.Domain.Enums;

// namespace ProRental.Interfaces.Domain;

// public interface IInventoryService
// {
//     List<Product> GetAllProducts();
//     Product GetProduct(int productId);
//     List<Product> GetProductsByCategory(int categoryId);
//     int CheckProductQuantityByStatus(int productId, string status);
//     ProductStatus CheckProductStatus(int productId);
//     void ReserveItems(Order order, List<int> productIds);
//     bool AddProductToOrder(Order order, int productId);
//     void ReleaseItems(Order order, List<int> inventoryItemIds);
//     List<Inventoryitem> GetInventoryItemsByStatus(string status);
//     List<Product> GetProductsByStatus(string status);
//     bool TriggerReturnProcess(Order order);
// }