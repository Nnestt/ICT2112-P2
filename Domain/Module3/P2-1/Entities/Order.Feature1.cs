namespace ProRental.Domain.Entities;

public partial class Order
{
    public int GetOrderId() => _orderid;
    public int GetCustomerId() => _customerid;
    public int GetCheckoutId() => _checkoutid;
    public int? GetTransactionId() => _transactionid;
    public DateTime GetOrderDate() => _orderdate;
    public decimal GetTotalAmount() => _totalamount;

    public void SetCustomerId(int customerId) => _customerid = customerId;
    public void SetCheckoutId(int checkoutId) => _checkoutid = checkoutId;
    public void SetTransactionId(int? transactionId) => _transactionid = transactionId;
    public void SetOrderDate(DateTime orderDate) => _orderdate = orderDate;
    public void SetTotalAmount(decimal totalAmount) => _totalamount = totalAmount;
}
