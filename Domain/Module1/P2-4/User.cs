using ProRental.Domain.Enums;
namespace ProRental.Domain.Entities;

public partial class User
{
    private UserRole _userRole;

    private UserRole UserRole 
    { 
        get => _userRole; 
        set => _userRole = value; 
    }

	public void UpdateUserRole(UserRole userRole) => _userRole = userRole;

	// A public constructor (Your Mappers/Gateways will use this!)
    public User(string name, string email, string passwordHash, UserRole role)
    {
        Name = name;
        Email = email;
        Passwordhash = passwordHash;
        UserRole = role;
    }

    // Parameterless constructor required by EF Core for scaffolding/queries
    protected User() { }
}