namespace AutoCrudAdmin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using AutoCrudAdmin.Attributes;
    using AutoCrudAdmin.Extensions;
    using AutoCrudAdmin.Helpers;
    using AutoCrudAdmin.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using NonFactors.Mvc.Grid;

    [AutoCrudAdminControllerNameConvention]
    public class AutoCrudAdminController<TEntity>
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
            typeof(AutoCrudAdminController<TEntity>)
                .GetMethod(nameof(GenerateColumnConfiguration),
                    BindingFlags.NonPublic | BindingFlags.Static);

        private IQueryable<TEntity> set;
        private DbContext db;
        private IFormControlsHelper formControlsHelper;

        protected virtual IEnumerable<string> ShownColumnNames
            => Enumerable.Empty<string>();

        protected virtual IEnumerable<string> HiddenColumnNames
            => Enumerable.Empty<string>();

        protected virtual IEnumerable<Func<TEntity, ValidatorResult>> EntityValidators
            => Array.Empty<Func<TEntity, ValidatorResult>>();

        protected virtual IEnumerable<GridAction> CustomActions
            => Enumerable.Empty<GridAction>();

        protected virtual int RowsPerPage
            => this.PageSizes.Any()
                ? this.PageSizes.First().Item1
                : 20;

        protected virtual bool ShowPageSizes
            => this.PageSizes.Any();

        protected virtual IEnumerable<Tuple<int, string>> PageSizes
            => Enumerable.Empty<Tuple<int, string>>();

        private static IEnumerable<GridAction> DefaultActions
            => new[]
            {
                new GridAction { Action = nameof(Edit) },
                new GridAction { Action = nameof(Delete) },
            };

        private IEnumerable<GridAction> Actions
            => DefaultActions.Concat(this.CustomActions);

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            this.ViewBag.LayoutName = context.HttpContext.Items["layout_name"];
            this.ViewBag.ApplicationName = context.HttpContext.Items["application_name"];
            base.OnActionExecuting(context);
        }

        [HttpGet]
        public virtual IActionResult Index()
            => this.View("../AutoCrudAdmin/Index", new AutoCrudAdminIndexViewModel
            {
                GenerateGrid = this.GenerateGrid,
            });

        [HttpGet]
        public virtual IActionResult Create()
            => this.GetEntityForm(
                ExpressionsBuilder.ForCreateInstance<TEntity>()(),
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

            return this.View("../AutoCrudAdmin/EntityForm", new AutoCrudAdminEntityFormViewModel
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

        protected virtual IHtmlGrid<TEntity> GenerateGrid(IHtmlHelper<AutoCrudAdminIndexViewModel> htmlHelper)
            => htmlHelper
                .Grid(this.Set)
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

        protected virtual IGridColumnsOf<TEntity> BuildGridColumns(IGridColumnsOf<TEntity> columns)
        {
            if (this.ShownColumnNames.Any() && this.HiddenColumnNames.Any())
            {
                throw new Exception("Both shown and hidden column names are declared. Leave only one of them");
            }

            Func<PropertyInfo, bool> filter = this.ShownColumnNames.Any()
                ? x => this.ShownColumnNames.Contains(x.Name)
                : this.HiddenColumnNames.Any()
                    ? x => !this.HiddenColumnNames.Contains(x.Name)
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
                        ))
                        .Titled("Action");
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