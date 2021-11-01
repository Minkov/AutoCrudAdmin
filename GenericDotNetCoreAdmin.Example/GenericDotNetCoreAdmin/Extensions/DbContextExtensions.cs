namespace GenericDotNetCoreAdmin.Extensions
{
    using System;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public static class DbContextExtensions
    {
        public static object Set(this DbContext dbContext, Type entityType)
        {
            var set = dbContext.GetType()
                .GetProperties()
                .Where(p => p.PropertyType.IsGenericType
                            && p.PropertyType.Name.StartsWith("DbSet"))
                .FirstOrDefault(property => property.PropertyType.GetGenericArguments().FirstOrDefault() == entityType);

            var result = set.GetValue(dbContext);
            return result;
        }
    }
}