namespace AutoCrudAdmin.Helpers.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Linq.Expressions;
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
            typeof(int?),
            typeof(short),
            typeof(short?),
            typeof(long),
            typeof(long?),
            typeof(double),
            typeof(double?),
            typeof(decimal),
            typeof(decimal?),
            typeof(bool),
            typeof(bool?),
            typeof(DateTime),
            typeof(DateTime?),
            typeof(TimeSpan),
            typeof(TimeSpan?),
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

        /// <summary>
        /// Generates the form controls for the respective entity.
        /// </summary>
        /// <param name="entity">The entity we want to generate the form controls for.</param>
        /// <param name="entityAction">The action to be performed on the entity.</param>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>An enumerable collection of FormControlViewModel objects.</returns>
        public IEnumerable<FormControlViewModel> GenerateFormControls<TEntity>(TEntity entity, EntityAction entityAction)
            => this.GenerateFormControls(entityAction, entityAction, null, null);

        /// <summary>
        /// Generates the form controls for the respective entity.
        /// </summary>
        /// <param name="entity">The entity we want to generate the form controls for.</param>
        /// <param name="entityAction">The action to be performed on the entity.</param>
        /// <param name="complexOptionFilters">Optional. A dictionary containing complex option filters, based on which we are loading the data for the form control.</param>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>An enumerable collection of FormControlViewModel objects.</returns>
        public IEnumerable<FormControlViewModel> GenerateFormControls<TEntity>(TEntity entity, EntityAction entityAction, IDictionary<string, Expression<Func<object, bool>>> complexOptionFilters)
            => this.GenerateFormControls(entityAction, entityAction, complexOptionFilters, null);

        /// <summary>
        /// Generates the form controls for the respective entity.
        /// </summary>
        /// <param name="entity">The entity we want to generate the form controls for.</param>
        /// <param name="entityAction">The action to be performed on the entity.</param>
        /// <param name="complexOptionFilters">Optional. A dictionary containing complex option filters, based on which we are loading the data for the form control.</param>
        /// <param name="autocompleteType">Optional. The type of the property that will be used for autocomplete functionality.
        /// When passed, values for this property will not be loaded immediately, but when searched for.</param>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>An enumerable collection of FormControlViewModel objects.</returns>
        public IEnumerable<FormControlViewModel> GenerateFormControls<TEntity>(
            TEntity entity,
            EntityAction entityAction,
            IDictionary<string, Expression<Func<object, bool>>>? complexOptionFilters,
            Type autocompleteType)
            => this.GeneratePrimaryKeyFormControls(entity, entityAction, autocompleteType)
                .Concat(GeneratePrimitiveFormControls(entity))
                .Concat(this.GenerateComplexFormControls(entity, entityAction, complexOptionFilters));

        /// <summary>
        /// Gets complex form control name for the provided entity name.
        /// </summary>
        /// <param name="entityName">The entity name.</param>
        /// <returns>The name of the complex form control.</returns>
        public string GetComplexFormControlNameForEntityName(string entityName)
            => entityName + "Id";

        private IEnumerable<FormControlViewModel> GeneratePrimaryKeyFormControls<TEntity>(
            TEntity entity,
            EntityAction entityAction,
            Type autocompleteType)
        {
            var entityType = ReflectionHelper.GetEntityTypeUnproxied<TEntity>();

            var primaryKeyValues = entityType.GetPrimaryKeyValue(entity)
                .ToList();

            if (primaryKeyValues.Count > 1)
            {
                return primaryKeyValues
                    .Select(pair =>
                    {
                        var name = pair.Key[..^2];
                        var property = entityType.GetProperty(name);
                        var value = ExpressionsBuilder.ForGetPropertyValue<TEntity>(
                            entityType.GetProperty(pair.Key))(entity);

                        var isAutocompleteFormcontrol = property.PropertyType == autocompleteType;

                        return new FormControlViewModel
                        {
                            Name = name,
                            Type = property.PropertyType,
                            Value = value,
                            Options = isAutocompleteFormcontrol ? Enumerable.Empty<object>() : this.dbContext.Set(property.PropertyType),
                            IsDbSet = true,
                            IsReadOnly = false,
                        };
                    });
            }

            return primaryKeyValues
                .Select(pair =>
                {
                    var name = pair.Key == SinglePrimaryKeyName
                        ? entityType.GetPrimaryKeyPropertyInfos()
                            .Select(pk => pk.Name)
                            .FirstOrDefault()
                        : pair.Key;

                    var value = pair.Key == SinglePrimaryKeyName
                        ? ExpressionsBuilder.ForGetPropertyValue<TEntity>(
                            entityType.GetPrimaryKeyPropertyInfos().FirstOrDefault())(entity)
                        : ExpressionsBuilder.ForGetPropertyValue<TEntity>(
                            entityType.GetProperty(pair.Key))(entity);

                    return new FormControlViewModel
                    {
                        Name = name!,
                        Type = pair.Value.GetType(),
                        Value = value,
                        IsReadOnly = true,
                    };
                });
        }

        private static IEnumerable<FormControlViewModel> GeneratePrimitiveFormControls<TEntity>(TEntity entity)
        {
            var entityType = ReflectionHelper.GetEntityTypeUnproxied<TEntity>();

            return entityType.GetProperties()
                .Where(p => !p.GetCustomAttributes<NotMappedAttribute>().Any())
                .Where(property => IsPrimitiveProperty(property, entityType)
                                   && !IsComplexPrimaryKey(property, entityType))
                .OrderBy(p => p.MetadataToken)
                .Select(property => new FormControlViewModel
                {
                    Name = property.Name,
                    Type = property.PropertyType,
                    Value = ExpressionsBuilder.ForGetPropertyValue<TEntity>(property)(entity),
                });
        }

        private static bool IsDbContextEntity(PropertyInfo property)
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

        private bool IsPartOfPrimaryKey(PropertyInfo property, Type entityType)
            => entityType.GetPrimaryKeyPropertyInfos()
                .Any(pk => pk.Name == this.GetComplexFormControlNameForEntityName(property.Name));

        private IEnumerable<FormControlViewModel> GenerateComplexFormControls<TEntity>(
            TEntity entity,
            EntityAction entityAction,
            IDictionary<string, Expression<Func<object, bool>>>? optionFilters = null)
        {
            var entityType = ReflectionHelper.GetEntityTypeUnproxied<TEntity>();

            return entityType.GetProperties()
                .Where(property => IsDbContextEntity(property) && !this.IsPartOfPrimaryKey(property, entityType))
                .Select(property =>
                {
                    var filter = optionFilters?.ContainsKey(property.Name) ?? false
                        ? optionFilters[property.Name]
                        : null;
                    return this.GenerateFormControlForComplexProperty(entity, property, entityAction, filter);
                })
                .OrderBy(x => x.Name)
                .ToList();
        }

        private FormControlViewModel GenerateFormControlForComplexProperty<TEntity>(
            TEntity entity,
            PropertyInfo property,
            EntityAction entityAction,
            Expression<Func<object, bool>>? optionsFilter = null)
        {
            var entityType = ReflectionHelper.GetEntityTypeUnproxied<TEntity>();
            var valueFunc = ExpressionsBuilder.ForGetPropertyValue<TEntity>(
                entityType.GetProperty(this.GetComplexFormControlNameForEntityName(property.Name)) !);
            var value = valueFunc(entity);
            var options = this.GetComplexPropertyOptionsForAction(value, property, entityAction, optionsFilter);

            return new FormControlViewModel
            {
                Name = property.Name,
                Type = property.PropertyType,
                Value = value,
                Options = options,
                IsDbSet = true,
                IsReadOnly = false,
            };
        }

        private IQueryable<object> GetComplexPropertyOptionsForAction(
            object? value,
            PropertyInfo property,
            EntityAction entityAction,
            Expression<Func<object, bool>>? optionsFilter = null)
        {
            if (entityAction == EntityAction.Delete)
            {
                var onlyOption = this.dbContext.Find(property.PropertyType, value);

                return onlyOption != default
                    ? new[] { onlyOption }.AsQueryable()
                    : Enumerable.Empty<object>().AsQueryable();
            }

            return optionsFilter != null
                ? this.dbContext.Set(property.PropertyType).Where(optionsFilter)
                : this.dbContext.Set(property.PropertyType);
        }
    }
}