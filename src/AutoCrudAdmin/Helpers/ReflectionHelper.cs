namespace AutoCrudAdmin.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using AutoCrudAdmin.Extensions;
    using Microsoft.EntityFrameworkCore;

    public static class ReflectionHelper
    {
        static ReflectionHelper()
        {
            DbContexts = FindDbContexts();
            DbSetProperties = GetDbSetProperties();
        }

        public static IEnumerable<Type> DbContexts { get; set; }

        public static IEnumerable<PropertyInfo> DbSetProperties { get; }

        private static IEnumerable<PropertyInfo> GetDbSetProperties()
        {
            var dbSetTypes = DbContexts
                .SelectMany(t => t.GetProperties())
                .Where(p => p.PropertyType.IsGenericType
                            && p.PropertyType.Name.StartsWith("DbSet"));

            var entityTypes = dbSetTypes
                .Select(dbSet => dbSet.PropertyType.GenericTypeArguments.FirstOrDefault())
                .Distinct()
                .ToHashSet();

            var uniqueEntityTypes = entityTypes
                .Where(parent =>
                    !parent.IsGenericParameter
                    && !entityTypes.Any(
                        child => child.IsSubclassOfAnyType(parent)))
                .ToHashSet();

            return dbSetTypes
                .DistinctBy(x => x.PropertyType.GenericTypeArguments.FirstOrDefault())
                .Where(x => uniqueEntityTypes.Contains(x.PropertyType.GenericTypeArguments.FirstOrDefault()))
                .OrderBy(x => x.Name)
                .ToList();
        }

        private static IEnumerable<Type> FindDbContexts()
        {
            var allDbContexts = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(DbContext)))
                .Distinct()
                .ToHashSet();

            var dbContexts = allDbContexts
                .Where(parent => !allDbContexts.Any(
                    child => child.IsSubclassOfAnyType(parent)))
                .ToList();

            return dbContexts;
        }
    }
}