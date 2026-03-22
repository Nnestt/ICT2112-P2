namespace ProRental.Domain.Entities;

public partial class Productdetail
{
    public decimal GetPrice() => _price;
    public void SetPrice(decimal price) => _price = price;
}