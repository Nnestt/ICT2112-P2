using ProRental.Data.Module1.Interfaces;
using ProRental.Data.Module3.P2_5.Interfaces;
using ProRental.Domain.Entities;
using ProRental.Interfaces.Module3.P2_5;

namespace ProRental.Domain.Module3.P2_5.Controls;

public class PackagingProfilerControl : IPackagingProfilerControl
{
    private readonly IOrderService _orderService;
    private readonly IPackagingProfileGateway _profileGateway;
    private readonly IPackagingConfigurationGateway _configGateway;
    private readonly IPackagingMaterialGateway _materialGateway;
    private readonly IPackagingFootprintControl _footprintControl;

    public PackagingProfilerControl(
        IOrderService orderService,
        IPackagingProfileGateway profileGateway,
        IPackagingConfigurationGateway configGateway,
        IPackagingMaterialGateway materialGateway,
        IPackagingFootprintControl footprintControl)
    {
        _orderService = orderService;
        _profileGateway = profileGateway;
        _configGateway = configGateway;
        _materialGateway = materialGateway;
        _footprintControl = footprintControl;
    }
    
    public Packagingprofile CreatePackagingProfile(int orderId, float volume, string fragilityLevel)
    {
        if (orderId <= 0)
            throw new ArgumentOutOfRangeException(nameof(orderId), "Error: Invalid Order ID.");

        if (string.IsNullOrWhiteSpace(fragilityLevel))
            fragilityLevel = "low";
        
        var existingProfile = _profileGateway.FindByOrderId(orderId);
        if (existingProfile != null)
            return existingProfile;

        _profileGateway.Save(orderId, volume, fragilityLevel);

        return _profileGateway.FindByOrderId(orderId);
    }

    public Packagingconfiguration CreatePackagingConfiguration(Packagingprofile profile)
    {
        if (profile == null) throw new ArgumentNullException(nameof(profile));

        var profileId = _profileGateway.GetProfileId(profile);
        if (profileId <= 0)
            throw new InvalidOperationException("Invalid packaging profile ID. Save profile before creating configuration.");
        
        _configGateway.SaveConfiguration(profileId);

        var profileFromDb = _profileGateway.FindByProfileId(profileId);
        if (profileFromDb == null)
            throw new InvalidOperationException("Profile was not found after configuration creation.");

        var config = profileFromDb.Packagingconfigurations.FirstOrDefault();
        if (config == null)
            throw new InvalidOperationException("Failed to create packaging configuration.");

        var fragilityLevel = _profileGateway.GetFragilityLevel(profile);
        var volume = _profileGateway.GetVolume(profile);

        var allMaterials = _materialGateway.FindAll();
        var primaryMaterial = profileFromDb.DeterminePrimaryPackaging(fragilityLevel, allMaterials);
        var secondaryMaterials = profileFromDb.DetermineSecondaryPackaging(volume, allMaterials);

        var configId = _configGateway.GetConfigurationId(config);

        if (primaryMaterial != null)
            _configGateway.SaveMaterials(configId, new List<Packagingmaterial> { primaryMaterial }, "primary", 1);

        if (secondaryMaterials.Any())
            _configGateway.SaveMaterials(configId, secondaryMaterials, "secondary", 1);

        return _configGateway.FindByConfigurationId(configId);
    }

    public List<dynamic> GetAllPackagingFootprints()
    {
        try
        {
            var profiles = _profileGateway.GetAllFootprintProfiles();
            var results = new List<dynamic>();

            foreach (var profile in profiles)
            {
                if (!profile.Materials.Any()) continue;

                foreach (var material in profile.Materials)
                {
                    var footprintg = _footprintControl.CalculatePackagingFootprint(profile.Volume, new List<MaterialFootprintDto> { material });

                    results.Add((dynamic)new
                    {
                        OrderId = profile.OrderId,
                        ProductName = profile.ProductName,
                        ProfileId = profile.ProfileId,
                        FragilityLevel = profile.FragilityLevel ?? "low",
                        Volume = profile.Volume,
                        
                        Category = material.Category,
                        MaterialName = material.MaterialName,
                        MaterialType = material.MaterialType,
                        Quantity = material.Quantity,
                        Recyclable = material.Recyclable,
                        Reusable = material.Reusable,

                        Footprintg = footprintg
                    });
                }
            }

            return results;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(
                $"[PackagingProfilerControl] Error retrieving footprints: {ex.Message}");
            return new List<dynamic>();
        }
    }
}