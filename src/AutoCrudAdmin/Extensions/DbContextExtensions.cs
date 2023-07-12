namespace AutoCrudAdmin.Extensions
{
    using System;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The DbContextExtensions class provides extension methods for the DbContext class.
    /// These extensions add functionality related to the AutoCrudAdmin system.
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// The Set method returns a new IQueryable object that can be used to query entities of a given type.
        /// Acts much like `Set&lt;TEntity&gt;`, but `TEntity` is a type parameter.
        /// </summary>
        /// <param name="dbContext">The DbContext instance that this method extends.</param>
        /// <param name="entityType">The type of the entities to be queried.</param>
        /// <typeparam name="TDbContext">The `DbContext` type. Must implement `DbContext`.</typeparam>
        /// <returns>A new IQueryable object that can be used to query entities of the specified type.</returns>
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