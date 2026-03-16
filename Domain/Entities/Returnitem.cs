using System;
using System.Collections.Generic;
using ProRental.Domain.Enums;

namespace ProRental.Domain.Entities;

public partial class Returnitem
{
    public int Returnitemid { get; private set; }

    public int Returnrequestid { get; private set; }

    public int Inventoryitemid { get; private set; }

    public ReturnItemStatus Status { get; private set; }

    public DateTime? Completiondate { get; private set; }

    public string? Image { get; private set; }

    public virtual ICollection<Damagereport> Damagereports { get; private set; } = new List<Damagereport>();

    public virtual Inventoryitem Inventoryitem { get; private set; } = null!;

    public virtual Returnrequest Returnrequest { get; private set; } = null!;
}
