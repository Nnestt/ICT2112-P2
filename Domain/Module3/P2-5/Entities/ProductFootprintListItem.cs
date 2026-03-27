namespace ProRental.Domain.Module3.P2_5.Entities;

public sealed record ProductFootprintListItem(
    int ProductCarbonFootprintId,
    int ProductId,
    string ProductName,
    string BadgeName,
    double? ToxicPercentage,
    double TotalCo2,
    DateTime CalculatedAt);
