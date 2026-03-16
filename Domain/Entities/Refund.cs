using System;
using System.Collections.Generic;

namespace ProRental.Domain.Entities;

public partial class Refund
{
    public int Refundid { get; private set; }

    public int Orderid { get; private set; }

    public int Customerid { get; private set; }

    public int? Transactionid { get; private set; }

    public int Returnrequestid { get; private set; }

    public decimal Depositrefundamount { get; private set; }

    public DateTime Returndate { get; private set; }

    public decimal? Penaltyamount { get; private set; }

    public string Returnmethod { get; private set; } = null!;

    public virtual Customer Customer { get; private set; } = null!;

    public virtual Order Order { get; private set; } = null!;

    public virtual Returnrequest Returnrequest { get; private set; } = null!;

    public virtual Transaction? Transaction { get; private set; }
}
