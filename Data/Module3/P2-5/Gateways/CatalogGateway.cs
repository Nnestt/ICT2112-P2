using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;
using ProRental.Interfaces.Data.Module3.P2_5;
using System.Reflection;

namespace ProRental.Data.Module3.P2_5.Gateways
{
    public class CatalogGateway : ICatalogGateway
    {
        private readonly AppDbContext _dbContext;

        public CatalogGateway(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Catalog> GetAll()
        {
            return BuildCatalogItems()
                .OrderBy(item => item.GetCarbonScore())
                .ThenBy(item => item.GetName())
                .ToList();
        }

        public List<Catalog> GetByEcoBadge(string badge)
        {
            return BuildCatalogItems()
                .Where(item => item.HasMatchingEcoBadge(badge))
                .OrderBy(item => item.GetCarbonScore())
                .ThenBy(item => item.GetName())
                .ToList();
        }

        public List<Catalog> GetSortedByCarbonScore()
        {
            return GetAll();
        }

        private IEnumerable<Catalog> BuildCatalogItems()
        {
            var products = _dbContext.Products
                .AsEnumerable()
                .OrderBy(GetSku)
                .ToList();

            var latestFootprints = _dbContext.Productfootprints
                .AsEnumerable()
                .GroupBy(GetProductId)
                .Select(group => group
                    .OrderByDescending(GetCalculatedAt)
                    .First())
                .ToList();

            var detailsByProductId = _dbContext.Productdetails
                .AsEnumerable()
                .ToDictionary(GetProductId);

            var badgesById = _dbContext.Ecobadges
                .AsEnumerable()
                .ToDictionary(GetBadgeId);

            var footprintsByProductId = latestFootprints
                .ToDictionary(GetProductId);

            foreach (var product in products)
            {
                var productId = GetProductId(product);
                detailsByProductId.TryGetValue(productId, out var detail);
                footprintsByProductId.TryGetValue(productId, out var footprint);

                var badgeId = footprint is null ? null : GetBadgeId(footprint);
                var carbonScore = footprint is null
                    ? 999m
                    : Convert.ToDecimal(GetTotalCo2(footprint));
                var ecoBadge = badgeId.HasValue && badgesById.TryGetValue(badgeId.Value, out var badge)
                    ? ReadMember<string>(badge, "Badgename", "_badgename")
                    : "Standard";

                yield return new Catalog(
                    productId,
                    GetSku(product),
                    detail is null ? string.Empty : ReadNullableMember<string>(detail, "Description", "_description") ?? string.Empty,
                    detail is null ? 0m : ReadMember<decimal>(detail, "Price", "_price"),
                    ecoBadge,
                    carbonScore);
            }
        }

        private static int GetProductId(Product product)
        {
            return ReadMember<int>(product, "Productid", "_productid");
        }

        private static string GetSku(Product product)
        {
            return ReadMember<string>(product, "Sku", "_sku");
        }

        private static int GetProductId(Productfootprint footprint)
        {
            return ReadMember<int>(footprint, "Productid", "_productid");
        }

        private static int GetProductId(Productdetail detail)
        {
            return ReadMember<int>(detail, "Productid", "_productid");
        }

        private static int? GetBadgeId(Productfootprint footprint)
        {
            return ReadNullableMember<int>(footprint, "Badgeid", "_badgeid");
        }

        private static int GetBadgeId(Ecobadge badge)
        {
            return ReadMember<int>(badge, "Badgeid", "_badgeid");
        }

        private static double GetTotalCo2(Productfootprint footprint)
        {
            return ReadMember<double>(footprint, "Totalco2", "_totalco2");
        }

        private static DateTime GetCalculatedAt(Productfootprint footprint)
        {
            return ReadMember<DateTime>(footprint, "Calculatedat", "_calculatedat");
        }

        private static T ReadMember<T>(object source, string propertyName, string fieldName)
        {
            var type = source.GetType();

            var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property?.GetValue(source) is T propertyValue)
            {
                return propertyValue;
            }

            var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field?.GetValue(source) is T fieldValue)
            {
                return fieldValue;
            }

            throw new InvalidOperationException($"Unable to read '{propertyName}' from {type.Name}.");
        }

        private static T? ReadNullableMember<T>(object source, string propertyName, string fieldName)
        {
            var type = source.GetType();

            var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property != null)
            {
                return (T?)property.GetValue(source);
            }

            var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                return (T?)field.GetValue(source);
            }

            throw new InvalidOperationException($"Unable to read '{propertyName}' from {type.Name}.");
        }
    }
}
