namespace GenericDotNetCoreAdmin.Example.Filters
{
    using GenericDotNetCoreAdmin.Filters;
    using Microsoft.AspNetCore.Http;

    public class AutoCrudAuthFilter
        : IAutoCrudAuthFilter
    {
        public bool Authorize(HttpContext context)
            => false;
    }
}