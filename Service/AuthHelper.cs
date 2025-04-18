using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public static class AuthHelper
{
    public static string HashPassword(string senha)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(senha);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public static bool VerifyPassword(string passwordEndered, string savedHashPassword)
    {
        var hashPasswordEndered = HashPassword(passwordEndered);
        return hashPasswordEndered == savedHashPassword;
    }

    public static int GetUserIdFromToken(HttpContext httpContext)
    {
        var userIdClaim = httpContext.User.FindFirst("id");
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }

        throw new UnauthorizedAccessException("Unauthenticated user.");
    }

}
