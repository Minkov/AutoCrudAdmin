﻿namespace AutoCrudAdmin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using AutoCrudAdmin.Attributes;
    using AutoCrudAdmin.Extensions;
    using AutoCrudAdmin.Helpers;
    using AutoCrudAdmin.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using NonFactors.Mvc.Grid;

    [AutoCrudAdminControllerNameConvention]
    public class AutoCrudAdminController<TEntity>
        : Controller
        where TEntity : class
    {
        private static readonly Type EntityType = typeof(TEntity);

        private IQueryable<TEntity> set;
        private DbContext db;
        private IFormControlsHelper formControlsHelper;

        private IQueryable<TEntity> Set
            => this.set ??= this.DbContext
                ?.Set<TEntity>();

        protected virtual IEnumerable<string> ShownColumnNames
            => Enumerable.Empty<string>();

        protected virtual IEnumerable<string> HiddenColumnNames
            => Enumerable.Empty<string>();

        protected virtual IEnumerable<Func<TEntity, TEntity, EntityAction, ValidatorResult>> EntityValidators
            => Array.Empty<Func<TEntity, TEntity, EntityAction, ValidatorResult>>();

        protected virtual IEnumerable<Func<TEntity, TEntity, EntityAction, Task<ValidatorResult>>> AsyncEntityValidators
            => Array.Empty<Func<TEntity, TEntity, EntityAction, Task<ValidatorResult>>>();

        protected virtual IEnumerable<GridAction> CustomActions
            => Enumerable.Empty<GridAction>();

        protected virtual IEnumerable<string> DateTimeFormats
            => new string[]
            {
                "dd/MM/yyyy hh:mm",
                "M/dd/yyyy hh:mm:ss tt",
                "M/dd/yyyy h:mm:ss tt",
                "M/d/yyyy h:mm:ss tt",
            };

        protected virtual int RowsPerPage
            => this.PageSizes.Any()
                ? this.PageSizes.First().Item1
                : 20;

        protected virtual bool ShowPageSizes
            => this.PageSizes.Any();

        protected virtual IEnumerable<Tuple<int, string>> PageSizes
            => Enumerable.Empty<Tuple<int, string>>();

        private static MethodInfo GenerateColumnExpressionMethod =>
            typeof(AutoCrudAdminController<TEntity>)
                .GetMethod(
                    nameof(GenerateColumnConfiguration),
                    BindingFlags.NonPublic | BindingFlags.Static);

        private static IEnumerable<GridAction> DefaultActions
            => new[] { new GridAction { Action = nameof(Edit) }, new GridAction { Action = nameof(Delete) }, };

        private IEnumerable<GridAction> Actions
            => DefaultActions.Concat(this.CustomActions);

        private DbContext DbContext
            => this.db ??= this.HttpContext
                .RequestServices
                .GetService<DbContext>();

        private IFormControlsHelper FormControlsHelper
            => this.formControlsHelper ??= this.HttpContext
                .RequestServices
                .GetService<IFormControlsHelper>();

        protected virtual IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> set) => set;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            this.ViewBag.LayoutName = context.HttpContext.Items["layout_name"];
            this.ViewBag.ApplicationName = context.HttpContext.Items["application_name"];
            base.OnActionExecuting(context);
        }

        [HttpGet]
        public virtual IActionResult Index()
            => this.View(
                "../AutoCrudAdmin/Index",
                new AutoCrudAdminIndexViewModel { GenerateGrid = this.GenerateGrid, });

        [HttpGet]
        public virtual IActionResult Create()
            => this.GetEntityForm(
                ExpressionsBuilder.ForCreateInstance<TEntity>()(),
                EntityAction.Create);

        [HttpGet]
        public virtual IActionResult Edit([FromQuery] IDictionary<string, string> complexId)
            => this.GetEntityForm(
                this.Set
                    .FirstOrDefault(ExpressionsBuilder.ForByEntityPrimaryKey<TEntity>(complexId)),
                EntityAction.Edit,
                complexId);

        [HttpGet]
        public virtual IActionResult Delete([FromQuery] IDictionary<string, string> complexId)
            => this.GetEntityForm(
                this.Set
                    .FirstOrDefault(ExpressionsBuilder.ForByEntityPrimaryKey<TEntity>(complexId)),
                EntityAction.Delete,
                complexId);

        [HttpPost]
        public virtual Task<IActionResult> PostCreate(IDictionary<string, string> entityDict)
            => this.PostEntityForm(entityDict, EntityAction.Create);

        [HttpPost]
        public virtual Task<IActionResult> PostEdit(IDictionary<string, string> entityDict)
            => this.PostEntityForm(entityDict, EntityAction.Edit);

        [HttpPost]
        public virtual Task<IActionResult> PostDelete(IDictionary<string, string> entityDict)
            => this.PostEntityForm(entityDict, EntityAction.Delete);

        protected virtual IActionResult GetEntityForm(
            TEntity entity,
            EntityAction action,
            IDictionary<string, string> complexId = null)
        {
            var formControls = this.GenerateFormControls(entity, action).ToList();

            if (action == EntityAction.Delete)
            {
                formControls.ForEach(fc => fc.IsReadOnly = true);
            }

            return this.View(
                "../AutoCrudAdmin/EntityForm",
                new AutoCrudAdminEntityFormViewModel { FormControls = formControls, Action = action, });
        }

        protected virtual IEnumerable<FormControlViewModel> GenerateFormControls(TEntity entity, EntityAction action)
            => this.FormControlsHelper.GenerateFormControls(entity, action);

        protected virtual async Task<IActionResult> PostEntityForm(
            IDictionary<string, string> entityDict,
            EntityAction action)
        {
            var (originalEntity, newEntity) = this.GetEntitiesForAction(action, entityDict);

            await this.ValidateBeforeSave(originalEntity, newEntity, action);

            switch (action)
            {
                case EntityAction.Create:
                    await this.BeforeEntitySaveOnCreateAsync(newEntity, entityDict);
                    await this.DbContext.AddAsync(newEntity);
                    await this.DbContext.SaveChangesAsync();
                    await this.AfterEntitySaveOnCreateAsync(newEntity, entityDict);
                    break;
                case EntityAction.Edit:
                    await this.BeforeEntitySaveOnEditAsync(originalEntity, newEntity, entityDict);
                    this.DbContext.Update(newEntity);
                    await this.DbContext.SaveChangesAsync();
                    await this.AfterEntitySaveOnEditAsync(originalEntity, newEntity, entityDict);
                    break;
                case EntityAction.Delete:
                    await this.BeforeEntitySaveOnDeleteAsync(originalEntity, entityDict);
                    this.DbContext.Remove(originalEntity);
                    await this.DbContext.SaveChangesAsync();
                    await this.AfterEntitySaveOnDeleteAsync(originalEntity, entityDict);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }

            return this.RedirectToAction("Index");
        }

        protected virtual Task BeforeEntitySaveOnCreateAsync(TEntity entity, IDictionary<string, string> entityDict)
            => Task.CompletedTask;

        protected virtual Task BeforeEntitySaveOnDeleteAsync(TEntity entity, IDictionary<string, string> entityDict)
            => Task.CompletedTask;

        protected virtual Task BeforeEntitySaveOnEditAsync(TEntity existingEntity, TEntity newEntity, IDictionary<string, string> entityDict)
            => Task.CompletedTask;

        protected virtual Task AfterEntitySaveOnCreateAsync(TEntity entity, IDictionary<string, string> entityDict)
            => Task.CompletedTask;

        protected virtual Task AfterEntitySaveOnEditAsync(TEntity oldEntity, TEntity entity, IDictionary<string, string> entityDict)
            => Task.CompletedTask;

        protected virtual Task AfterEntitySaveOnDeleteAsync(TEntity entity, IDictionary<string, string> entityDict)
            => Task.CompletedTask;

        protected virtual IHtmlGrid<TEntity> GenerateGrid(IHtmlHelper<AutoCrudAdminIndexViewModel> htmlHelper)
        {
            var types = ReflectionHelper.DbSetProperties
                .Select(p => p.PropertyType)
                .Select(t => t.GetGenericArguments())
                .Select(a => a.FirstOrDefault())
                .ToHashSet();

            var names = this.Set.GetType()
                .GetGenericArguments()
                .FirstOrDefault()
                ?.GetProperties()
                .Where(prop => types.Contains(prop.PropertyType))
                .Select(prop => prop.Name);

            var setForGrid = names
                !.Aggregate(this.Set, (current, name) => current.Include(name));

            setForGrid = this.ApplyIncludes(setForGrid);

            return htmlHelper
                .Grid(setForGrid)
                .Build(columns =>
                {
                    this.BuildGridColumns(columns);
                    this.BuildGridActions(columns, htmlHelper);
                })
                .Using(GridFilterMode.Header)
                .Empty("No data found")
                .Pageable(pager =>
                {
                    pager.PageSizes = this.PageSizes.ToDictionary(
                        x => x.Item1,
                        y => y.Item2);
                    pager.ShowPageSizes = this.ShowPageSizes;
                    pager.RowsPerPage = this.RowsPerPage;
                });
        }

        protected virtual IGridColumnsOf<TEntity> BuildGridColumns(IGridColumnsOf<TEntity> columns)
        {
            if (this.ShownColumnNames.Any() && this.HiddenColumnNames.Any())
            {
                throw new Exception("Both shown and hidden column names are declared. Leave only one of them");
            }

            Func<PropertyInfo, bool> filter = this.ShownColumnNames.Any()
                ? x => this.ShownColumnNames.Contains(x.Name)
                : x => !this.HiddenColumnNames.Contains(x.Name) &&
                    x.PropertyType.IsNotEnumerable() &&
                    !x.GetCustomAttributes<NotMappedAttribute>().Any();

            var primaryKeys = EntityType.GetPrimaryKeyPropertyInfos();

            var properties = EntityType
                .GetProperties()
                .Where(filter)
                .OrderBy(property => property != primaryKeys.FirstOrDefault());

            properties = primaryKeys.Skip(1)
                .Aggregate(
                    properties,
                    (current, pk)
                        => current.ThenBy(property => property != pk))
                .ThenBy(property => property.Name);

            return properties
                .Aggregate(
                    columns,
                    (currentColumns, prop) => (IGridColumnsOf<TEntity>)GenerateColumnExpressionMethod
                        .MakeGenericMethod(prop.PropertyType)
                        .Invoke(null, new object[] { currentColumns, prop }));
        }

        protected virtual IGridColumnsOf<TEntity> BuildGridActions(
            IGridColumnsOf<TEntity> columns,
            IHtmlHelper htmlHelper)
        {
            this.Actions
                .ToList()
                .ForEach(action =>
                {
                    columns.Add(model => htmlHelper.ActionLink(
                            action.Name,
                            action.Action,
                            this.RouteData.Values["controller"].ToString(),
                            RouteValueDictionary.FromArray(
                                EntityType.GetPrimaryKeyValue(model).ToArray()),
                            new { }))
                        .Titled("Action");
                });

            return columns;
        }

        protected virtual DateTime ParseDateTime(string dateTimeStr)
        {
            foreach (var dateTimeFormat in this.DateTimeFormats)
            {
                if (DateTime.TryParseExact(
                    dateTimeStr,
                    dateTimeFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var dateTime))
                {
                    return dateTime;
                }
            }

            throw new ArgumentException(
                $"Cannot parse string date \"{dateTimeStr}\" to DateTime. Try adding another format template.");
        }

        private static IGridColumnsOf<TEntity> GenerateColumnConfiguration<TProperty>(
            IGridColumnsOf<TEntity> columns,
            MemberInfo property)
        {
            var lambda = ExpressionsBuilder.ForGetProperty<TEntity, TProperty>(property);
            columns
                .Add(lambda)
                .Titled(property.Name)
                .Filterable(true)
                .Sortable(true);

            return columns;
        }

        private static void CopyFormPropertiesToExistingEntityFromNewEntity(TEntity existingEntity, TEntity newEntity)
        {
            foreach (PropertyInfo property in typeof(TEntity)
                .GetProperties()
                .Where(p => p.CanWrite &&
                    !p.PropertyType.IsClass &&
                    p.PropertyType.IsNotEnumerable()))
            {
                property.SetValue(existingEntity, property.GetValue(newEntity, null), null);
            }
        }

        private TEntity DictToEntity(IDictionary<string, string> entityDict)
        {
            var entity = Activator.CreateInstance<TEntity>();
            EntityType.GetProperties()
                .Where(prop => prop.CanWrite)
                .ToList()
                .ForEach(prop =>
                {
                    if (!entityDict.ContainsKey(prop.Name))
                    {
                        return;
                    }

                    var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                    var strValue = entityDict[prop.Name];
                    object safeValue;

                    if (strValue == null)
                    {
                        safeValue = null;
                    }
                    else if (propType.IsEnum)
                    {
                        safeValue = Enum.Parse(propType, strValue);
                    }
                    else if (propType == typeof(DateTime))
                    {
                        safeValue = this.ParseDateTime(strValue);
                    }
                    else if (propType == typeof(TimeSpan))
                    {
                        safeValue = TimeSpan.Parse(strValue, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        safeValue = Convert.ChangeType(strValue, propType);
                    }

                    prop.SetValue(entity, safeValue);
                });

            return entity;
        }

        private async Task ValidateBeforeSave(TEntity existingEntity, TEntity newEntity, EntityAction action)
        {
            var errors = this.GetValidatorResults(existingEntity, newEntity, action)
                .Concat(await this.GetAsyncValidatorResults(existingEntity, newEntity, action))
                .Where(x => !x.IsValid)
                .Select(x => x.Message)
                .ToList();

            if (errors.Any())
            {
                throw new Exception(string.Join(", ", errors));
            }
        }

        private IEnumerable<ValidatorResult> GetValidatorResults(
            TEntity existingEntity,
            TEntity newEntity,
            EntityAction action)
            => this.EntityValidators
                .Select(v => v(existingEntity, newEntity, action));

        private async Task<IEnumerable<ValidatorResult>> GetAsyncValidatorResults(
            TEntity existingEntity,
            TEntity newEntity,
            EntityAction action)
        {
            var resultTasks = this.AsyncEntityValidators
                .Select(v => v(existingEntity, newEntity, action))
                .ToList();

            await Task.WhenAll(resultTasks);

            return resultTasks.Select(r => r.Result);
        }

        private (TEntity originalEntity, TEntity newEntity) GetEntitiesForAction(
            EntityAction action,
            IDictionary<string, string> entityDict)
        {
            var newEntity = this.DictToEntity(entityDict);

            if (action is EntityAction.Create)
            {
                // No original entity here.
                return (null, newEntity);
            }

            var originalEntity = this.GetExistingEntity(newEntity);

            if (action is EntityAction.Delete)
            {
                return (originalEntity, newEntity);
            }

            // Detach original entity, so ChangeTracker does not track two instances of the same object.
            this.DbContext.Entry(originalEntity).State = EntityState.Detached;

            // Get the existing entity again, to which the new properties will be copied.
            // We want to preserve the original entity as it is.
            var existingEntity = this.GetExistingEntity(originalEntity);

            // Copy properties from new entity to the existing entity.
            CopyFormPropertiesToExistingEntityFromNewEntity(existingEntity, newEntity);

            // Return original entity and modified existing entity.
            // This approach allows for lazy loading to work on the new entity.
            return (originalEntity, existingEntity);
        }

        private TEntity GetExistingEntity(TEntity newEntity)
        {
            var entityType = typeof(TEntity);
            var keyValues = entityType.GetPrimaryKeyValue(newEntity).Select(x => x.Value).ToArray();
            var existingEntity = (TEntity)this.DbContext.Find(entityType, keyValues);

            if (existingEntity == null)
            {
                throw new Exception(
                    "The Action cannot be performed, because the original entity was not found in the db.");
            }

            return existingEntity;
        }
    }
}