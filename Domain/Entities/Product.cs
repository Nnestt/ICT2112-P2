using System;
using System.Collections.Generic;

namespace ProRental.Domain.Entities;

public partial class Product
{
    private int _productid;
    private int Productid { get => _productid; set => _productid = value; }

    private int _categoryid;
    private int Categoryid { get => _categoryid; set => _categoryid = value; }

    private string _sku = null!;
    private string Sku { get => _sku; set => _sku = value; }

    private decimal _threshold;
    private decimal Threshold { get => _threshold; set => _threshold = value; }

    private DateTime _createdat;
    private DateTime Createdat { get => _createdat; set => _createdat = value; }

    private DateTime _updatedat;
    private DateTime Updatedat { get => _updatedat; set => _updatedat = value; }

    public virtual ICollection<Alert> Alerts { get; private set; } = new List<Alert>();

    public virtual ICollection<Cartitem> Cartitems { get; private set; } = new List<Cartitem>();

    public virtual Category Category { get; private set; } = null!;

    public virtual ICollection<Inventoryitem> Inventoryitems { get; private set; } = new List<Inventoryitem>();

    public virtual ICollection<Lineitem> Lineitems { get; private set; } = new List<Lineitem>();

    public virtual ICollection<Orderitem> Orderitems { get; private set; } = new List<Orderitem>();

    public virtual Productdetail? Productdetail { get; private set; }

    public virtual Productfootprint? Productfootprint { get; private set; }
}
