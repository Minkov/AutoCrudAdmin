namespace AutoCrudAdmin.Extensions
{
    using System;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public static class DbContextExtensions
    {
        public static IQueryable<object> Set<TDbContext>(this TDbContext dbContext, Type entityType)
            where TDbContext : DbContext
            => (IQueryable<object>)dbContext
                .GetType()
                .GetMethods()
                .FirstOrDefault(m => m.Name == "Set" && m.GetParameters().Length == 0)
                ?.MakeGenericMethod(entityType)
                .Invoke(dbContext, null) !;
    }
}