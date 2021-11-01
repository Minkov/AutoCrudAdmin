namespace GenericDotNetCoreAdmin.TagHelpers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class AuthMiddleware
    {
        private readonly RequestDelegate next;
        private readonly AutoCrudAdminOptions options;

        public AuthMiddleware(RequestDelegate next, AutoCrudAdminOptions options)
        {
            this.next = next;
            this.options = options;
        }

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

        private static int GetUnauthorizedStatusCode(HttpContext httpContext)
            => httpContext.User?.Identity?.IsAuthenticated == true
                ? (int)HttpStatusCode.Forbidden
                : (int)HttpStatusCode.Unauthorized;
    }
}