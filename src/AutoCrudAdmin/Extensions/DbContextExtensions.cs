namespace AutoCrudAdmin.Extensions
{
    using System;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public static class DbContextExtensions
    {
        public static IQueryable<object> Set<TDbContext>(this TDbContext dbContext, Type entityType)
            where TDbContext : DbContext
        {
            var result = dbContext
                    .GetType()
                    .GetMethods()
                    .Where(m => m.Name == "Set")
                    .FirstOrDefault(m => m.GetParameters().Length == 0)
                    !.MakeGenericMethod(entityType)
                    .Invoke(dbContext, null)
                !;

            return (IQueryable<object>)result;
        }
    }
}