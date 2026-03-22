using ProRental.Domain.Entities;
using ProRental.Interfaces.Domain;

namespace ProRental.Domain.Controls;

public class CostCalculationControl : ICostCalculation
{
    private readonly IInventoryService _inventoryService;
    private readonly IShippingOptionService _shippingOptionService;

    public CostCalculationControl(
        IInventoryService inventoryService,
        IShippingOptionService shippingOptionService)
    {
        _inventoryService = inventoryService;
        _shippingOptionService = shippingOptionService;
    }

    // ========================
    // 1. CalculateRentalCost
    // ========================
    public CostSummary CalculateRentalCost(
        List<SelectedItem> items, int rentalPeriod)
    {
        decimal rentalCost = 0;

        foreach (var item in items)
        {
            var product = item.GetProduct();
            var price = product.ProductDetail?.GetPrice() ?? 0;
            var quantity = item.GetQuantity();
            rentalCost += price * quantity * rentalPeriod;
        }

        decimal deposit = CalculateDepositAmount(rentalCost);

        return new CostSummary
        {
            RentalCost = rentalCost,
            DepositAmount = deposit
        };
    }

    // ============================
    // 2. CalculateFinalOrderCost
    // ============================
    public CostSummary CalculateFinalOrderCost(
        CostSummary summary, string orderId)
    {
        var shippingOptions =
            _shippingOptionService.GetShippingOptions(orderId);
        var shippingOption = shippingOptions.FirstOrDefault();
        decimal shippingCost = shippingOption?.GetCost() ?? 0;

        summary.DeliveryCost = shippingCost;
        summary.FinalOrderCost = summary.RentalCost +
                                 summary.DepositAmount +
                                 shippingCost;
        return summary;
    }

    // ===========================
    // 3. CalculateCartItemCosts
    // (commented out — waiting for Cart team)
    // ===========================
    // public List<CartItemCost> CalculateCartItemCosts(List<CartItem> items)
    // {
    //     var result = new List<CartItemCost>();
    //     foreach (var item in items)
    //     {
    //         var product = item.GetProduct();
    //         var price = product.ProductDetail?.GetPrice() ?? 0;
    //         var quantity = item.GetQuantity();
    //         result.Add(new CartItemCost(item, price * quantity));
    //     }
    //     return result;
    // }

    // ===========================
    // 4. CalculateDepositAmount
    // ===========================
    public decimal CalculateDepositAmount(decimal rentalCost)
    {
        return rentalCost * 0.2m;
    }
}