namespace ProRental.Domain.Entities;

public partial class Customerreward
{
    // Use a static factory method instead of a constructor
    // so EF Core doesn't try to use it for hydration
    public static Customerreward Create(int customerId, int orderCarbonDataId,
        string rewardType, double rewardValue, DateTime createdAt)
    {
        var r = new Customerreward();
        r.Customerid        = customerId;
        r.Ordercarbondataid = orderCarbonDataId;
        r.Rewardtype        = rewardType;
        r.Rewardvalue       = rewardValue;
        r.Createdat         = createdAt;
        return r;
    }

    public int      GetRewardid()          => Rewardid;
    public int      GetCustomerid()        => Customerid;
    public int      GetOrdercarbondataid() => Ordercarbondataid;
    public string   GetRewardtype()        => Rewardtype;
    public double   GetRewardvalue()       => Rewardvalue;
    public DateTime GetCreatedat()         => Createdat;

    public string GetFormattedValue()
        => $"${Rewardvalue:F0} {Rewardtype}";
}