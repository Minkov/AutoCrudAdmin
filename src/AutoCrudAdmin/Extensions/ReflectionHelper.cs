namespace AutoCrudAdmin.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;

    public static class ReflectionHelper
    {
        static ReflectionHelper()
            => DbSetProperties = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(DbContext)))
                .SelectMany(t => t.GetProperties())
                .Where(p => p.PropertyType.IsGenericType
                            && p.PropertyType.Name.StartsWith("DbSet"));

        public static IEnumerable<PropertyInfo> DbSetProperties { get; }
    }
}