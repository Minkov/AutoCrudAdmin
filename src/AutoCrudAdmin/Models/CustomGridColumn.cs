namespace AutoCrudAdmin.Models;

using NonFactors.Mvc.Grid;
using System;
using System.Linq.Expressions;

/// <summary>
/// The `CustomGridColumn&lt;TEntity&gt; class represents a customized grid column for the `NonFactors.Mvc.Grid` library.
/// This class allows customization of the column name, its value function, and optional configuration function.
/// </summary>
/// <typeparam name="TEntity">The entity type of the grid column.</typeparam>
public class CustomGridColumn<TEntity>
{
    /// <summary>
    /// Gets or sets the name of the grid column. This is used as the display name for the column in the grid.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Gets or sets the function used to extract the value for the column from the `TEntity` instance.
    /// This is an expression that, given an entity of type `TEntity`, produces a string value for the column.
    /// </summary>
    public Expression<Func<TEntity, string>> ValueFunc { get; set; } = default!;

    /// <summary>
    /// Gets or sets an optional function to further configure the grid column.
    /// This function takes an IGridColumn&lt;TEntity, string&gt;" instance and returns a possibly modified `IGridColumn&lt;TEntity, string&gt;` instance.
    /// If this property is not set, no additional configuration is applied to the column.
    /// </summary>
    public Func<IGridColumn<TEntity, string>, IGridColumn<TEntity, string>>? ConfigurationFunc { get; set; }
}