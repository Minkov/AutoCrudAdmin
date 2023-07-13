namespace AutoCrudAdmin.Helpers;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The NavHelper static class provides a method for retrieving navigation items for the AutoCrudAdmin system.
/// </summary>
public static class NavHelper
{
    /// <summary>
    /// The <see cref="GetNavItems"/> method retrieves the names of all DbSet properties in the ReflectionHelper.
    /// </summary>
    /// <returns>An enumerable collection of string representing the names of DbSet properties.</returns>
    public static IEnumerable<string> GetNavItems()
        => ReflectionHelper.DbSetProperties
            .Select(property => property.Name);
}