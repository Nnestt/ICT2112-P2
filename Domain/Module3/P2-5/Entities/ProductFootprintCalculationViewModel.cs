using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProRental.Domain.Module3.P2_5.Entities;

public sealed class ProductFootprintCalculationViewModel
{
    [Required(ErrorMessage = "Please select a product.")]
    [Display(Name = "Product")]
    public int? ProductId { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Product mass must be 0 or greater.")]
    [Display(Name = "Product Mass")]
    public double? ProductMass { get; set; }

    [Required]
    [Range(0, 100, ErrorMessage = "Toxic percentage must be between 0 and 100.")]
    [Display(Name = "Toxic Percentage")]
    public double? ToxicPercentage { get; set; }

    public double? CarbonFootprint { get; set; }

    public DateTime? CalculatedAt { get; set; }

    public string? StatusMessage { get; set; }

    public List<SelectListItem> ProductOptions { get; set; } = [];

    public List<ProductFootprintListItem> SavedFootprints { get; set; } = [];
}
