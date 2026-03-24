namespace ProRental.Domain.Module3.P2_5;

public class MaterialFootprintDto
{
    public string MaterialName { get; set; } = "";
    public int Quantity { get; set; }
    public string? Category { get; set; }
    public string? MaterialType { get; set; }
    public bool Recyclable { get; set; }
    public bool Reusable { get; set; }
}

public class PackagingFootprintProfileDto
{
    public int OrderId { get; set; }
    public string ProductName { get; set; } = "";
    public int ProfileId { get; set; }
    public string? FragilityLevel { get; set; }
    public double Volume { get; set; }
    public List<MaterialFootprintDto> Materials { get; set; } = new();
}