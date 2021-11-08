namespace AutoCrudAdmin.Extensions
{
    using System;
    using System.Linq;
    using AutoCrudAdmin.Helpers;
    using Microsoft.EntityFrameworkCore;

    public static class DbContextExtensions
    {
        public static object Set<TDbContext>(this TDbContext dbContext, Type entityType)
            where TDbContext : DbContext
        {
            var set = dbContext.GetType()
                .GetProperties()
                .Where(p => p.PropertyType.IsGenericType
                            && p.PropertyType.Name.StartsWith("DbSet"))
                .FirstOrDefault(property => property.PropertyType.GetGenericArguments().FirstOrDefault() == entityType);

            // return ExpressionsBuilder.ForGetPropertyValue<TDbContext>(set)(dbContext);
            return set?.GetValue(dbContext);
        }
    }
}