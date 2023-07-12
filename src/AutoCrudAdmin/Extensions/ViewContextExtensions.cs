namespace AutoCrudAdmin.Extensions;

using Microsoft.AspNetCore.Mvc.Rendering;

/// <summary>
/// The ViewContextExtensions class provides extension methods for the ViewContext class.
/// These extensions add functionality related to the AutoCrudAdmin system.
/// </summary>
public static class ViewContextExtensions
{
    /// <summary>
    /// The GetControllerName method retrieves the controller name from the route data of the ViewContext.
    /// </summary>
    /// <param name="viewContext">The ViewContext instance that this method extends.</param>
    /// <returns>The controller name from the route data if it exists; otherwise, null.</returns>
    public static string? GetControllerName(this ViewContext viewContext)
        => viewContext.RouteData?.Values["controller"]?.ToString();
}