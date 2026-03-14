using System;
using System.Collections.Generic;

namespace ProRental.Domain.Entities;

public partial class Customerreward
{
    public int Rewardid { get; private set; }

    public int Customerid { get; private set; }

    public int Ordercarbondataid { get; private set; }

    public string Rewardtype { get; private set; } = null!;

    public double Rewardvalue { get; private set; }

    public DateTime Createdat { get; private set; }

    public virtual Customer Customer { get; private set; } = null!;

    public virtual Ordercarbondatum Ordercarbondata { get; private set; } = null!;
}
