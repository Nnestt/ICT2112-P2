using ProRental.Domain.Entities;

namespace ProRental.Interfaces.Domain;

public interface ICostCalculation
{
    CostSummary CalculateRentalCost(List<SelectedItem> items, int rentalPeriod);
    CostSummary CalculateFinalOrderCost(CostSummary summary, string orderId);

//    List<CartItemCost> CalculateCartItemCosts(List<CartItem> items);
    decimal CalculateDepositAmount(decimal rentalCost);
}