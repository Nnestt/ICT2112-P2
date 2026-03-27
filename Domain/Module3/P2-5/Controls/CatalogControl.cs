using ProRental.Domain.Entities;
using ProRental.Interfaces.Data.Module3.P2_5;
using System.Collections.Generic;
using System.Linq;

namespace ProRental.Domain.Module3.P2_5.Controls
{
    public class CatalogControl
    {
        private readonly ICatalogGateway _catalogGateway;
        private readonly EcoBadgeControl _ecoBadgeControl;

        public CatalogControl(ICatalogGateway catalogGateway)
        {
            _catalogGateway = catalogGateway;
            _ecoBadgeControl = new EcoBadgeControl();
        }

        // FEATURE 5: Eco Product Discovery
        public List<Catalog> GetEcoProducts()
        {
            var products = _catalogGateway.GetAll();

            return products
                .Where(product => _ecoBadgeControl.IsEcoFriendly(product.GetCarbonScore()))
                .OrderBy(product => product.GetCarbonScore())
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
