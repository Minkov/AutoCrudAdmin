namespace AutoCrudAdmin.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using AutoCrudAdmin.Attributes;

    public static class TypeExtensions
    {
        public static PropertyInfo GetPrimaryKeyPropertyInfo(this Type type)
            => type
                .GetProperties()
                .FirstOrDefault(pi =>
                    Attribute.IsDefined(pi, typeof(KeyAttribute))
                    || Attribute.IsDefined(pi, typeof(ComplexKeyAttribute)));

        public static IEnumerable<KeyValuePair<string, object>> GetPrimaryKeyValue(this Type type, object value)
        {
            var primaryKeyInfo = type.GetPrimaryKeyPropertyInfo();

            var primaryKey = primaryKeyInfo.GetValue(value);

            if (primaryKey is IEnumerable<string> complexKey)
            {
                return complexKey.Select(name => type.GetProperty(name))
                    .Select(p => new KeyValuePair<string, object>(p.Name, p.GetValue(value)));
            }

            return new[] { new KeyValuePair<string, object>("pk", primaryKey) };
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