using ProRental.Domain.Entities;
using ProRental.Interfaces.Data.Module3.P2_5;
using System.Collections.Generic;
using System.Linq;

namespace ProRental.Domain.Module3.P2_5.Controls
{
    public class CatalogControl
    {
        private readonly ICatalogGateway _catalogGateway;

        public CatalogControl(ICatalogGateway catalogGateway)
        {
            _catalogGateway = catalogGateway;
        }

        // FEATURE 5: Eco Product Discovery
        public List<Catalog> GetEcoProducts()
        {
            var products = _catalogGateway.GetAll();

            return products
                .Where(p => !string.IsNullOrEmpty(p.EcoBadge))
                .OrderBy(p => p.CarbonScore)
                .ToList();
        }

        public List<Catalog> GetByBadge(string badge)
        {
            if (string.IsNullOrWhiteSpace(badge))
            {
                return new List<Catalog>();
            }

            return _catalogGateway.GetByEcoBadge(badge.Trim());
        }

        public List<Catalog> GetSortedByCarbon()
        {
            return _catalogGateway.GetSortedByCarbonScore();
        }
    }
}
