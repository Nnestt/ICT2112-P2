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

    public AuthResult Authenticate(int userId, string password)
    {
        // User.Userid is private — use EF's raw SQL query to bypass the private property.
        // This queries the "User" table directly without touching the private C# properties.
        var users = _db.Users
            .FromSqlRaw(@"SELECT * FROM ""User"" WHERE ""userId"" = {0}", userId)
            .AsEnumerable()
            .ToList();

        if (users.Count == 0)
            return AuthResult.Failure("Invalid user ID or password.");

        var user = users[0];

        // Access Passwordhash and Userrole via reflection since they are private properties.
        // This is necessary because the entity file cannot be modified.
        var passwordHash = GetPrivateProperty<string>(user, "Passwordhash");
        var userRoleRaw  = GetPrivateProperty<object>(user, "Userrole");

        if (passwordHash == null || userRoleRaw == null)
            return AuthResult.Failure("User account data is incomplete.");

        // Password check (plain for now — swap for BCrypt once package is added)
        // TODO: BCrypt.Net.BCrypt.Verify(password, passwordHash)
        if (password != passwordHash)
            return AuthResult.Failure("Invalid user ID or password.");

        // userRoleRaw is the UserRole enum value stored by EF
        if (!Enum.TryParse<UserRole>(userRoleRaw.ToString(), ignoreCase: true, out var role))
            return AuthResult.Failure("User account has an unrecognised role.");

        var session = _sessionService.CreateSession(userId, role);
        return AuthResult.Success(session);
    }

    // Reads a private property from an entity without modifying the entity file.
    private static T? GetPrivateProperty<T>(object obj, string propertyName)
    {
        var prop = obj.GetType().GetProperty(
            propertyName,
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);
        return prop == null ? default : (T?)prop.GetValue(obj);
    }
}