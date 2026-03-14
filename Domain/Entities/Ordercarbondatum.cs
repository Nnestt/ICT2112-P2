using System;
using System.Collections.Generic;

namespace ProRental.Domain.Entities;

public partial class Ordercarbondatum
{
    public int Ordercarbondataid { get; private set; }

    public int Orderid { get; private set; }

    public double Productcarbon { get; private set; }

    public double Packagingcarbon { get; private set; }

    public double Staffcarbon { get; private set; }

    public double Buildingcarbon { get; private set; }

    public double Totalcarbon { get; private set; }

    public string? Impactlevel { get; private set; }

    public DateTime Calculatedat { get; private set; }

    public virtual ICollection<Customerreward> Customerrewards { get; private set; } = new List<Customerreward>();

    public virtual Order Order { get; private set; } = null!;
}
