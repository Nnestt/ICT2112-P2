namespace ProRental.Domain.Entities
{
    public class Catalog
    {
        private readonly int _id;
        private readonly string _name;
        private readonly string _description;
        private readonly decimal _price;
        private readonly string _ecoBadge;
        private readonly decimal _carbonScore;

        public Catalog(int id, string name, string description, decimal price, string ecoBadge, decimal carbonScore)
        {
            _id = id;
            _name = name;
            _description = description;
            _price = price;
            _ecoBadge = ecoBadge;
            _carbonScore = carbonScore;
        }

        public int GetId()
        {
            return _id;
        }

        public string GetName()
        {
            return _name;
        }

        public string GetDescription()
        {
            return _description;
        }

        public decimal GetPrice()
        {
            return _price;
        }

        public string GetEcoBadge()
        {
            return _ecoBadge;
        }

        public decimal GetCarbonScore()
        {
            return _carbonScore;
        }

        public bool HasEcoBadge()
        {
            return !string.IsNullOrWhiteSpace(_ecoBadge);
        }

        public bool HasMatchingEcoBadge(string badge)
        {
            return string.Equals(_ecoBadge, badge, StringComparison.OrdinalIgnoreCase);
        }
    }
}   
