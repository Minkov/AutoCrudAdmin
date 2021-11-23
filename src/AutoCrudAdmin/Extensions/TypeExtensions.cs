namespace AutoCrudAdmin.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using AutoCrudAdmin.Helpers;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.EntityFrameworkCore;
    using static AutoCrudAdmin.Constants.Entity;

    public static class TypeExtensions
    {
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

        public static IEnumerable<PropertyInfo> GetPrimaryKeyPropertyInfos(this Type type)
        {
            var dbContextTypes = ReflectionHelper.DbContexts
                .ToList();

            var primaryKeyNames = dbContextTypes
                .Select(t => CreateDbContext(t))
                .Where(dbContext => dbContext != null)
                .Select(dbContext => dbContext.Model.FindEntityType(type))
                .Where(x => x != null)
                .Select(entity => entity.FindPrimaryKey()
                    .Properties
                    .Select(x => x.Name)
                    .ToHashSet())
                .FirstOrDefault();

            return type.GetProperties()
                .Where(property => primaryKeyNames.Contains(property.Name));
        }

        public static IEnumerable<KeyValuePair<string, object>> GetPrimaryKeyValue(this Type type, object value)
        {
            var primaryKeyInfos = type.GetPrimaryKeyPropertyInfos()
                .ToList();

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
    }
}