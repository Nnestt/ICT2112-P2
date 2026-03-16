namespace ProRental.Domain.Enums;

public enum OrderHistoryStatus
{
    PENDING,
    CONFIRMED,
    PROCESSING,
    READY_FOR_DISPATCH,
    DISPATCHED,
    DELIVERED,
    CANCELLED
}
