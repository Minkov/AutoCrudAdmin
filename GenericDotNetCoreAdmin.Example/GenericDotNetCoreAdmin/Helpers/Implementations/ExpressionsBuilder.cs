namespace GenericDotNetCoreAdmin.Helpers.Implementations
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using GenericDotNetCoreAdmin.Extensions;

    public class ExpressionsBuilder
    {
        public static Expression<Func<TEntity, bool>> ForByEntityId<TEntity>(object entityId)
        {
            var entityType = typeof(TEntity);
            var primaryKeyProperty = entityType
                .GetPrimaryKeyPropertyInfo();
            var parameter = Expression.Parameter(typeof(object), "model");
            var convertedParameter = Expression.Convert(
                parameter,
                entityType
            );

            var memberAccess = Expression.MakeMemberAccess(
                convertedParameter,
                primaryKeyProperty
            );

            var cast = Expression.Call(
                Expression.Convert(memberAccess, typeof(object)),
                typeof(object).GetMethod("ToString")!);

            var id = Expression.Call(
                Expression.Convert(
                    Expression.Constant(entityId),
                    typeof(object)),
                typeof(object).GetMethod("ToString")!);

            var equals = Expression.Equal(
                cast,
                id
            );

            return Expression.Lambda<Func<TEntity, bool>>(
                equals,
                parameter);
        }

        public static Expression<Func<TEntity, TProperty>> ForGetProperty<TEntity, TProperty>(MemberInfo property)
        {
            var entityType = typeof(TEntity);
            var parameter = Expression.Parameter(entityType, "model");

            // model.Column
            var memberAccess = Expression.MakeMemberAccess(
                parameter,
                property);

            // model => model.Column
            return Expression.Lambda<Func<TEntity, TProperty>>(memberAccess, parameter);
        }
    }
}