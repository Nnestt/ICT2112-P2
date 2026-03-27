namespace ProRental.Domain.Entities;

public partial class Ordercarbondatum
{
    public static Ordercarbondatum Create(int orderId, double productCarbon, double packagingCarbon,
        double staffCarbon, double buildingCarbon, double totalCarbon,
        string impactLevel, DateTime calculatedAt)
    {
        var d = new Ordercarbondatum();
        d.Orderid         = orderId;
        d.Productcarbon   = productCarbon;
        d.Packagingcarbon = packagingCarbon;
        d.Staffcarbon     = staffCarbon;
        d.Buildingcarbon  = buildingCarbon;
        d.Totalcarbon     = totalCarbon;
        d.Impactlevel     = impactLevel;
        d.Calculatedat    = calculatedAt;
        return d;
    }

    public int      GetOrdercarbondataid() => Ordercarbondataid;
    public int      GetOrderid()           => Orderid;
    public double   GetProductcarbon()     => Productcarbon;
    public double   GetPackagingcarbon()   => Packagingcarbon;
    public double   GetStaffcarbon()       => Staffcarbon;
    public double   GetBuildingcarbon()    => Buildingcarbon;
    public double   GetTotalcarbon()       => Totalcarbon;
    public string?   GetImpactlevel()       => Impactlevel;
    public DateTime GetCalculatedat()      => Calculatedat;

    public string GetImpactBadgeColour() => Impactlevel switch
    {
        "Low"      => "success",
        "Moderate" => "warning",
        "High"     => "danger",
        _          => "dark"
    };
}