using ProRental.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ProRental.Interfaces.Domain;

public interface IShippingOptionService
{
    List<ShippingOption> GetShippingOptions(string orderId);
    void ApplyCustomerSelection(string orderId, string optionId, 
        string preference);
    List<ShippingOption> BuildOptionSet(Order order);
    IActionResult SelectShippingOption(string orderId, string optionId);
    IActionResult CompareOptions(string orderId);
}