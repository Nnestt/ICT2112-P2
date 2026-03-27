// Interfaces/Module3/IProductFootprintService.cs
// Stub interface declared by Feature 4 as per the Module 3 class diagram.
// The concrete implementation lives in Feature 1's codebase.
// Feature 4 only declares this interface so that RewardsControl can receive
// product carbon values from any caller that resolves this service.

namespace ProRental.Interfaces.Domain;

public interface IProductFootprintService
{
    /// <summary>
    /// Returns the pre-calculated product carbon footprint (grams CO₂e)
    /// for the given order. Returns 0 if not yet available.
    /// </summary>
    double CalculateProductFootprint(int orderId);
}
