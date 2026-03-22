using Microsoft.EntityFrameworkCore;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;
using ProRental.Interfaces.Domain;

namespace ProRental.Data.Services;

/// <summary>
/// Concrete implementation of ICustomerValidationService.
/// Uses raw SQL to bypass Customer.Customerid which is a private property
/// that cannot be accessed directly (entity file cannot be modified).
/// </summary>
public class CustomerValidationService : ICustomerValidationService
{
    private readonly AppDbContext _db;

    public CustomerValidationService(AppDbContext db)
    {
        _db = db;
    }

    public CustomerValidationResult ValidateCustomer(int customerId)
    {
        // Customer.Customerid is private — query via raw SQL to avoid CS0122.
        var matches = _db.Customers
            .FromSqlRaw(@"SELECT * FROM ""Customer"" WHERE ""customerId"" = {0}", customerId)
            .AsEnumerable()
            .ToList();

        if (matches.Count == 0)
            return CustomerValidationResult.Invalid(customerId,
                $"Customer ID {customerId} was not found. Please check and try again.");

        return CustomerValidationResult.Valid(customerId);
    }
}