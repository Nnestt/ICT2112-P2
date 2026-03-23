using Microsoft.EntityFrameworkCore;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;
using ProRental.Domain.Enums;
using ProRental.Interfaces.Domain;

namespace ProRental.Data.Services;

/// <summary>
/// Concrete implementation of IAuthenticationService.
/// NOTE: named ProRentalAuthenticationService to avoid ambiguity with
/// Microsoft.AspNetCore.Authentication.IAuthenticationService.
/// </summary>
public class ProRentalAuthenticationService : IAuthenticationService
{
    private readonly AppDbContext _db;
    private readonly ISessionService _sessionService;

    public ProRentalAuthenticationService(AppDbContext db, ISessionService sessionService)
    {
        _db = db;
        _sessionService = sessionService;
    }

    public AuthResult Authenticate(string email, string password)
    {
        var user = _db.Users
            .AsEnumerable()
            .FirstOrDefault(u =>
            {
                var userEmail = GetPrivateProperty<string>(u, "Email");
                return string.Equals(userEmail, email, StringComparison.OrdinalIgnoreCase);
            });

        if (user == null)
            return AuthResult.Failure("Invalid email or password.");

        var passwordHash = GetPrivateProperty<string>(user, "Passwordhash");

        if (passwordHash == null)
            return AuthResult.Failure("User account data is incomplete.");

        if (password != passwordHash)
            return AuthResult.Failure("Invalid email or password.");

        // Read role directly as UserRole enum instead of object
        var role = GetPrivateProperty<UserRole>(user, "Userrole");
        var name = GetPrivateProperty<string>(user, "Name") ?? email; // Fallback to email if name is not available

        var roleRaw = GetPrivateProperty<object>(user, "Userrole");

        // TEMP DEBUG
        Console.WriteLine($"[DEBUG] role as UserRole: '{role}'");
        Console.WriteLine($"[DEBUG] role as object: '{roleRaw}'");
        Console.WriteLine($"[DEBUG] role raw type: '{roleRaw?.GetType()}'");

        var userId = GetPrivateProperty<int>(user, "Userid");
        var session = _sessionService.CreateSession(userId, role);
        return AuthResult.Success(session, name);
    }

    private static T? GetPrivateProperty<T>(object obj, string propertyName)
    {
        var prop = obj.GetType().GetProperty(
            propertyName,
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);
        return prop == null ? default : (T?)prop.GetValue(obj);
    }
}