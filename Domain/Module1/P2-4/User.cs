using ProRental.Domain.Enums;
namespace ProRental.Domain.Entities;

public partial class User
{
    public void UpdateUserRole(UserRole userRole) => 
        typeof(User).GetProperty("Userrole", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance)!
            .SetValue(this, userRole);

    public User(string name, string email, string passwordHash, UserRole role)
    {
        typeof(User).GetProperty("Name",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance)!
            .SetValue(this, name);
        typeof(User).GetProperty("Email",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance)!
            .SetValue(this, email);
        typeof(User).GetProperty("Passwordhash",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance)!
            .SetValue(this, passwordHash);
        typeof(User).GetProperty("Userrole",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance)!
            .SetValue(this, role);
    }

    protected User() { }
}