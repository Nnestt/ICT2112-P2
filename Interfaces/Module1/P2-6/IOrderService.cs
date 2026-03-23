using ProRental.Domain.Entities;
using ProRental.Domain.Enums;

namespace ProRental.Interfaces.Domain;

public interface IOrderService
{
    Order CreateOrder(int customerId, int checkoutId,
                      List<(int productId, int quantity, decimal unitPrice, DateTime rentalStart, DateTime rentalEnd)> itemData,
                      DeliveryDuration deliveryType, decimal totalAmount,
                      Dictionary<int, int> productQuantities);

    Order GetOrder(int orderId);
    List<Order> GetOrders();
    List<Order> GetOrdersByCustomer(int customerId);
    OrderStatus GetOrderStatus(int orderId);
    void UpdateOrderStatus(int orderId, OrderStatus status);
    bool CancelOrder(int orderId);
}