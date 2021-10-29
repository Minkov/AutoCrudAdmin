namespace GenericDotNetCoreAdmin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using GenericDotNetCoreAdmin.Attributes;
    using GenericDotNetCoreAdmin.Extensions;
    using GenericDotNetCoreAdmin.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using NonFactors.Mvc.Grid;

    [GenericAdminControllerNameConvention]
    public class GenericAdminController<TEntity>
        : Controller
        where TEntity : class
    {
        private static readonly Type EntityType = typeof(TEntity);

        private static readonly MethodInfo GenerateColumnExpressionMethod = typeof(GenericAdminController<TEntity>)
            .GetMethod(nameof(GenerateColumnConfiguration), BindingFlags.NonPublic | BindingFlags.Static);

        private DbContext DbContext
            => this.HttpContext
                .RequestServices
                .GetService<DbContext>();

        private IQueryable<TEntity> set;

        protected virtual IEnumerable<string> ColumnNames
            => Enumerable.Empty<string>();

        protected virtual IEnumerable<Func<TEntity, ValidatorResult>> EntityValidators
            => Array.Empty<Func<TEntity, ValidatorResult>>();

        protected virtual IQueryable<TEntity> Set
            => this.set ??= this.DbContext
                ?.Set<TEntity>();

        [HttpGet]
        public virtual IActionResult Index()
            => this.View("../GenericAdmin/Index", new GenericAdminIndexViewModel
            {
                GenerateGrid = this.GenerateGrid
            });

        [HttpGet]
        public virtual IActionResult Create()
            => this.GetEntityForm(Activator.CreateInstance<TEntity>());

        [HttpGet]
        public virtual IActionResult Edit(string id)
            => this.GetEntityForm(this.Set
                .FirstOrDefault(this.GetObjectByIdLambda(id)));

        [HttpPost]
        public virtual IActionResult Create(TEntity entity)
            => this.PostEntityForm(entity, EntityState.Added);

        [HttpPost]
        public virtual IActionResult Edit(TEntity entity)
            => this.PostEntityForm(entity, EntityState.Modified);

        protected virtual IActionResult GetEntityForm(TEntity entity)
            => this.View("../GenericAdmin/EntityForm", new GenericAdminEntityFormViewModel
            {
                FormControls = GenerateFormControls(entity)
            });

        protected virtual IActionResult PostEntityForm(TEntity entity, EntityState state)
        {
            this.ValidateBeforeSave(entity);
            // EntityType.GetPrimaryKeyPropertyInfo().SetValue(entity, 0);
            this.DbContext.Entry(entity).State = state;
            this.DbContext.SaveChanges();

            return this.RedirectToAction("Index");
        }

        private static IEnumerable<FormControlViewModel> GenerateFormControls(TEntity entity)
        {
            var primaryKeyProperty = EntityType.GetPrimaryKeyPropertyInfo();
            return EntityType.GetProperties()
                .Select(property => new FormControlViewModel
                {
                    Name = property.Name,
                    Type = property.PropertyType,
                    Value = property.GetValue(entity),
                    IsReadOnly = property == primaryKeyProperty,
                });
        }

        protected virtual IHtmlGrid<TEntity> GenerateGrid(IHtmlHelper<GenericAdminIndexViewModel> htmlHelper)
            => htmlHelper
                .Grid(this.Set)
                .Build(columns => this.BuildGridColumns(columns))
                .Using(GridFilterMode.Header)
                .Empty("No data found")
                .Pageable();

        protected virtual void BuildGridColumns(IGridColumnsOf<TEntity> columns)
        {
            Func<PropertyInfo, bool> filter = this.ColumnNames.Any()
                ? x => this.ColumnNames.Contains(x.Name)
                : x => true;

            EntityType
                .GetProperties()
                .Where(filter)
                .Aggregate(columns, (currentColumns, prop) => (IGridColumnsOf<TEntity>)GenerateColumnExpressionMethod
                    .MakeGenericMethod(prop.PropertyType)
                    .Invoke(null, new object[] { currentColumns, prop }));
        }

        // This method is called via reflection.
        private static IGridColumnsOf<TEntity> GenerateColumnConfiguration<TProperty>(
            IGridColumnsOf<TEntity> columns,
            PropertyInfo property)
        {
            var parameter = Expression.Parameter(EntityType, "model");

            // model.Column
            var memberAccess = Expression.MakeMemberAccess(
                parameter,
                property);

            // model => model.Column
            var lambda = Expression.Lambda<Func<TEntity, TProperty>>(memberAccess, parameter);

            columns
                .Add(lambda)
                .Titled(property.Name)
                .Filterable(true)
                .Sortable(true);

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

        private Expression<Func<TEntity, bool>> GetObjectByIdLambda(object entityId)
        {
            var primaryKeyProperty = EntityType
                .GetPrimaryKeyPropertyInfo();
            var parameter = Expression.Parameter(typeof(object), "model");
            var convertedParameter = Expression.Convert(
                parameter,
                EntityType
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
    }
}