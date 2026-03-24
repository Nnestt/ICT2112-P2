using Microsoft.EntityFrameworkCore;
using ProRental.Data.Module3.P2_1.Interfaces;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;
using ProRental.Domain.Enums;

namespace ProRental.Data.Module3.P2_1.Gateways;

public class PricingRuleGateway : IPricingRuleGateway
{
    private readonly AppDbContext _context;

    public PricingRuleGateway(AppDbContext context)
    {
        _context = context;
    }

    public List<PricingRule> FindActiveRules()
    {
        return _context.PricingRules
            .Where(r => EF.Property<bool?>(r, "IsActive") == true)
            .ToList();
    }

    public List<PricingRule> FindByTransportMode(TransportMode mode)
    {
        return _context.PricingRules
            .Where(r => EF.Property<TransportMode?>(r, "TransportMode") == mode)
            .ToList();
    }

    public void Save(PricingRule rule)
    {
        _context.PricingRules.Add(rule);
        _context.SaveChanges();
    }

    public void Update(PricingRule rule)
    {
        _context.PricingRules.Update(rule);
        _context.SaveChanges();
    }
}
