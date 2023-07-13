namespace AutoCrudAdmin.Filters;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public interface IAutoCrudAuthFilter
{
    bool Authorize(HttpContext context);
}

public interface IAsyncAuthCrudFilter
{
    Task<bool> Authorize(HttpContext context);
}