namespace AutoCrudAdmin.Extensions;

using Microsoft.AspNetCore.Mvc.Rendering;

public static class ViewContextExtensions
{
    public static string? GetControllerName(this ViewContext viewContext)
        => viewContext.RouteData?.Values["controller"]?.ToString();
}