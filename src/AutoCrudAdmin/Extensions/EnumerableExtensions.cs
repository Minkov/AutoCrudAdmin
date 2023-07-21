namespace AutoCrudAdmin.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The EnumerableExtensions class provides extension methods for the IEnumerable&lt;T&gt; interface.
/// These extensions add functionality related to the AutoCrudAdmin system.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// The DistinctBy method returns a new collection that includes only distinct elements from the original collection, based on a specified key selector function.
    /// </summary>
    /// <param name="collection">The IEnumerable&lt;T&gt; instance that this method extends.</param>
    /// <param name="func">A function to extract the key for each element.</param>
    /// <typeparam name="T">The type of the elements of the source collection.</typeparam>
    /// <returns>A new IEnumerable&lt;T&gt; that contains distinct elements from the source collection based on the key selector function.</returns>
    public static IEnumerable<T> DistinctBy<T>(this IEnumerable<T> collection, Func<T, object> func)
        => collection
            .GroupBy(func)
            .Select(x => x.First())
            .ToList();
}