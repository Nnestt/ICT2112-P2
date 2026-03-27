using ProRental.Domain.Module3.P2_5;

namespace ProRental.Interfaces.Module3.P2_5;

public interface IPackagingFootprintControl
{
    float CalculatePackagingFootprint(double volume, List<MaterialFootprintDto> materials);
}