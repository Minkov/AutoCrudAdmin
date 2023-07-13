namespace AutoCrudAdmin.Helpers;

using System.Collections.Generic;
using System.Linq;

public static class NavHelper
{
    public static IEnumerable<string> GetNavItems()
        => ReflectionHelper.DbSetProperties
            .Select(property => property.Name);
}