namespace ProRental.Domain.Entities;

public partial class Cart
{
    public int GetCartId() => _cartid;
    public void SetCustomerId(int? customerId) => _customerid = customerId;
    public void SetSessionId(int? sessionId) => _sessionid = sessionId;
    public void SetRentalStart(DateTime? rentalStart) => _rentalstart = rentalStart;
    public void SetRentalEnd(DateTime? rentalEnd) => _rentalend = rentalEnd;
}
