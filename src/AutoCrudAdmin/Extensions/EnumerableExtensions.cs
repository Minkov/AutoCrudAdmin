namespace AutoCrudAdmin.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtensions
    {
        public static IEnumerable<T> DistinctBy<T>(this IEnumerable<T> collection, Func<T, object> func)
            => collection
                .GroupBy(func)
                .Select(x => x.First())
                .ToList();
    }
}