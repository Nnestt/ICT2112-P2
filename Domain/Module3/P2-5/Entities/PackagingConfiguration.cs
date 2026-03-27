namespace ProRental.Domain.Entities;

public partial class Packagingconfiguration
{
    private Dictionary<Packagingmaterial, int> _primaryMaterials = new();
    private Dictionary<Packagingmaterial, int> _secondaryMaterials = new();
    
    public void AddMaterial(string category, Packagingmaterial material, int quantity)
    {
        if (category.Equals("primary", StringComparison.OrdinalIgnoreCase))
            _primaryMaterials[material] = quantity;
        else
            _secondaryMaterials[material] = quantity;
    }
}
