using ProRental.Domain.Enums;

namespace ProRental.Domain.Entities;

public partial class Order
{
    // ── Enum-typed fields (mapped via AppDbContext.Custom.cs) ────────────
    private OrderStatus? _status;
    private OrderStatus? Status { get => _status; set => _status = value; }

    private DeliveryDuration? _deliveryType;
    private DeliveryDuration? DeliveryType { get => _deliveryType; set => _deliveryType = value; }

    // ── Public accessors ─────────────────────────────────────────────────
    public int OrderId                            => Orderid;
    public int CustomerId                         => Customerid;
    public int CheckoutId                         => Checkoutid;
    public DateTime OrderDate                     => Orderdate;
    public decimal TotalAmount                    => Totalamount;
    public OrderStatus? CurrentStatus             => _status;
    public DeliveryDuration? DeliveryDurationType => _deliveryType;

    // ── Mutators ─────────────────────────────────────────────────────────
    public void UpdateStatus(OrderStatus newStatus)        => _status = newStatus;
    public void UpdateDeliveryType(DeliveryDuration type)  => _deliveryType = type;

    // ── Navigation helper ─────────────────────────────────────────────────
    public void AddItem(Orderitem item) => Orderitems.Add(item);

    // ── Factory method ───────────────────────────────────────────────────
    public static Order Create(int customerId, int checkoutId, decimal totalAmount, DeliveryDuration deliveryType)
{
    var order = new Order();
    typeof(Order).GetProperty("Customerid",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
        .SetValue(order, customerId);
    typeof(Order).GetProperty("Checkoutid",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
        .SetValue(order, checkoutId);
    typeof(Order).GetProperty("Totalamount",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
        .SetValue(order, totalAmount);
    typeof(Order).GetProperty("Orderdate",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
        .SetValue(order, DateTime.UtcNow);
    order._status       = OrderStatus.PENDING;
    order._deliveryType = deliveryType;
    return order;
}
}