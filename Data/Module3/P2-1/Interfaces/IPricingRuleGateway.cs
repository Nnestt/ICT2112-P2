using ProRental.Domain.Entities;
using ProRental.Domain.Enums;

namespace ProRental.Data.Module3.P2_1.Interfaces;

public interface IPricingRuleGateway
{
    List<PricingRule> FindActiveRules();
    List<PricingRule>    FindByTransportMode(TransportMode mode);
    void Save(PricingRule rule);
    void Update(PricingRule rule);
}
