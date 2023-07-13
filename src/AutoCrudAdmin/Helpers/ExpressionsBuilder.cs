namespace AutoCrudAdmin.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoCrudAdmin.Extensions;
using static AutoCrudAdmin.Constants.Entity;

/// <summary>
/// The ExpressionsBuilder static class provides a set of methods for building and manipulating LINQ expressions.
/// These expressions are primarily used in the context of the AutoCrudAdmin system.
/// </summary>
public static class ExpressionsBuilder
{
    /// <summary>
    /// Constructs a predicate expression for an entity based on its primary keys.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="primaryKeys">A dictionary where the key is the primary key name and the value is its corresponding value.</param>
    /// <returns>An expression that represents the predicate.</returns>
    public static Expression<Func<TEntity, bool>> ForByEntityPrimaryKey<TEntity>(
        IDictionary<string, string> primaryKeys)
        where TEntity : class
    {
        var entityType = ReflectionHelper.GetEntityTypeUnproxied<TEntity>();
        var parameter = Expression.Parameter(typeof(object), "model");
        var convertedParameter = Expression.Convert(
            parameter,
            entityType);

        var equals = Expression.Equal(
            Expression.Constant(true),
            Expression.Constant(true));

        primaryKeys
            .Select(pair =>
            {
                var key = pair.Key == SinglePrimaryKeyName
                    ? entityType.GetPrimaryKeyPropertyInfos()
                        .Select(x => x.Name)
                        .FirstOrDefault() !
                    : pair.Key;

                return new KeyValuePair<string, string>(key!, pair.Value);
            })
            .Select(pair =>
                ForPrimaryKeySubExpression(
                    pair.Key,
                    pair.Value,
                    convertedParameter,
                    entityType))
            .ToList()
            .ForEach(expression =>
                equals = Expression.And(
                    equals,
                    expression));

        return Expression.Lambda<Func<TEntity, bool>>(
            equals,
            parameter);
    }

    /// <summary>
    /// Constructs a property access expression for a specific property of an entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="property">The property for which to construct the expression.</param>
    /// <returns>An expression that represents the property access.</returns>
    public static Expression<Func<TEntity, TProperty>> ForGetProperty<TEntity, TProperty>(
        PropertyInfo property)
    {
        var entityType = ReflectionHelper.GetEntityTypeUnproxied<TEntity>();
        var parameter = Expression.Parameter(entityType, "model");

        // model.Column
        var memberAccess = Expression.MakeMemberAccess(
            parameter,
            property);

        // model => model.Column
        return Expression.Lambda<Func<TEntity, TProperty>>(memberAccess, parameter);
    }

    /// <summary>
    /// Constructs a factory method for creating instances of a specific type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <returns>A delegate representing the factory method.</returns>
    public static Func<TEntity> ForCreateInstance<TEntity>()
        => Expression.Lambda<Func<TEntity>>(Expression.New(ReflectionHelper.GetEntityTypeUnproxied<TEntity>()))
            .Compile();

    /// <summary>
    /// Constructs a method for getting the value of a specific property of an entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="property">The property for which to construct the method.</param>
    /// <returns>A delegate representing the method.</returns>
    public static Func<TEntity, object> ForGetPropertyValue<TEntity>(PropertyInfo property)
    {
        var type = ReflectionHelper.GetEntityTypeUnproxied<TEntity>();
        var instanceParam = Expression.Parameter(type);
        return Expression.Lambda<Func<TEntity, object>>(
            Expression.Convert(
                Expression.Call(
                    instanceParam,
                    property.GetGetMethod() !),
                typeof(object)),
            instanceParam).Compile();
    }

    private static Expression ForPrimaryKeySubExpression(
        string name,
        string value,
        Expression parameter,
        Type entityType)
    {
        var memberAccess = Expression.MakeMemberAccess(
            parameter,
            entityType.GetProperty(name) !);

        var cast = Expression.Convert(
            memberAccess,
            typeof(object));

        var id = Expression.Convert(
            Expression.Constant(value),
            typeof(object));

        return Expression.Equal(cast, id);
    }
}