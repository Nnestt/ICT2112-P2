namespace ProRental.Domain.Enums;

public enum OrderStatus
{
    PENDING,
    CONFIRMED,
    PROCESSING,
    READY_FOR_DISPATCH,
    DISPATCHED,
    DELIVERED,
    CANCELLED
}
