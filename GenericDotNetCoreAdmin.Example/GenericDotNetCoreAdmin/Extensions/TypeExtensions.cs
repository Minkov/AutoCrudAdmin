namespace GenericDotNetCoreAdmin.Extensions
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    public static class TypeExtensions
    {
        public static PropertyInfo GetPrimaryKeyPropertyInfo(this Type type)
            => type
                .GetProperties()
                .FirstOrDefault(pi => Attribute.IsDefined(pi, typeof(KeyAttribute)));

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