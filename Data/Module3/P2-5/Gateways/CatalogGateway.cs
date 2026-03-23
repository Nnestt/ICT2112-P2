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
                .OrderBy(item => item.CarbonScore)
                .ThenBy(item => item.Name)
                .ToList();
        }

        public List<Catalog> GetByEcoBadge(string badge)
        {
            return BuildCatalogItems()
                .Where(item => string.Equals(item.EcoBadge, badge, StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => item.CarbonScore)
                .ThenBy(item => item.Name)
                .ToList();
        }

        public List<Catalog> GetSortedByCarbonScore()
        {
            return GetAll();
        }

        private IEnumerable<Catalog> BuildCatalogItems()
        {
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

            foreach (var footprint in latestFootprints)
            {
                var productId = GetProductId(footprint);
                var badgeId = GetBadgeId(footprint);

                if (!detailsByProductId.TryGetValue(productId, out var detail))
                {
                    continue;
                }

                var carbonScore = Convert.ToDecimal(GetTotalCo2(footprint));
                var ecoBadge = badgeId.HasValue && badgesById.TryGetValue(badgeId.Value, out var badge)
                    ? ReadMember<string>(badge, "Badgename", "_badgename")
                    : "Standard";

                yield return new Catalog
                {
                    Id = productId,
                    Name = ReadMember<string>(detail, "Name", "_name"),
                    Description = ReadNullableMember<string>(detail, "Description", "_description") ?? string.Empty,
                    Price = ReadMember<decimal>(detail, "Price", "_price"),
                    EcoBadge = ecoBadge,
                    CarbonScore = carbonScore
                };
            }
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
