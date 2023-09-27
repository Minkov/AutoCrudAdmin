namespace AutoCrudAdmin.Extensions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoCrudAdmin.Helpers;
using Microsoft.EntityFrameworkCore;
using static AutoCrudAdmin.Constants.Entity;

/// <summary>
/// The TypeExtensions class provides extension methods for the Type class.
/// These extensions add functionality related to the AutoCrudAdmin system.
/// </summary>
public static class TypeExtensions
{
    private static readonly IDictionary<Type, IList<PropertyInfo>> PrimaryKeyPropertyInfosCache =
        new Dictionary<Type, IList<PropertyInfo>>();

    /// <summary>
    /// The GetPrimaryKeyPropertyInfos method retrieves the primary key property information for a given type.
    /// </summary>
    /// <param name="type">The Type instance that this method extends.</param>
    /// <returns>An IEnumerable of PropertyInfo instances representing the primary key properties of the given type.</returns>
    public static IEnumerable<PropertyInfo> GetPrimaryKeyPropertyInfos(this Type type)
    {
        var dbContextTypes = ReflectionHelper.DbContexts
            .ToList();

        var primaryKeyNames = dbContextTypes
            .Select(CreateDbContext)
            .Where(dbContext => dbContext != null)
            .Select(dbContext => dbContext!.Model.FindEntityType(type))
            .Where(entity => entity != null)
            .Select(entity => entity!.FindPrimaryKey()
                ?.Properties
                .Select(x => x.Name)
                .ToHashSet())
            .FirstOrDefault();

        return type.GetProperties()
            .Where(property => primaryKeyNames != null && primaryKeyNames.Contains(property.Name));
    }

    /// <summary>
    /// The GetForeignKeyPropertyInfos method retrieves the foreign key property information for a given type.
    /// </summary>
    /// <param name="type">The Type instance that this method extends.</param>
    /// <returns>An IEnumerable of PropertyInfo instances representing the foreign key properties of the given type.</returns>
    public static IEnumerable<PropertyInfo> GetForeignKeyPropertyInfos(this Type type)
        => ReflectionHelper.DbContexts
            .Select(CreateDbContext)
            .Where(dbContext => dbContext != null)
            .SelectMany(dbContext => dbContext!.Model.FindEntityType(type) !.GetForeignKeys())
            .SelectMany(fk => fk.Properties.Select(p => p.PropertyInfo!))
            .ToHashSet();

    /// <summary>
    /// The GetPrimaryKeyValue method retrieves the primary key value of a given object of a certain type.
    /// </summary>
    /// <param name="type">The Type instance that this method extends.</param>
    /// <param name="value">The object from which to retrieve the primary key value.</param>
    /// <returns>An IEnumerable of KeyValuePair instances representing the primary key property names and values of the given object.</returns>
    public static IEnumerable<KeyValuePair<string, object>> GetPrimaryKeyValue(this Type type, object value)
    {
        if (!PrimaryKeyPropertyInfosCache.ContainsKey(type))
        {
            PrimaryKeyPropertyInfosCache.Add(type, type.GetPrimaryKeyPropertyInfos().ToList());
        }

        var primaryKeyInfos = PrimaryKeyPropertyInfosCache[type];

        if (primaryKeyInfos.Count == 1)
        {
            var primaryKeyValue = primaryKeyInfos
                .Select(property => property.GetValue(value))
                .FirstOrDefault() !;

            return new[] { new KeyValuePair<string, object>(SinglePrimaryKeyName, primaryKeyValue) };
        }

        return primaryKeyInfos
            .Select(property => new KeyValuePair<string, object>(property.Name, property.GetValue(value) !));
    }

    /// <summary>
    /// The IsSubclassOfRawGeneric method determines whether a given type is a subclass of a specific generic type.
    /// </summary>
    /// <param name="type">The Type instance that this method extends.</param>
    /// <param name="genericParent">The generic type to check against.</param>
    /// <returns>True if the type is a subclass of the generic type, otherwise false.</returns>
    public static bool IsSubclassOfRawGeneric(this Type? type, Type genericParent)
    {
        if (type == genericParent)
        {
            return false;
        }

        while (type != null && type != typeof(object))
        {
            var cur = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
            if (genericParent == cur)
            {
                return true;
            }

            type = type.BaseType;
        }

        return false;
    }

    /// <summary>
    /// The IsSubclassOfAnyType method determines whether a given type is a subclass of a specific type or a subclass of a raw generic type.
    /// </summary>
    /// <param name="type">The Type instance that this method extends.</param>
    /// <param name="parent">The parent type to check against.</param>
    /// <returns>True if the type is a subclass of the parent type or a subclass of a raw generic type, otherwise false.</returns>
    public static bool IsSubclassOfAnyType(this Type type, Type parent)
        => type.IsSubclassOf(parent) || type.IsSubclassOfRawGeneric(parent);

    /// <summary>
    /// The UnProxy method unwraps a proxy type, returning the base type if the given type is a proxy.
    /// </summary>
    /// <param name="entityType">The Type instance that this method extends.</param>
    /// <returns>The base type if the given type is a proxy, otherwise the given type.</returns>
    // TODO: Check what is this.
    public static Type UnProxy(this Type entityType)
        => (entityType.Namespace == "Castle.Proxies"
            ? entityType.BaseType
            : entityType) !;

    /// <summary>
    /// The IsNavigationProperty method determines whether a given type is a navigation property.
    /// </summary>
    /// <param name="type">The Type instance that this method extends.</param>
    /// <returns>True if the type is a navigation property, otherwise false.</returns>
    public static bool IsNavigationProperty(this Type type)
        => (type.IsClass || typeof(IEnumerable).IsAssignableFrom(type)) &&
           type != typeof(string);

    /// <summary>
    /// The IsEnumerableExceptString method determines whether a given type is an enumerable type other than string.
    /// </summary>
    /// <param name="type">The Type instance that this method extends.</param>
    /// <returns>True if the type is an enumerable type other than string, otherwise false.</returns>
    public static bool IsEnumerableExceptString(this Type type)
        => typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);

    private static DbContext? CreateDbContext(Type type)
    {
        try
        {
            return Activator.CreateInstance(type) as DbContext;
        }
        catch
        {
            return null;
        }
    }
}