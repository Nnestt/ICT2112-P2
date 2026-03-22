using ProRental.Domain.Entities;

namespace ProRental.Interfaces.Domain;

public interface IAuthenticationService
{
    /// <summary>
    /// Authenticates a user by userId and password.
    /// Returns an AuthResult containing success status and the created session if successful.
    /// </summary>
    AuthResult Authenticate(int userId, string password);
}