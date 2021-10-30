namespace GenericDotNetCoreAdmin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using GenericDotNetCoreAdmin.Attributes;
    using GenericDotNetCoreAdmin.Extensions;
    using GenericDotNetCoreAdmin.Helpers;
    using GenericDotNetCoreAdmin.Helpers.Implementations;
    using GenericDotNetCoreAdmin.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using NonFactors.Mvc.Grid;

    public enum EntityAction
    {
        Create = 1,
        Edit = 2,
        Delete = 3,
    }

    [GenericAdminControllerNameConvention]
    public class GenericAdminController<TEntity>
        : Controller
        where TEntity : class
    {
        private static readonly Type EntityType = typeof(TEntity);

        private DbContext DbContext
            => this.db ??= this.HttpContext
                .RequestServices
                .GetService<DbContext>();

        protected virtual IQueryable<TEntity> Set
            => this.set ??= this.DbContext
                ?.Set<TEntity>();

        private IFormControlsHelper FormControlsHelper
            => this.formControlsHelper ??= this.HttpContext
                .RequestServices
                .GetService<IFormControlsHelper>();

        private static MethodInfo GenerateColumnExpressionMethod =>
            typeof(GenericAdminController<TEntity>)
                .GetMethod(nameof(GenerateColumnConfiguration),
                    BindingFlags.NonPublic | BindingFlags.Static);

        private IQueryable<TEntity> set;
        private DbContext db;
        private IFormControlsHelper formControlsHelper;

        protected virtual IEnumerable<string> ColumnNames
            => Enumerable.Empty<string>();

        protected virtual IEnumerable<Func<TEntity, ValidatorResult>> EntityValidators
            => Array.Empty<Func<TEntity, ValidatorResult>>();

        protected virtual IEnumerable<GridAction> CustomActions
            => Enumerable.Empty<GridAction>();

        private static IEnumerable<GridAction> DefaultActions
            => new[]
            {
                new GridAction { Action = nameof(Edit) },
                new GridAction { Action = nameof(Delete) },
            };

        private IEnumerable<GridAction> Actions
            => DefaultActions.Concat(this.CustomActions);

        [HttpGet]
        public virtual IActionResult Index()
            => this.View("../GenericAdmin/Index", new GenericAdminIndexViewModel
            {
                GenerateGrid = this.GenerateGrid,
            });

        [HttpGet]
        public virtual IActionResult Create()
            => this.GetEntityForm(
                Activator.CreateInstance<TEntity>(),
                EntityAction.Create);

        [HttpGet]
        public virtual IActionResult Edit(string id)
            => this.GetEntityForm(this.Set
                    .FirstOrDefault(ExpressionsBuilder.ForByEntityId<TEntity>(id)),
                EntityAction.Edit);

        [HttpGet]
        public virtual IActionResult Delete(string id)
            => this.GetEntityForm(this.Set
                    .FirstOrDefault(ExpressionsBuilder.ForByEntityId<TEntity>(id)),
                EntityAction.Delete);

        [HttpPost]
        public virtual IActionResult Create(TEntity entity)
            => this.PostEntityForm(entity, EntityAction.Create);

        [HttpPost]
        public virtual IActionResult Edit(TEntity entity)
            => this.PostEntityForm(entity, EntityAction.Edit);

        [HttpPost]
        public virtual IActionResult Delete(TEntity entity)
            => this.PostEntityForm(entity, EntityAction.Delete);

        protected virtual IActionResult GetEntityForm(TEntity entity, EntityAction action)
        {
            var formControls = this.FormControlsHelper.GenerateFormControls(entity)
                .ToList();

            if (action == EntityAction.Delete)
            {
                formControls.ForEach(fc => fc.IsReadOnly = true);
            }

            return this.View("../GenericAdmin/EntityForm", new GenericAdminEntityFormViewModel
            {
                FormControls = formControls,
                Action = action,
            });
        }

        protected virtual IActionResult PostEntityForm(TEntity entity, EntityAction action)
        {
            this.ValidateBeforeSave(entity);
            this.DbContext.Entry(entity).State = action switch
            {
                EntityAction.Create => EntityState.Added,
                EntityAction.Edit => EntityState.Modified,
                var _ => EntityState.Deleted
            };

            this.DbContext.SaveChanges();

            return this.RedirectToAction("Index");
        }

        protected virtual IHtmlGrid<TEntity> GenerateGrid(IHtmlHelper<GenericAdminIndexViewModel> htmlHelper)
            => htmlHelper
                .Grid(this.Set)
                .Build(columns =>
                {
                    this.BuildGridColumns(columns);
                    this.BuildGridActions(columns, htmlHelper);
                })
                .Using(GridFilterMode.Header)
                .Empty("No data found")
                .Pageable();

        protected virtual IGridColumnsOf<TEntity> BuildGridColumns(IGridColumnsOf<TEntity> columns)
        {
            Func<PropertyInfo, bool> filter = this.ColumnNames.Any()
                ? x => this.ColumnNames.Contains(x.Name)
                : _ => true;

            return EntityType
                .GetProperties()
                .Where(filter)
                .Aggregate(columns, (currentColumns, prop) => (IGridColumnsOf<TEntity>)
                    GenerateColumnExpressionMethod
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
                        new
                        {
                            id = EntityType.GetPrimaryKeyValue(model),
                        },
                        new { }
                    ));
                });

            return columns;
        }

        private void ValidateBeforeSave(TEntity entity)
        {
            var errors = this.EntityValidators
                .Select(v => v(entity))
                .Where(x => !x.IsValid)
                .Select(x => x.Message)
                .ToList();

            if (errors.Any())
            {
                throw new Exception(string.Join(", ", errors));
            }
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
    }
}