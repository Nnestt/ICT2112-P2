using ProRental.Domain.Module3.P2_5;

namespace ProRental.Interfaces.Module3.P2_5;

public interface IPackagingFootprintControl
{
    float CalculatePackagingFootprint(List<MaterialFootprintDto> materials);
}