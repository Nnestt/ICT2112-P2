namespace ProRental.Domain.Entities;
public class CostSummary
{
    public decimal RentalCost { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal DeliveryCost { get; set; }
    public decimal FinalOrderCost { get; set; }
    public List<UnobtainableItemInfo> UnobtainableItems { get; set; } = [];
}