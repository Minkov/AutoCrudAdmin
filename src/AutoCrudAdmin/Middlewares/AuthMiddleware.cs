namespace AutoCrudAdmin.Middlewares;

using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

/// <summary>
/// The `AuthMiddleware` class is a custom middleware for authorization in the AutoCrudAdmin system.
/// This middleware uses the provided `AutoCrudAdminOptions` to apply authorization rules.
/// </summary>
public class AuthMiddleware
{
    private readonly RequestDelegate next;
    private readonly AutoCrudAdminOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the request pipeline.</param>
    /// <param name="options">The `AutoCrudAdminOptions` that hold the authorization filters.</param>
    public AuthMiddleware(RequestDelegate next, AutoCrudAdminOptions options)
    {
        this.next = next;
        this.options = options;
    }

    /// <summary>
    /// This method is called for each HTTP request. It checks the request against the authorization filters,
    /// and either passes the request to the next middleware or ends the request with an unauthorized status code.
    /// </summary>
    /// <param name="httpContext">The HttpContext for the current request.</param>
    /// <returns>A Task representing the completion of the operation.</returns>
    public async Task Invoke(HttpContext httpContext)
    {
        if (this.options.Authorization.Any(filter => !filter.Authorize(httpContext)))
        {
            var statusCode = GetUnauthorizedStatusCode(httpContext);
            httpContext.Response.StatusCode = statusCode;
            return;
        }

        foreach (var filter in this.options.AsyncAuthorization)
        {
            if (!await filter.Authorize(httpContext))
            {
                var statusCode = GetUnauthorizedStatusCode(httpContext);
                httpContext.Response.StatusCode = statusCode;
                return;
            }
        }

        await this.next.Invoke(httpContext);
    }

    /// <summary>
    /// Gets the appropriate status code for unauthorized requests. It returns 401 (Unauthorized) if the user is not authenticated,
    /// and 403 (Forbidden) if the user is authenticated but does not have the required permissions.
    /// </summary>
    /// <param name="httpContext">The HttpContext for the current request.</param>
    /// <returns>An integer representing the HTTP status code.</returns>
    private static int GetUnauthorizedStatusCode(HttpContext httpContext)
        => httpContext.User?.Identity?.IsAuthenticated == true
            ? (int)HttpStatusCode.Forbidden
            : (int)HttpStatusCode.Unauthorized;
}