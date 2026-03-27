namespace ProRental.Domain.Entities;

public partial class Order
{
    public int      GetOrderid()    => Orderid;
    public int      GetCustomerid() => Customerid;
    public DateTime GetOrderdate()  => Orderdate;
}