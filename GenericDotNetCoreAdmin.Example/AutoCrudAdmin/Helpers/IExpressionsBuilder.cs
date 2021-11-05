namespace AutoCrudAdmin.Helpers
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public interface IExpressionsBuilder<TEntity>
    {
        Expression<Func<TEntity, bool>> ForByEntityId(object entityId);

        Expression<Func<TEntity, TProperty>> ForGetProperty<TProperty>(MemberInfo memberInfo);
    }
}