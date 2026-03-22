using ProRental.Domain.Enums;

namespace ProRental.Domain.Entities;

public partial class Session
{
    // ── Public accessors for auto-generated private properties ──────────
    public int SessionId => Sessionid;
    public int UserId => Userid;
    public string RoleString => Role;
    public DateTime CreatedAt => Createdat;
    public DateTime ExpiresAt => Expiresat;

    // ── Computed helpers ─────────────────────────────────────────────────
    public UserRole RoleEnum => Enum.Parse<UserRole>(Role);
    public bool IsExpired() => DateTime.UtcNow >= Expiresat;

    // ── Factory method ───────────────────────────────────────────────────
    public static Session Create(int userId, UserRole role)
    {
        var session = new Session();
        typeof(Session).GetProperty("Userid")!.SetValue(session, userId);
        typeof(Session).GetProperty("Role")!.SetValue(session, role.ToString());
        typeof(Session).GetProperty("Createdat")!.SetValue(session, DateTime.UtcNow);
        typeof(Session).GetProperty("Expiresat")!.SetValue(session, DateTime.UtcNow.AddHours(2));
        return session;
    }
}