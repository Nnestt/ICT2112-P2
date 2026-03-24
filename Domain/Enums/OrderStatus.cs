namespace ProRental.Domain.Enums;

public enum OrderStatus
{
    PENDING,
    CONFIRMED,
    PACKING,
    READY_FOR_DISPATCH,
    DISPATCHED,
    DELIVERED,
    IN_RENTAL,
    CANCELLED,
    RETURN_PICKUP,
    RETURNED,
    INSPECTION,
    REFUND_PROCESSING,
    COMPLETED
}
