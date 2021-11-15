namespace AutoCrudAdmin.Helpers.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using AutoCrudAdmin.Extensions;
    using AutoCrudAdmin.ViewModels;
    using Microsoft.EntityFrameworkCore;
    using static AutoCrudAdmin.Constants.Entity;

    public class FormControlsHelper
        : IFormControlsHelper
    {
        private static readonly ISet<Type> PrimitiveTypes = new HashSet<Type>
        {
            typeof(string),
            typeof(int),
            typeof(int),
            typeof(bool),
            typeof(DateTime),
        };

        private readonly DbContext dbContext;

        static FormControlsHelper()
            => Types = ReflectionHelper.DbSetProperties
                .Select(p => p.PropertyType)
                .Select(dt => dt.GetGenericArguments().FirstOrDefault())
                .ToHashSet();

        public FormControlsHelper(DbContext dbContext)
            => this.dbContext = dbContext;

        private static ISet<Type> Types { get; set; }

        public IEnumerable<FormControlViewModel> GenerateFormControls<TEntity>(
            TEntity entity,
            EntityAction entityAction)
            => this.GeneratePrimaryKeyFormControls(entity, entityAction)
                .Concat(GeneratePrimitiveFormControls(entity))
                .Concat(this.GenerateComplexFormControls(entity));

        private IEnumerable<FormControlViewModel> GeneratePrimaryKeyFormControls<TEntity>(
            TEntity entity,
            EntityAction entityAction)
        {
            var entityType = typeof(TEntity);

            var primaryKeyValues = entityType.GetPrimaryKeyValue(entity)
                .ToList();

            if (entityAction == EntityAction.Create && primaryKeyValues.Count > 1)
            {
                return primaryKeyValues
                    .Select(pair =>
                    {
                        var name = pair.Key[..^2];
                        var property = entityType.GetProperty(name);

                        return new FormControlViewModel
                        {
                            Name = name,
                            Type = property.PropertyType,
                            Value = null,
                            Options = this.dbContext.Set(property.PropertyType) as IEnumerable<object>,
                            IsComplex = true,
                            IsReadOnly = false,
                        };
                    });
            }

            return primaryKeyValues
                .Select(pair => new FormControlViewModel
                {
                    Name = pair.Key == SinglePrimaryKeyName
                        ? typeof(TEntity).GetPrimaryKeyPropertyInfos()
                            .Select(pk => pk.Name)
                            .FirstOrDefault()
                        : pair.Key,
                    Type = pair.Value.GetType(),
                    Value = pair.Key == SinglePrimaryKeyName
                        ? ExpressionsBuilder.ForGetPropertyValue<TEntity>(
                            typeof(TEntity).GetPrimaryKeyPropertyInfos()
                                .FirstOrDefault())(
                            entity)
                        : ExpressionsBuilder.ForGetPropertyValue<TEntity>(
                            typeof(TEntity).GetProperty(pair.Key))(entity),
                    IsComplex = false,
                    IsReadOnly = true,
                });
        }

        private static IEnumerable<FormControlViewModel> GeneratePrimitiveFormControls<TEntity>(TEntity entity)
        {
            var entityType = typeof(TEntity);

            return entityType.GetProperties()
                .Where(property => IsPrimitiveProperty(property, entityType)
                                   && !IsComplexPrimaryKey(property, entityType))
                .Select(property => new FormControlViewModel
                {
                    Name = property.Name,
                    Type = property.PropertyType,
                    Value = ExpressionsBuilder.ForGetPropertyValue<TEntity>(property)(entity),
                    IsComplex = false,
                })
                .OrderBy(x => x.Name);
        }

        private static bool IsDbContextEntity<TEntity>(PropertyInfo property, TEntity entity)
            => Types.Contains(property.PropertyType);

        private static bool IsPrimitiveProperty(PropertyInfo property, Type entityType)
            => entityType
                   .GetPrimaryKeyPropertyInfos()
                   .Any(pk => pk == property)
               || property.PropertyType.IsEnum
               || (PrimitiveTypes.Contains(property.PropertyType) && !property.Name.ToLower().EndsWith("id"));

        private static bool IsComplexPrimaryKey(PropertyInfo property, Type entityType)
            => entityType
                .GetPrimaryKeyPropertyInfos()
                .Any(p => p == property);

        private static bool IsPartOfPrimaryKey(PropertyInfo property, Type entityType)
            => entityType.GetPrimaryKeyPropertyInfos()
                .Any(pk => pk.Name == property.Name + "Id");

        private IEnumerable<FormControlViewModel> GenerateComplexFormControls<TEntity>(TEntity entity)
        {
            var entityType = typeof(TEntity);

            return entityType.GetProperties()
                .Where(property => IsDbContextEntity(property, entity) && !IsPartOfPrimaryKey(property, entityType))
                .Select(property => new FormControlViewModel
                {
                    Name = property.Name,
                    Type = property.PropertyType,
                    Value =
                        ExpressionsBuilder.ForGetPropertyValue<TEntity>(entityType.GetProperty(property.Name + "Id"))(
                            entity),
                    Options = this.dbContext.Set(property.PropertyType) as IEnumerable<object>,
                    IsComplex = true,
                    IsReadOnly = false,
                })
                .OrderBy(x => x.Name);
        }
    }
}