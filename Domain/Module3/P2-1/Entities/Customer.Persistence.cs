namespace ProRental.Domain.Entities;

public partial class Customer
{
    public int GetCustomerId() => _customerid;
    public string GetAddress() => _address;
    public void SetUserId(int userId) => _userid = userId;
    public void SetAddress(string address) => _address = address;
    public void SetCustomerType(int customerType) => _customertype = customerType;
}
