using System;
using System.Collections.Generic;

namespace ProRental.Domain.Entities;

public partial class Order
{
    public int Orderid { get; set; }
    public int Customerid { get; set; }
    public int Checkoutid { get; set; }
    public int? Transactionid { get; set; }         
    public DateTime Orderdate { get; set; }
    public string Status { get; set; } = null!;
    public string Deliverytype { get; set; } = null!;
    public decimal Totalamount { get; set; }
 
    public virtual Checkout Checkout { get; set; } = null!;
    public virtual Customer Customer { get; set; } = null!;
    public virtual Transaction? Transaction { get; set; }
    public virtual ICollection<Orderitem> Orderitems { get; set; } = new List<Orderitem>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual ICollection<Deposit> Deposits { get; set; } = new List<Deposit>();
    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();
    public virtual ICollection<Returnrequest> Returnrequests { get; set; } = new List<Returnrequest>();
}
