using System;
using System.Collections.Generic;

namespace ProRental.Domain.Entities;

public partial class Deliverymethod
{
    public int Deliveryid { get; private set; }

    public int Orderid { get; private set; }

    public int Durationdays { get; private set; }

    public decimal Deliverycost { get; private set; }

    public string Carrierid { get; private set; } = null!;

    public virtual ICollection<Checkout> Checkouts { get; private set; } = new List<Checkout>();

    public virtual Order Order { get; private set; } = null!;
}
