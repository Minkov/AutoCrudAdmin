namespace AutoCrudAdmin.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoCrudAdmin.Extensions;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// The ReflectionHelper static class provides a set of properties and methods for performing
/// reflection operations in the context of the AutoCrudAdmin system.
/// This includes operations related to DbContexts and DbSet properties.
/// </summary>
public static class ReflectionHelper
{
    static ReflectionHelper()
    {
        DbContexts = FindDbContexts();
        DbSetProperties = GetDbSetProperties();
    }

    /// <summary>
    /// Gets or sets enumerable collection of DbContext types found within the application domain.
    /// </summary>
    public static IEnumerable<Type> DbContexts { get; set; }

    /// <summary>
    /// Gets an enumerable collection of DbSet properties found within the DbContexts.
    /// </summary>
    public static IEnumerable<PropertyInfo> DbSetProperties { get; }

    /// <summary>
    /// Retrieves the real type of a given entity by stripping away any proxy types that may be wrapping the real type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <returns>The real type of the entity.</returns>
    public static Type GetEntityTypeUnproxied<TEntity>()
        => GetEntityTypeUnproxied(typeof(TEntity));

    /// <summary>
    /// Retrieves the real type of a given entity by stripping away any proxy types that may be wrapping the real type.
    /// </summary>
    /// <param name="entity">The entity whose real type is to be retrieved.</param>
    /// <returns>The real type of the entity.</returns>
    public static Type GetEntityTypeUnproxied(object entity)
        => GetEntityTypeUnproxied(entity.GetType());

    private static Type GetEntityTypeUnproxied(Type entityType)
        => entityType.UnProxy();

    private static IEnumerable<PropertyInfo> GetDbSetProperties()
    {
        var dbSetTypes = DbContexts
            .SelectMany(t => t.GetProperties())
            .Where(p => p.PropertyType.IsGenericType
                        && p.PropertyType.Name.StartsWith("DbSet"))
            .ToList();

        var entityTypes = dbSetTypes
            .Select(dbSet => dbSet.PropertyType.GenericTypeArguments.FirstOrDefault())
            .Distinct()
            .ToHashSet();

        var uniqueEntityTypes = entityTypes
            .Where(parent =>
                parent?.IsGenericParameter == true
                && entityTypes.All(child => child?.IsSubclassOfAnyType(parent) != true))
            .ToHashSet();

        return dbSetTypes
            .DistinctBy(x => x.PropertyType.GenericTypeArguments.FirstOrDefault())
            .Where(x => uniqueEntityTypes.Contains(x.PropertyType.GenericTypeArguments.FirstOrDefault()))
            .OrderBy(x => x.Name)
            .ToList();
    }

    private static IEnumerable<Type> FindDbContexts()
    {
        var allDbContexts = AppDomain.CurrentDomain
            .GetAssemblies()
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