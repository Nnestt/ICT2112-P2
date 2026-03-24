using Microsoft.EntityFrameworkCore;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;
using ProRental.Interfaces.Module3.P2_1;

namespace ProRental.Data.Module3.P2_1;

/// <summary>
/// Feature 1 persistence gateway for shipping-option reads and writes. It wraps the
/// specific queries needed by checkout generation and customer selection.
/// by: ernest
/// </summary>
public sealed class ShippingOptionMapper : IShippingOptionMapper
{
    private readonly AppDbContext _context;

    public ShippingOptionMapper(AppDbContext context)
    {
        _context = context;
    }

    public Task<Order?> FindOrderWithCheckoutAsync(int orderId, CancellationToken cancellationToken = default)
    {
        return _context.Orders
            .Include(order => order.Checkout)
            .AsNoTracking()
            .FirstOrDefaultAsync(order => EF.Property<int>(order, "Orderid") == orderId, cancellationToken);
    }

    public async Task<IReadOnlyList<ShippingOption>> FindByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        // Options are returned in insertion order so checkout presents a stable FAST/CHEAP/GREEN set.
        var options = await _context.ShippingOptions
            .Include(option => option.Route)
            .AsNoTracking()
            .Where(option => EF.Property<int?>(option, "OrderId") == orderId)
            .OrderBy(option => EF.Property<int?>(option, "OptionId"))
            .ToListAsync(cancellationToken);

        return options;
    }

    public Task<ShippingOption?> FindByIdAsync(int optionId, CancellationToken cancellationToken = default)
    {
        return _context.ShippingOptions
            .Include(option => option.Route)
            .FirstOrDefaultAsync(option => EF.Property<int>(option, "OptionId") == optionId, cancellationToken);
    }

    public async Task AddAsync(ShippingOption option, CancellationToken cancellationToken = default)
    {
        await _context.ShippingOptions.AddAsync(option, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<ShippingOption> options, CancellationToken cancellationToken = default)
    {
        await _context.ShippingOptions.AddRangeAsync(options, cancellationToken);
    }

    public Task UpdateAsync(ShippingOption option, CancellationToken cancellationToken = default)
    {
        _context.ShippingOptions.Update(option);
        return Task.CompletedTask;
    }

    public async Task SetCheckoutSelectedOptionAsync(int checkoutId, int optionId, CancellationToken cancellationToken = default)
    {
        // Selection is written to checkout because Module 1 treats checkout state as the
        // persisted integration point for the customer-confirmed shipping choice.
        var checkout = await _context.Checkouts
            .FirstOrDefaultAsync(item => EF.Property<int>(item, "Checkoutid") == checkoutId, cancellationToken)
            ?? throw new InvalidOperationException($"Checkout '{checkoutId}' was not found.");

        var shippingOption = await _context.ShippingOptions
            .AsNoTracking()
            .FirstOrDefaultAsync(option => EF.Property<int>(option, "OptionId") == optionId, cancellationToken)
            ?? throw new InvalidOperationException($"Shipping option '{optionId}' was not found.");

        var order = await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(item => EF.Property<int>(item, "Checkoutid") == checkoutId, cancellationToken)
            ?? throw new InvalidOperationException($"Order for checkout '{checkoutId}' was not found.");

        var orderId = order.GetOrderContext().OrderId;

        if (!shippingOption.BelongsToOrder(orderId))
        {
            throw new InvalidOperationException(
                $"Shipping option '{optionId}' does not belong to order '{orderId}'.");
        }

        checkout.SelectShippingOption(optionId);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
