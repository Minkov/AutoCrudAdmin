namespace GenericDotNetCoreAdmin.Filters
{
    using Microsoft.AspNetCore.Http;

    public interface IAutoCrudAuthFilter
    {
        bool Authorize(HttpContext context);
    }
}