namespace AutoCrudAdmin.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using AutoCrudAdmin.Helpers;
    using Microsoft.EntityFrameworkCore;
    using static AutoCrudAdmin.Constants.Entity;

    public static class TypeExtensions
    {
        private static readonly IDictionary<Type, IList<PropertyInfo>> PrimaryKeyPropertyInfosCache =
            new Dictionary<Type, IList<PropertyInfo>>();

        /// <summary>
        /// Gets the property infos of the primary key of the respective type.
        /// </summary>
        /// <param name="type">The type that we want to get the primary key's property infos for.</param>
        /// <returns>The property infos of the primary key of the respective type.</returns>
        public static IEnumerable<PropertyInfo> GetPrimaryKeyPropertyInfos(this Type type)
        {
            var dbContextTypes = ReflectionHelper.DbContexts
                .ToList();

            var primaryKeyNames = dbContextTypes
                .Select(CreateDbContext)
                .Where(dbContext => dbContext != null)
                .Select(dbContext => dbContext?.Model.FindEntityType(type))
                .Where(x => x != null)
                .Select(entity => entity?.FindPrimaryKey()
                    ?.Properties
                    .Select(x => x.Name)
                    .ToHashSet())
                .FirstOrDefault();

            return type.GetProperties()
                .Where(property => primaryKeyNames != null && primaryKeyNames.Contains(property.Name));
        }

        /// <summary>
        /// Gets the values of the primary key of the type.
        /// </summary>
        /// <param name="type">The type we want to get the primary key property infos for.</param>
        /// <param name="value">The value </param>
        /// <returns></returns>
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
                    .FirstOrDefault();

                return new[] { new KeyValuePair<string, object>(SinglePrimaryKeyName, primaryKeyValue!) };
            }

            return primaryKeyInfos
                .Select(property => new KeyValuePair<string, object>(property.Name, property.GetValue(value) !));
        }

        public static bool IsSubclassOfRawGeneric(this Type type, Type genericParent)
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

        public static bool IsSubclassOfAnyType(this Type type, Type parent)
            => type.IsSubclassOf(parent) || type.IsSubclassOfRawGeneric(parent);

        /// <summary>
        /// Gets the unproxied type of the entity.
        /// </summary>
        /// <param name="entityType">The entity we want to get the type of.</param>
        /// <returns>The unproxied type of the entity.</returns>
        public static Type UnProxy(this Type entityType)
            => entityType.Namespace == "Castle.Proxies"
                ? entityType.BaseType!
                : entityType;

        public static bool IsNavigationProperty(this Type type)
            => (type.IsClass || typeof(IEnumerable).IsAssignableFrom(type)) &&
               type != typeof(string);

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
}