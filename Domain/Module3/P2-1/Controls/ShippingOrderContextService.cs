using Microsoft.EntityFrameworkCore;
using ProRental.Data.UnitOfWork;
using ProRental.Interfaces.Module3.P2_1;
using ProRental.Models.Module3.P2_1;

namespace ProRental.Domain.Controls;

/// <summary>
/// Temporary Module 1 adapter used by Feature 1 while the final checkout-side
/// integration contract is still being built. It exposes only the order inputs
/// needed to construct a shipping option set.
/// by: ernest
/// </summary>
public sealed class ShippingOrderContextService : IOrderService
{
    private readonly AppDbContext _context;

    public ShippingOrderContextService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrderShippingContext?> GetShippingContextAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await _context.Orders
            .Include(entity => entity.Customer)
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => EF.Property<int>(entity, "Orderid") == orderId, cancellationToken);

        if (order is null)
        {
            return null;
        }

        var destinationAddress = order.Customer.GetAddress();
        if (string.IsNullOrWhiteSpace(destinationAddress))
        {
            throw new InvalidOperationException($"Order '{orderId}' does not have a delivery address.");
        }

        var orderContext = order.GetOrderContext();

        return new OrderShippingContext(
            orderContext.OrderId,
            orderContext.CustomerId,
            orderContext.CheckoutId,
            destinationAddress,
            // Placeholder values: the final cross-module contract is expected to source
            // weight and quantity from Module 1 / Module 2 instead of hardcoding them here.
            WeightKg: 1d,
            Quantity: 1); //hardcoded as 1 for both
    }
}
