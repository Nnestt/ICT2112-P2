using Microsoft.AspNetCore.Mvc;
using ProRental.Domain.Module3.P2_5.Controls;
using ProRental.Interfaces.Data.Module3.P2_5;

namespace ProRental.Controllers.Module3.P2_5
{
    [ApiController]
    [Route("api/catalog")]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogControl _control;

        public CatalogController(ICatalogGateway catalogGateway)
        {
            _control = new CatalogControl(catalogGateway);
        }

        //Feature 5: Eco products
        [HttpGet("eco")]
        public IActionResult GetEcoProducts()
        {
            var result = _control.GetEcoProducts()
                .Select(MapCatalog)
                .ToList();
            return Ok(result);
        }

        // filter by badge
        [HttpGet("badge/{badge}")]
        public IActionResult GetByBadge(string badge)
        {
            var result = _control.GetByBadge(badge)
                .Select(MapCatalog)
                .ToList();
            return Ok(result);
        }

        // sorted by carbon score
        [HttpGet("sorted")]
        public IActionResult GetSorted()
        {
            var result = _control.GetSortedByCarbon()
                .Select(MapCatalog)
                .ToList();
            return Ok(result);
        }

        private static object MapCatalog(ProRental.Domain.Entities.Catalog catalog)
        {
            return new
            {
                Id = catalog.GetId(),
                Name = catalog.GetName(),
                Description = catalog.GetDescription(),
                Price = catalog.GetPrice(),
                EcoBadge = catalog.GetEcoBadge(),
                CarbonScore = catalog.GetCarbonScore()
            };
        }
    }
}
