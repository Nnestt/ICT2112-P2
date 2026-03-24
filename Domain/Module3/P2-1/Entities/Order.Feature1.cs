namespace ProRental.Domain.Entities;

/// <summary>
/// Feature 1 extension methods for the shared Order entity. These grouped accessors let
/// checkout-related flows read order state without exposing each field independently.
/// by: ernest
/// </summary>
public partial class Order
{
    public void InitializeForCheckout(
        int customerId,
        int checkoutId,
        DateTime orderDate,
        decimal totalAmount,
        int? transactionId = null)
    {
        _customerid = customerId;
        _checkoutid = checkoutId;
        _orderdate = orderDate;
        _totalamount = totalAmount;
        _transactionid = transactionId;
    }

    public (int OrderId, int CustomerId, int CheckoutId) GetOrderContext()
    {
        return (_orderid, _customerid, _checkoutid);
    }

    public (int OrderId, int CustomerId, int CheckoutId, int? TransactionId, DateTime OrderDate, decimal TotalAmount) GetOrderSnapshot()
    {
        return (_orderid, _customerid, _checkoutid, _transactionid, _orderdate, _totalamount);
    }
}
