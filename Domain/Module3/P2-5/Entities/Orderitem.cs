namespace ProRental.Domain.Entities;

public partial class Orderitem
{
    public int GetOrderid()   => Orderid;
    public int GetProductid() => Productid;
    public int GetQuantity()  => Quantity;
}