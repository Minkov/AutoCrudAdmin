namespace AutoCrudAdmin.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;

    public static class ReflectionHelper
    {
        static ReflectionHelper()
        {
            DbContexts = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(DbContext)));
            DbSetProperties = DbContexts
                .SelectMany(t => t.GetProperties())
                .Where(p => p.PropertyType.IsGenericType
                            && p.PropertyType.Name.StartsWith("DbSet"));
        }

        public static IEnumerable<Type> DbContexts { get; set; }

        public static IEnumerable<PropertyInfo> DbSetProperties { get; }
    }
}