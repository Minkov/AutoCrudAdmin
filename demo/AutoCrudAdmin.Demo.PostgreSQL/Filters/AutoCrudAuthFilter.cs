namespace AutoCrudAdmin.Demo.PostgreSQL.Filters;

using AutoCrudAdmin.Filters;
using Microsoft.AspNetCore.Http;

public class AutoCrudAuthFilter
    : IAutoCrudAuthFilter
{
    public bool Authorize(HttpContext context)
        => true;
}