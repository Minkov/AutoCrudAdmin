namespace AutoCrudAdmin.Helpers;

using System.Collections.Generic;
using System.Linq;
using AutoCrudAdmin.Extensions;

public static class NavHelper
{
    public static IEnumerable<string> GetNavItems()
        => ReflectionHelper.DbSetProperties
            .Select(property => property.Name);
}