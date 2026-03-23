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
            var result = _control.GetEcoProducts();
            return Ok(result);
        }

        // filter by badge
        [HttpGet("badge/{badge}")]
        public IActionResult GetByBadge(string badge)
        {
            var result = _control.GetByBadge(badge);
            return Ok(result);
        }

        // sorted by carbon score
        [HttpGet("sorted")]
        public IActionResult GetSorted()
        {
            var result = _control.GetSortedByCarbon();
            return Ok(result);
        }
    }
}
