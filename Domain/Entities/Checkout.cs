using System;
using System.Collections.Generic;

namespace ProRental.Domain.Entities;

public partial class Checkout
{
    public int Checkoutid { get; private set; }

    public int Customerid { get; private set; }

    public int Cartid { get; private set; }

    public int? Deliveryid { get; private set; }

    public bool? Notifyoptin { get; private set; }

    public DateTime Createdat { get; private set; }

    public virtual Cart Cart { get; private set; } = null!;

    public virtual Customer Customer { get; private set; } = null!;

    public virtual Deliverymethod? Delivery { get; private set; }

    public virtual ICollection<Order> Orders { get; private set; } = new List<Order>();
}
