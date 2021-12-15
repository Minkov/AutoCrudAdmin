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
        private static IDictionary<Type, IList<PropertyInfo>> primaryKeyPropertyInfosCache =
            new Dictionary<Type, IList<PropertyInfo>>();

        public static IEnumerable<PropertyInfo> GetPrimaryKeyPropertyInfos(this Type type)
        {
            var dbContextTypes = ReflectionHelper.DbContexts
                .ToList();

            var primaryKeyNames = dbContextTypes
                .Select(CreateDbContext)
                .Where(dbContext => dbContext != null)
                .Select(dbContext => dbContext.Model.FindEntityType(type))
                .Where(x => x != null)
                .Select(entity => entity.FindPrimaryKey()
                    ?.Properties
                    .Select(x => x.Name)
                    .ToHashSet())
                .FirstOrDefault();

            return type.GetProperties()
                .Where(property => primaryKeyNames != null && primaryKeyNames.Contains(property.Name));
        }

        public static IEnumerable<KeyValuePair<string, object>> GetPrimaryKeyValue(this Type type, object value)
        {
            if (!primaryKeyPropertyInfosCache.ContainsKey(type))
            {
                primaryKeyPropertyInfosCache.Add(type, type.GetPrimaryKeyPropertyInfos().ToList());
            }

            var primaryKeyInfos = primaryKeyPropertyInfosCache[type];

            if (primaryKeyInfos.Count == 1)
            {
                var primaryKeyValue = primaryKeyInfos
                    .Select(property => property.GetValue(value))
                    .FirstOrDefault();

                return new[] { new KeyValuePair<string, object>(SinglePrimaryKeyName, primaryKeyValue) };
            }

            return primaryKeyInfos
                .Select(property => new KeyValuePair<string, object>(property.Name, property.GetValue(value)));
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

        public static Type UnProxy(this Type entityType)
            => entityType.Namespace == "Castle.Proxies"
                ? entityType.BaseType
                : entityType;

        public static bool IsNavigationProperty(this Type type)
            => (type.IsClass || typeof(IEnumerable).IsAssignableFrom(type)) &&
               type != typeof(string);

        public static bool IsEnumerableExceptString(this Type type)
            => typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);

        private static DbContext CreateDbContext(Type type)
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