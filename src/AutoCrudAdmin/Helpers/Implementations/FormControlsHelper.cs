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

        public IEnumerable<FormControlViewModel> GenerateFormControls<TEntity>(TEntity entity)
            => GeneratePrimaryKeyFormControls(entity)
                .Concat(GeneratePrimitiveFormControls(entity))
                .Concat(this.GenerateComplexFormControls(entity));

        private static IEnumerable<FormControlViewModel> GeneratePrimaryKeyFormControls<TEntity>(TEntity entity)
            => typeof(TEntity)
                .GetPrimaryKeyValue(entity)
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

        private static bool IsDbContextEntity<TEntity>(PropertyInfo property, TEntity entity)
            => Types.Contains(property.PropertyType);

        private static bool IsPrimitiveProperty(PropertyInfo property, Type entityType)
            => entityType
                   .GetPrimaryKeyPropertyInfos()
                   .Any(pk => pk == property)
               || property.PropertyType.IsEnum
               || (PrimitiveTypes.Contains(property.PropertyType) && !property.Name.ToLower().EndsWith("id"));

        private static bool IsComplexPrimaryKey(PropertyInfo propertyInfo, Type entityType)
            => entityType
                .GetPrimaryKeyPropertyInfos()
                .Any(property => property == propertyInfo);

        private static IEnumerable<FormControlViewModel> GeneratePrimitiveFormControls<TEntity>(TEntity entity)
        {
            var entityType = typeof(TEntity);

            var properties = entityType.GetProperties()
                .Where(property => IsPrimitiveProperty(property, entityType)
                                   && !IsComplexPrimaryKey(property, entityType))
                .Select(property => new FormControlViewModel
                {
                    Name = property.Name,
                    Type = property.PropertyType,
                    Value = ExpressionsBuilder.ForGetPropertyValue<TEntity>(property)(entity),
                    IsComplex = false,
                })
                .ToList();

            return properties;
        }

        private IEnumerable<FormControlViewModel> GenerateComplexFormControls<TEntity>(TEntity entity)
        {
            var entityType = typeof(TEntity);

            var primaryKeyProperty = entityType.GetPrimaryKeyPropertyInfos()
                .ToHashSet();
            var result = entityType.GetProperties()
                .Where(property => IsDbContextEntity(property, entity))
                .Select(property => new FormControlViewModel()
                {
                    Name = property.Name,
                    Type = property.PropertyType,
                    Value = this.dbContext.Set(property.PropertyType),
                    IsComplex = true,
                    IsReadOnly = primaryKeyProperty.Contains(property),
                })
                .ToList();

            return result;
        }
    }
}