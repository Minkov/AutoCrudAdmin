namespace AutoCrudAdmin.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using AutoCrudAdmin.Extensions;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// A class that helps with reflection.
    /// </summary>
    public static class ReflectionHelper
    {
        static ReflectionHelper()
        {
            DbContexts = FindDbContexts();
            DbSetProperties = GetDbSetProperties();
        }

        /// <summary>
        /// Gets or sets the database contexts in the executing assembly.
        /// </summary>
        public static IEnumerable<Type> DbContexts { get; set; }

        /// <summary>
        /// Gets the database sets properties.
        /// </summary>
        public static IEnumerable<PropertyInfo> DbSetProperties { get; }

        /// <summary>
        /// Gets the unproxied type of the entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity we want to get the type of.</typeparam>
        /// <returns>The type of the entity.</returns>
        public static Type GetEntityTypeUnproxied<TEntity>()
            => GetEntityTypeUnproxied(typeof(TEntity));

        /// <summary>
        /// Gets the unproxied type of the entity.
        /// </summary>
        /// <param name="entity">The entity we want to get the type of.</param>
        /// <returns>The unproxied entity's type.</returns>
        public static Type GetEntityTypeUnproxied(object entity)
            => GetEntityTypeUnproxied(entity.GetType());

        private static Type GetEntityTypeUnproxied(Type entityType)
            => entityType.UnProxy();

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
                    !parent!.IsGenericParameter
                    && !entityTypes.Any(
                        child => child!.IsSubclassOfAnyType(parent)))
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