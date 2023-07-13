namespace AutoCrudAdmin.Models;

using NonFactors.Mvc.Grid;
using System;
using System.Linq.Expressions;

public class CustomGridColumn<TEntity>
{
    public string Name { get; set; } = default!;

    public Expression<Func<TEntity, string>> ValueFunc { get; set; } = default!;

    public Func<IGridColumn<TEntity, string>, IGridColumn<TEntity, string>>? ConfigurationFunc { get; set; }
}