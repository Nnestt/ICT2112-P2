using Microsoft.EntityFrameworkCore;
using ProRental.Data.UnitOfWork;
using ProRental.Interfaces.Domain;
using ProRental.Models.Module3.P2_1;

namespace ProRental.Domain.Controls;

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

        return new OrderShippingContext(
            order.GetOrderId(),
            order.GetCustomerId(),
            order.GetCheckoutId(),
            destinationAddress,
            WeightKg: 1d,
            Quantity: 1);
    }
}
