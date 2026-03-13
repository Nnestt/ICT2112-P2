using System;
using System.Collections.Generic;

namespace ProRental.Domain.Entities;

public partial class Transaction
{
    public int Transactionid { get; set; }

    public int Orderid { get; set; }

    public string Type { get; set; } = null!;

    public string Purpose { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Status { get; set; } = null!;

    public string? Providertransactionid { get; set; }

    public DateTime Createdat { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Deposit> Deposits { get; set; } = new List<Deposit>();
}