namespace ProRental.Domain.Entities;

public partial class Orderitem
{
    // ── Public accessors ─────────────────────────────────────────────────
    public int OrderItemId          => Orderitemid;
    public int OrderId              => Orderid;
    public int ProductId            => Productid;
    public int OrderedQuantity      => _quantity;    // Quantity is taken by scaffolded private
    public decimal UnitPrice        => Unitprice;
    public DateTime? RentalStart    => Rentalstartdate;
    public DateTime? RentalEnd      => Rentalenddate;

    // ── Factory method ───────────────────────────────────────────────────
    public static Orderitem Create(int productId, int quantity, decimal unitPrice,
                                   DateTime? rentalStart, DateTime? rentalEnd)
    {
        var item = new Orderitem();
        typeof(Orderitem).GetProperty("Productid",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(item, productId);
        typeof(Orderitem).GetProperty("Quantity",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(item, quantity);
        typeof(Orderitem).GetProperty("Unitprice",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(item, unitPrice);
        typeof(Orderitem).GetProperty("Rentalstartdate",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(item, rentalStart);
        typeof(Orderitem).GetProperty("Rentalenddate",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(item, rentalEnd);
        return item;
    }
}