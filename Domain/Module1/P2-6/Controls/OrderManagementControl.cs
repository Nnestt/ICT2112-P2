using ProRental.Domain.Entities;
using ProRental.Domain.Enums;
using ProRental.Interfaces.Data;
using ProRental.Interfaces.Domain;

namespace ProRental.Domain.Controls;

public class OrderManagementControl : IOrderService
{
    private readonly IOrderMapper _orderMapper;
    private readonly IInventoryService _inventoryService;

    public OrderManagementControl(IOrderMapper orderMapper, IInventoryService inventoryService)
    {
        _orderMapper      = orderMapper;
        _inventoryService = inventoryService;
    }

    public Order CreateOrder(int customerId, int checkoutId,
                             List<(int productId, int quantity, decimal unitPrice, DateTime rentalStart, DateTime rentalEnd)> itemData,
                             DeliveryDuration deliveryType, decimal totalAmount,
                             Dictionary<int, int> productQuantities)
    {
        // 1. Build the order entity — starts as PENDING
        var order = Order.Create(customerId, checkoutId, totalAmount, deliveryType);

        // 2. Build and attach each item — checkout passes raw data, we own entity construction
        foreach (var i in itemData)
            order.AddItem(Orderitem.Create(i.productId, i.quantity, i.unitPrice, i.rentalStart, i.rentalEnd));

        // 3. Persist — writes Order row + cascades Orderitem rows
        _orderMapper.Insert(order);

        // 4. Call inventory to process the loan
        var loanSuccess = _inventoryService.ProcessLoan(
            order.OrderId,
            customerId,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(GetRentalDays(deliveryType)),
            productQuantities
        );

        // 5. Status transition based on inventory result
        order.UpdateStatus(loanSuccess ? OrderStatus.CONFIRMED : OrderStatus.CANCELLED);
        _orderMapper.Update(order);

        return order;
    }

    public Order GetOrder(int orderId)
    {
        return _orderMapper.FindById(orderId)
            ?? throw new KeyNotFoundException($"Order {orderId} not found.");
    }

    public List<Order> GetOrders()
    {
        return _orderMapper.FindAll();
    }

    public List<Order> GetOrdersByCustomer(int customerId)
    {
        return _orderMapper.FindByCustomer(customerId);
    }

    public OrderStatus GetOrderStatus(int orderId)
    {
        var order = GetOrder(orderId);
        return order.CurrentStatus ?? OrderStatus.PENDING;
    }

    public void UpdateOrderStatus(int orderId, OrderStatus status)
    {
        var order = GetOrder(orderId);
        order.UpdateStatus(status);
        _orderMapper.Update(order);
    }

    public bool CancelOrder(int orderId)
    {
        var order = _orderMapper.FindById(orderId);
        if (order == null) return false;

        var nonCancellable = new[] { OrderStatus.DISPATCHED, OrderStatus.DELIVERED, OrderStatus.CANCELLED };
        if (order.CurrentStatus.HasValue && nonCancellable.Contains(order.CurrentStatus.Value))
            return false;

        order.UpdateStatus(OrderStatus.CANCELLED);
        _orderMapper.Update(order);
        return true;
    }

    // ── Private helpers ───────────────────────────────────────────────────

    private static int GetRentalDays(DeliveryDuration deliveryType) => deliveryType switch
    {
        DeliveryDuration.NextDay   => 1,
        DeliveryDuration.ThreeDays => 3,
        DeliveryDuration.OneWeek   => 7,
        _                          => 7
    };
}