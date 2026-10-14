using PortfolioBuilder.Web.Data;
using PortfolioBuilder.Web.Models;

namespace PortfolioBuilder.Web.Services;

public class AdminAuthService
{
    private const string SessionIdKey = "AdminId";
    private const string SessionNameKey = "AdminUsername";
    private const string RememberCookieName = ".Portfolio.Remember";
    private static readonly TimeSpan RememberDuration = TimeSpan.FromDays(30);

    private readonly AdminUserRepository _admins;

    public AdminAuthService(AdminUserRepository admins)
    {
        _admins = admins;
    }

    public bool IsLoggedIn(HttpContext http) =>
        http.Session.GetInt32(SessionIdKey) != null;

    public string? CurrentUsername(HttpContext http) =>
        http.Session.GetString(SessionNameKey);

    public async Task SignInAsync(HttpContext http, AdminUser user, bool remember)
    {
        StartSession(http, user);

        if (remember)
        {
            string token = RememberToken.Generate();
            DateTime expires = DateTime.UtcNow.Add(RememberDuration);

            await _admins.SetRememberTokenAsync(user.Id, RememberToken.Hash(token), expires);

            http.Response.Cookies.Append(RememberCookieName, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = http.Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Expires = expires
            });
        }
    }

    public async Task<bool> TryRestoreFromCookieAsync(HttpContext http)
    {
        if (IsLoggedIn(http)) return true;

        if (!http.Request.Cookies.TryGetValue(RememberCookieName, out string? token)
            || string.IsNullOrWhiteSpace(token))
            return false;

        var user = await _admins.GetByRememberTokenHashAsync(RememberToken.Hash(token));
        if (user == null)
        {
            http.Response.Cookies.Delete(RememberCookieName);
            return false;
        }

        StartSession(http, user);
        return true;
    }

    public async Task SignOutAsync(HttpContext http)
    {
        int? id = http.Session.GetInt32(SessionIdKey);
        if (id != null)
            await _admins.ClearRememberTokenAsync(id.Value);

        http.Response.Cookies.Delete(RememberCookieName);
        http.Session.Clear();
    }

    private static void StartSession(HttpContext http, AdminUser user)
    {
        http.Session.Clear();
        http.Session.SetInt32(SessionIdKey, user.Id);
        http.Session.SetString(SessionNameKey, user.Username);
    }
}