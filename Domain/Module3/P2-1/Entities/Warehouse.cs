namespace ProRental.Domain.Entities;

public partial class Warehouse
{
    // --- Public getters for private scaffolded fields ---
    public int GetHubId() => _hubId;
    public string GetWarehouseCode() => _warehouseCode;
    public int? GetMaxProductCapacity() => _maxProductCapacity;
    public double? GetTotalWarehouseVolume() => _totalWarehouseVolume;
    public double? GetClimateControlEmissionRate() => _climateControlEmissionRate;
    public double? GetLightingEmissionRate() => _lightingEmissionRate;
    public double? GetSecuritySystemEmissionRate() => _securitySystemEmissionRate;

    // --- Public setters for private scaffolded fields ---
    public void SetWarehouseCode(string warehouseCode) => _warehouseCode = warehouseCode;
    public void SetTotalWarehouseVolume(double? totalWarehouseVolume) => _totalWarehouseVolume = totalWarehouseVolume;
    public void SetMaxProductCapacity(int? maxProductCapacity) => _maxProductCapacity = maxProductCapacity;

}
