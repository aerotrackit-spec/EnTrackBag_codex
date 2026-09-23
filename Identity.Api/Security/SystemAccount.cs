using Identity.Api.Data.Entities;

namespace Identity.Api.Security;

public static class SystemAccount
{
    public const string UserName = "admin";
    public static bool IsAdminRole(string? name) =>
        string.Equals(name?.Trim(), "Admin", StringComparison.OrdinalIgnoreCase);
    public static bool IsProtected(UserEntity user) =>
        user.UserRoles.Any(assignment => IsAdminRole(assignment.Role?.Name));
    public static bool IsReservedName(string name) =>
        string.Equals(name.Trim(), UserName, StringComparison.OrdinalIgnoreCase);
    public static void RejectModification(UserEntity user)
    {
        if (IsProtected(user))
            throw new ProtectedAccountException("Users assigned the Admin role can only be viewed or have their password changed.");
    }
}

public sealed class ProtectedAccountException(string message) : Exception(message);
