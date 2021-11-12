namespace AutoCrudAdmin.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using AutoCrudAdmin.Helpers;
    using Microsoft.EntityFrameworkCore;
    using static AutoCrudAdmin.Constants.Entity;

    public static class TypeExtensions
    {
        public static IEnumerable<PropertyInfo> GetPrimaryKeyPropertyInfos(this Type type)
        {
            var dbContextType = ReflectionHelper.DbContexts
                .FirstOrDefault();

            var primaryKeyNames = (Activator.CreateInstance(dbContextType) as DbContext)
                .Model.FindEntityType(type)
                .FindPrimaryKey()
                .Properties
                .Select(x => x.Name)
                .ToHashSet();

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
    }
}