namespace AutoCrudAdmin.Controllers;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AutoCrudAdmin.Attributes;
using AutoCrudAdmin.Enumerations;
using AutoCrudAdmin.Extensions;
using AutoCrudAdmin.Helpers;
using AutoCrudAdmin.Models;
using AutoCrudAdmin.ViewModels;
using AutoCrudAdmin.ViewModels.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NonFactors.Mvc.Grid;
using static Constants;

/// <summary>
/// The AutoCrudAdminController provides default CRUD operations for entities.
/// </summary>
/// <typeparam name="TEntity">The type of the entity that this controller operates on.</typeparam>
[AutoCrudAdminControllerNameConvention]
public class AutoCrudAdminController<TEntity>
    : Controller
    where TEntity : class
{
    private static readonly Type EntityType = typeof(TEntity);

    private IQueryable<TEntity>? set;
    private DbContext? db;
    private IFormControlsHelper? formControlsHelper;

    /// <summary>
    /// Gets the column names to show in the index grid view.
    /// </summary>
    protected virtual IEnumerable<string> ShownColumnNames
        => Enumerable.Empty<string>();

    /// <summary>
    /// Gets the column names to hide in the index grid view.
    /// </summary>
    protected virtual IEnumerable<string> HiddenColumnNames
        => Enumerable.Empty<string>();

    /// <summary>
    /// Gets the form control names to show by default.
    /// </summary>
    protected virtual IEnumerable<string> ShownFormControlNames
        => Enumerable.Empty<string>();

    /// <summary>
    /// Gets the form control names to show on create forms.
    /// </summary>
    protected virtual IEnumerable<string> ShownFormControlNamesOnCreate
        => Enumerable.Empty<string>();

    /// <summary>
    /// Gets the form control names to show on edit forms.
    /// </summary>
    protected virtual IEnumerable<string> ShownFormControlNamesOnEdit
        => Enumerable.Empty<string>();

    /// <summary>
    /// Gets the form control names to hide by default.
    /// </summary>
    protected virtual IEnumerable<string> HiddenFormControlNames
        => Enumerable.Empty<string>();

    /// <summary>
    /// Gets the form control names to hide on create forms.
    /// </summary>
    protected virtual IEnumerable<string> HiddenFormControlNamesOnCreate
        => Enumerable.Empty<string>();

    /// <summary>
    /// Gets the form control names to hide on edit forms.
    /// </summary>
    protected virtual IEnumerable<string> HiddenFormControlNamesOnEdit
        => Enumerable.Empty<string>();

    /// <summary>
    /// Gets the validators to run on entity save.
    /// </summary>
    protected virtual IEnumerable<Func<TEntity, TEntity, AdminActionContext, ValidatorResult>> EntityValidators
        => Array.Empty<Func<TEntity, TEntity, AdminActionContext, ValidatorResult>>();

    /// <summary>
    /// Gets the async validators to run on entity save.
    /// </summary>
    protected virtual IEnumerable<Func<TEntity, TEntity, AdminActionContext, Task<ValidatorResult>>> AsyncEntityValidators
        => Array.Empty<Func<TEntity, TEntity, AdminActionContext, Task<ValidatorResult>>>();

    /// <summary>
    /// Gets the default grid actions.
    /// </summary>
    protected virtual IEnumerable<GridAction> DefaultActions
        => new[]
        {
            new GridAction { Action = nameof(this.Edit) },
            new GridAction { Action = nameof(this.Delete) },
        };

    /// <summary>
    /// Gets any custom grid actions.
    /// </summary>
    protected virtual IEnumerable<GridAction> CustomActions
        => Enumerable.Empty<GridAction>();

    /// <summary>
    /// Gets any custom toolbar actions for the index grid.
    /// </summary>
    protected virtual IEnumerable<AutoCrudAdminGridToolbarActionViewModel> CustomToolbarActions
        => Enumerable.Empty<AutoCrudAdminGridToolbarActionViewModel>();

    /// <summary>
    /// Gets any custom grid columns.
    /// </summary>
    protected virtual IEnumerable<CustomGridColumn<TEntity>> CustomColumns
        => Enumerable.Empty<CustomGridColumn<TEntity>>();

    /// <summary>
    /// Gets the supported date time formats.
    /// </summary>
    protected virtual IEnumerable<string> DateTimeFormats
        => new[]
        {
            "d/M/yyyy h:mm tt",
            "d/M/yyyy H:mm tt",
            "dd/MM/yyyy hh:mm",
        };

    /// <summary>
    /// Gets the default rows per page.
    /// </summary>
    protected virtual int RowsPerPage
        => this.PageSizes.Any()
            ? this.PageSizes.First().Item1
            : 20;

    /// <summary>
    /// Gets a value indicating whether to show page sizes dropdown.
    /// By default, this depends on the size of `PageSizes` property.
    /// </summary>
    protected virtual bool ShowPageSizes
        => this.PageSizes.Any();

    /// <summary>
    /// Gets the page size options.
    /// </summary>
    protected virtual IEnumerable<Tuple<int, string>> PageSizes
        => Enumerable.Empty<Tuple<int, string>>();

    /// <summary>
    /// Gets the max string column length.
    /// </summary>
    protected virtual int ColumnStringMaxLength => Grid.DefaultColumnStringMaxLength;

    /// <summary>
    /// Gets generators for default dropdown options.
    /// </summary>
    protected virtual IDictionary<Type, Func<object>> DefaultOptionsGenerators
        => new Dictionary<Type, Func<object>>();

    private static MethodInfo GenerateColumnExpressionMethod =>
        typeof(AutoCrudAdminController<TEntity>)
            .GetMethod(
                nameof(GenerateColumnConfiguration),
                BindingFlags.NonPublic | BindingFlags.Static) !;

#pragma warning disable CA1822
    private Expression<Func<TEntity, bool>>? MasterGridFilter
        => null;
#pragma warning restore CA1822

    private IEnumerable<GridAction> Actions
        => this.DefaultActions.Concat(this.CustomActions);

    private DbContext DbContext
        => this.db ??= this.HttpContext
            .RequestServices
            .GetRequiredService<DbContext>();

    private IQueryable<TEntity> Set
        => this.set ??= this.DbContext
            .Set<TEntity>();

    private IFormControlsHelper FormControlsHelper
        => this.formControlsHelper ??= this.HttpContext
            .RequestServices
            .GetRequiredService<IFormControlsHelper>();

    /// <summary>
    /// Called when an action is executing. Sets layout and app name.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        this.ViewBag.LayoutName = context.HttpContext.Items["layout_name"] ?? string.Empty;
        this.ViewBag.ApplicationName = context.HttpContext.Items["application_name"] ?? string.Empty;
        base.OnActionExecuting(context);
    }

    /// <summary>
    /// Handles autocomplete searches.
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    /// <param name="searchProperty">The property to search.</param>
    /// <returns>The autocomplete results.</returns>
    [HttpGet]
    public virtual IEnumerable<DropDownViewModel> Autocomplete([FromQuery] string searchTerm, string searchProperty)
    {
        var entityType = typeof(TEntity);
        var searchedProperty = entityType.GetProperty(searchProperty);
        var idProperty = entityType.GetProperty("Id");

        if (searchedProperty == null || idProperty == null)
        {
            throw new ArgumentException("No such property exists on the entity!");
        }

        var entities = this.Set
            .AsNoTracking()
            .Where(e => EF.Property<string>(e, searchProperty).Contains(searchTerm))
            .Select(x => new DropDownViewModel
            {
                Value = idProperty.GetValue(x) !.ToString() !,
                Name = searchedProperty.GetValue(x) !.ToString() !,
            })
            .Take(20)
            .ToList();

        return entities;
    }

    /// <summary>
    /// Shows the index view. Contains a grid with paging, sorting and filtering.
    /// </summary>
    /// <returns>The index view.</returns>
    [HttpGet]
    public virtual IActionResult Index()
        => this.View(
            "../AutoCrudAdmin/Index",
            new AutoCrudAdminIndexViewModel
            {
                GenerateGrid = this.GenerateGrid,
                ToolbarActions = this.CustomToolbarActions,
            });

    /// <summary>
    /// Shows the create form.
    /// </summary>
    /// <param name="complexId">Relations for complex properties.</param>
    /// <param name="postEndpointName">Optional custom post endpoint name.</param>
    /// <returns>The create form view.</returns>
    [HttpGet]
    public virtual async Task<IActionResult> Create(
        [FromQuery] IDictionary<string, string> complexId,
        string? postEndpointName)
        => await this.GetEntityForm(
            ExpressionsBuilder.ForCreateInstance<TEntity>()(),
            EntityAction.Create,
            complexId,
            postEndpointName);

    /// <summary>
    /// Shows the edit form.
    /// </summary>
    /// <param name="complexId">Relations for complex properties.</param>
    /// <param name="postEndpointName">Optional custom post endpoint name.</param>
    /// <returns>The edit form view.</returns>
    [HttpGet]
    public virtual async Task<IActionResult> Edit(
        [FromQuery] IDictionary<string, string> complexId,
        string? postEndpointName)
        => await this.GetEntityForm(
            this.Set.First(ExpressionsBuilder.ForByEntityPrimaryKey<TEntity>(complexId)),
            EntityAction.Edit,
            complexId,
            postEndpointName);

    /// <summary>
    /// Shows the delete confirmation form.
    /// </summary>
    /// <param name="complexId">Relations for complex properties.</param>
    /// <param name="postEndpointName">Optional custom post endpoint name.</param>
    /// <returns>The delete confirmation form view.</returns>
    [HttpGet]
    public virtual async Task<IActionResult> Delete(
        [FromQuery] IDictionary<string, string> complexId,
        string? postEndpointName)
        => await this.GetEntityForm(
            this.Set.First(ExpressionsBuilder.ForByEntityPrimaryKey<TEntity>(complexId)),
            EntityAction.Delete,
            complexId,
            postEndpointName);

    /// <summary>
    /// Handles HTTP POST for entity create.
    /// </summary>
    /// <param name="entityDict">The entity values.</param>
    /// <param name="files">Any uploaded files.</param>
    /// <returns>A redirect to index after save.</returns>
    [HttpPost]
    public virtual Task<IActionResult> PostCreate(IDictionary<string, string> entityDict, FormFilesContainer files)
        => this.PostEntityForm(entityDict, EntityAction.Create, files);

    /// <summary>
    /// Handles HTTP POST for entity edit.
    /// </summary>
    /// <param name="entityDict">The entity values.</param>
    /// <param name="files">Any uploaded files.</param>
    /// <returns>A redirect to index after save.</returns>
    [HttpPost]
    public virtual Task<IActionResult> PostEdit(IDictionary<string, string> entityDict, FormFilesContainer files)
        => this.PostEntityForm(entityDict, EntityAction.Edit, files);

    /// <summary>
    /// Handles HTTP POST for entity delete.
    /// </summary>
    /// <param name="entityDict">The entity values.</param>
    /// <param name="files">Any uploaded files.</param>
    /// <returns>A redirect to index after delete.</returns>
    [HttpPost]
    public virtual Task<IActionResult> PostDelete(IDictionary<string, string> entityDict, FormFilesContainer files)
        => this.PostEntityForm(entityDict, EntityAction.Delete, files);

    /// <summary>
    /// Applies eager loading / explicit includes.
    /// </summary>
    /// <param name="queryable">The base query.</param>
    /// <returns>Query with includes applied.</returns>
    protected virtual IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> queryable) => queryable;

    /// <summary>
    /// Shows the form for an entity action.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="action">The action.</param>
    /// <param name="entityDict">The entity dictionary.</param>
    /// <param name="postEndpointName">Optional custom post endpoint.</param>
    /// <returns>The entity form view.</returns>
    protected virtual async Task<IActionResult> GetEntityForm(
        TEntity entity,
        EntityAction action,
        IDictionary<string, string> entityDict,
        string? postEndpointName = null)
    {
        await this.BeforeGeneratingForm(entity, action, entityDict);

        var formControls = (await this.GenerateFormControlsAsync(
                entity,
                action,
                entityDict,
                new Dictionary<string, Expression<Func<object, bool>>>(),
                autocompleteType: null))
            .ToList();

        switch (action)
        {
            case EntityAction.Create:
            {
                var shownFormControls = this.ShownFormControlNames.Concat(this.ShownFormControlNamesOnCreate);
                var hiddenFormControls = this.HiddenFormControlNames.Concat(this.HiddenFormControlNamesOnCreate);
                formControls = SetFormControlsVisibility(formControls, shownFormControls, hiddenFormControls)
                    .ToList();
                break;
            }

            case EntityAction.Edit:
            {
                var shownFormControls = this.ShownFormControlNames.Concat(this.ShownFormControlNamesOnEdit);
                var hiddenFormControls = this.HiddenFormControlNames.Concat(this.HiddenFormControlNamesOnEdit);
                formControls = SetFormControlsVisibility(formControls, shownFormControls, hiddenFormControls)
                    .ToList();
                break;
            }

            case EntityAction.Delete:
            {
                formControls.ForEach(fc => fc.IsReadOnly = true);
                break;
            }

            default:
            {
                throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }

        return this.View(
            "../AutoCrudAdmin/EntityForm",
            new AutoCrudAdminEntityFormViewModel
            {
                FormControls = formControls,
                Action = action,
                CustomActionName = postEndpointName,
            });
    }

    /// <summary>
    /// Generates the form controls for the given entity and action.
    /// It uses the `IFormControlsHelper` to generate form controls for both primitive and complex properties of the entity.
    /// Complex properties can have their options filtered using the `complexOptionFilters` parameter.
    /// </summary>
    /// <param name="entity">The entity for which the form controls are generated.</param>
    /// <param name="action">The action for which the form controls are generated.</param>
    /// <param name="entityDict">A dictionary containing the entity's primary key value pairs.</param>
    /// <param name="complexOptionFilters">A dictionary containing lambda expressions to filter the options of complex properties.</param>
    /// <param name="autocompleteType">A type that specifies which property should have autocomplete functionality. If not provided, no property will have autocomplete functionality.</param>
    /// <returns>Returns a list of `FormControlViewModel` objects representing the generated form controls.</returns>
    protected virtual IEnumerable<FormControlViewModel> GenerateFormControls(
        TEntity entity,
        EntityAction action,
        IDictionary<string, string> entityDict,
        IDictionary<string, Expression<Func<object, bool>>> complexOptionFilters,
        Type? autocompleteType)
        => this.FormControlsHelper
            .GenerateFormControls(entity, action, complexOptionFilters, autocompleteType)
            .Select(this.AddDefaultOptions);

    /// <summary>
    /// Generates the form controls for the given entity and action.
    /// It uses the `IFormControlsHelper` to generate form controls for both primitive and complex properties of the entity.
    /// Complex properties can have their options filtered using the `complexOptionFilters` parameter.
    /// </summary>
    /// <param name="entity">The entity for which the form controls are generated.</param>
    /// <param name="action">The action for which the form controls are generated.</param>
    /// <param name="entityDict">A dictionary containing the entity's primary key value pairs.</param>
    /// <param name="complexOptionFilters">A dictionary containing lambda expressions to filter the options of complex properties.</param>
    /// <returns>Returns a list of `FormControlViewModel` objects representing the generated form controls.</returns>
    protected virtual IEnumerable<FormControlViewModel> GenerateFormControls(
        TEntity entity,
        EntityAction action,
        IDictionary<string, string> entityDict,
        IDictionary<string, Expression<Func<object, bool>>> complexOptionFilters)
        => this.GenerateFormControls(entity, action, entityDict, complexOptionFilters, null);

    /// <summary>
    /// Generates the form controls for the given entity and action.
    /// It uses the `IFormControlsHelper` to generate form controls for both primitive and complex properties of the entity.
    /// Complex properties can have their options filtered using the `complexOptionFilters` parameter.
    /// </summary>
    /// <param name="entity">The entity for which the form controls are generated.</param>
    /// <param name="action">The action for which the form controls are generated.</param>
    /// <param name="entityDict">A dictionary containing the entity's primary key value pairs.</param>
    /// <param name="complexOptionFilters">A dictionary containing lambda expressions to filter the options of complex properties.</param>
    /// <param name="autocompleteType">A type that specifies which property should have autocomplete functionality. If not provided, no property will have autocomplete functionality.</param>
    /// <returns>Returns a list of `FormControlViewModel` objects representing the generated form controls.</returns>
    protected virtual Task<IEnumerable<FormControlViewModel>> GenerateFormControlsAsync(
        TEntity entity,
        EntityAction action,
        IDictionary<string, string> entityDict,
        IDictionary<string, Expression<Func<object, bool>>> complexOptionFilters,
        Type? autocompleteType)
        => Task.FromResult(autocompleteType == null
            ? this.GenerateFormControls(entity, action, entityDict, complexOptionFilters)
            : this.GenerateFormControls(entity, action, entityDict, complexOptionFilters, autocompleteType));

    /// <summary>
    /// Handles HTTP POST for an entity form.
    /// </summary>
    /// <param name="entityDict">The entity values.</param>
    /// <param name="action">The action.</param>
    /// <param name="files">Any uploaded files.</param>
    /// <param name="routeValuesForRedirect">Optional redirect route values.</param>
    /// <returns>A redirect after handling the POST.</returns>
    protected virtual async Task<IActionResult> PostEntityForm(
        IDictionary<string, string> entityDict,
        EntityAction action,
        FormFilesContainer files,
        object? routeValuesForRedirect = null)
    {
        var (originalEntity, newEntity) = this.GetEntitiesForAction(action, entityDict);

        var actionContext = new AdminActionContext
        {
            Action = action,
            EntityDict = entityDict,
            Files = files,
        };

        await this.ValidateBeforeSave(originalEntity, newEntity, actionContext);

        await this.BeforeEntitySaveAsync(newEntity, actionContext);

        switch (action)
        {
            case EntityAction.Create:
                await this.BeforeEntitySaveOnCreateAsync(newEntity, actionContext);
                await this.CreateEntityAndSaveAsync(newEntity, actionContext);
                await this.AfterEntitySaveOnCreateAsync(newEntity, actionContext);
                break;
            case EntityAction.Edit:
                await this.BeforeEntitySaveOnEditAsync(originalEntity, newEntity, actionContext);
                await this.EditEntityAndSaveAsync(newEntity, actionContext);
                await this.AfterEntitySaveOnEditAsync(originalEntity, newEntity, actionContext);
                break;
            case EntityAction.Delete:
                await this.BeforeEntitySaveOnDeleteAsync(originalEntity, actionContext);
                await this.DeleteEntityAndSaveAsync(originalEntity, actionContext);
                await this.AfterEntitySaveOnDeleteAsync(originalEntity, actionContext);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, null);
        }

        await this.AfterEntitySaveAsync(newEntity, actionContext);

        routeValuesForRedirect ??= this.GetDefaultRouteValuesForPostEntityFormRedirect(newEntity);

        return this.RedirectToAction("Index", routeValuesForRedirect);
    }

    /// <summary>
    /// Called before saving an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="actionContext">The action context.</param>
    protected virtual Task BeforeEntitySaveAsync(TEntity entity, AdminActionContext actionContext)
        => Task.CompletedTask;

    /// <summary>
    /// Called before saving a new entity.
    /// </summary>
    /// <param name="entity">The new entity.</param>
    /// <param name="actionContext">The action context.</param>
    protected virtual Task BeforeEntitySaveOnCreateAsync(TEntity entity, AdminActionContext actionContext)
        => Task.CompletedTask;

    /// <summary>
    /// Called before deleting an existing entity.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="actionContext">The action context.</param>
    protected virtual Task BeforeEntitySaveOnDeleteAsync(TEntity entity, AdminActionContext actionContext)
        => Task.CompletedTask;

    /// <summary>
    /// Called before updating an existing entity.
    /// </summary>
    /// <param name="existingEntity">The existing entity.</param>
    /// <param name="newEntity">The updated entity.</param>
    /// <param name="actionContext">The action context.</param>
    protected virtual Task BeforeEntitySaveOnEditAsync(TEntity existingEntity, TEntity newEntity, AdminActionContext actionContext)
        => Task.CompletedTask;

    /// <summary>
    /// Creates and saves a new entity.
    /// </summary>
    /// <param name="entity">The new entity.</param>
    /// <param name="actionContext">The action context.</param>
    protected virtual async Task CreateEntityAndSaveAsync(TEntity entity, AdminActionContext actionContext)
    {
        await this.DbContext.AddAsync(entity);
        await this.DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Updates and saves an existing entity.
    /// </summary>
    /// <param name="entity">The updated entity.</param>
    /// <param name="actionContext">The action context.</param>
    protected virtual async Task EditEntityAndSaveAsync(TEntity entity, AdminActionContext actionContext)
    {
        this.DbContext.Update(entity);
        await this.DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="actionContext">The action context.</param>
    protected virtual async Task DeleteEntityAndSaveAsync(TEntity entity, AdminActionContext actionContext)
    {
        this.DbContext.Remove(entity);
        await this.DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Called after saving an entity.
    /// </summary>
    /// <param name="entity">The saved entity.</param>
    /// <param name="actionContext">The action context.</param>
    protected virtual Task AfterEntitySaveAsync(TEntity entity, AdminActionContext actionContext)
        => Task.CompletedTask;

    /// <summary>
    /// Called after saving a new entity.
    /// </summary>
    /// <param name="entity">The new entity.</param>
    /// <param name="actionContext">The action context.</param>
    protected virtual Task AfterEntitySaveOnCreateAsync(TEntity entity, AdminActionContext actionContext)
        => Task.CompletedTask;

    /// <summary>
    /// Called after updating an existing entity.
    /// </summary>
    /// <param name="oldEntity">The original entity.</param>
    /// <param name="entity">The updated entity.</param>
    /// <param name="actionContext">The action context.</param>
    protected virtual Task AfterEntitySaveOnEditAsync(TEntity oldEntity, TEntity entity, AdminActionContext actionContext)
        => Task.CompletedTask;

    /// <summary>
    /// Called after deleting an entity.
    /// </summary>
    /// <param name="entity">The deleted entity.</param>
    /// <param name="actionContext">The action context.</param>
    protected virtual Task AfterEntitySaveOnDeleteAsync(TEntity entity, AdminActionContext actionContext)
        => Task.CompletedTask;

    /// <summary>
    /// Called before generating the entity form.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="action">The action.</param>
    /// <param name="entityDict">The entity dictionary.</param>
    // TODO: check
    protected virtual Task BeforeGeneratingForm(
        TEntity entity,
        EntityAction action,
        IDictionary<string, string> entityDict)
        => Task.CompletedTask;

    /// <summary>
    /// Parses a date time from a string based on <see cref="DateTimeFormats"/>.
    /// </summary>
    /// <param name="dateTimeStr">The date time string.</param>
    /// <returns>The parsed date time.</returns>
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

    /// <summary>
    /// Gets default route values for redirect after entity form POST.
    /// </summary>
    /// <param name="newEntity">The new entity.</param>
    /// <returns>Route values for redirect.</returns>
    //TODO: Check fix
    protected virtual object? GetDefaultRouteValuesForPostEntityFormRedirect(TEntity newEntity)
        => null;

    /// <summary>
    /// Gets the complex form control name for an entity type.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>The name of the complex form control.</returns>
    protected string GetComplexFormControlNameFor<T>()
        => this.FormControlsHelper.GetComplexFormControlNameForEntityName(typeof(T).Name);

    /// <summary>
    /// Redirects to an action with a number column filter.
    /// </summary>
    /// <param name="controllerName">The controller name.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="number">The number value.</param>
    /// <param name="actionName">The action name.</param>
    /// <param name="numberFilterType">The filter type.</param>
    /// <returns>The redirect result.</returns>
    protected IActionResult RedirectToActionWithNumberFilter(
        string controllerName,
        string columnName,
        int number,
        string actionName = "Index",
        GridNumberFilterType numberFilterType = GridNumberFilterType.Equals)
        => this.RedirectToActionWithFilter(
            controllerName,
            columnName,
            number,
            actionName,
            numberFilterType.ToString());

    /// <summary>
    /// Redirects to an action with a string column filter.
    /// </summary>
    /// <param name="controllerName">The controller name.</param>
    /// <param name="columnName">The column name.</param>
    /// <param name="value">The string value.</param>
    /// <param name="actionName">The action name.</param>
    /// <param name="gridStringFilterType">The filter type.</param>
    /// <returns>The redirect result.</returns>
    protected IActionResult RedirectToActionWithStringFilter(
        string controllerName,
        string columnName,
        string value,
        string actionName = "Index",
        GridStringFilterType gridStringFilterType = GridStringFilterType.Equals)
        => this.RedirectToActionWithFilter(
            controllerName,
            columnName,
            value,
            actionName,
            gridStringFilterType.ToString());

    private static IEnumerable<FormControlViewModel> SetFormControlsVisibility(
        List<FormControlViewModel> formControls,
        IEnumerable<string> shownFormControlNames,
        IEnumerable<string> hiddenFormControlNames)
    {
        var shownFormControlNamesList = shownFormControlNames.ToList();
        var isAnyShownExplicitly = shownFormControlNamesList.Any();

        formControls.Where(x => !x.IsHidden).ToList().ForEach(fc =>
        {
            if (isAnyShownExplicitly && shownFormControlNamesList.Contains(fc.Name))
            {
                fc.IsHidden = false;
                return;
            }

            fc.IsHidden = isAnyShownExplicitly || hiddenFormControlNames.Contains(fc.Name);
        });

        return formControls;
    }

    private static IGridColumnsOf<TEntity> GenerateColumnConfiguration<TProperty>(
        IGridColumnsOf<TEntity> columns,
        PropertyInfo property,
        int columnStringMaxLength,
        IEnumerable<PropertyInfo> foreignKeys)
    {
        var lambda = ExpressionsBuilder.ForGetProperty<TEntity, TProperty>(property);

        var columnBuilder = columns
            .Add(lambda)
            .Titled(property.Name)
            .Filterable(true)
            .Sortable(true);

        if (foreignKeys.Contains(property) || property.PropertyType == typeof(string))
        {
            columnBuilder.RenderedAs(entity => property.GetValue(entity)?.ToString().ToEllipsis(columnStringMaxLength));
        }

        return columns;
    }

    private static void CopyFormPropertiesToExistingEntityFromNewEntity(TEntity existingEntity, TEntity newEntity)
        => typeof(TEntity)
            .GetProperties()
            .Where(p => p.CanWrite && !p.PropertyType.IsNavigationProperty())
            .ToList()
            .ForEach(property => property.SetValue(existingEntity, property.GetValue(newEntity, null), null));

    private IHtmlGrid<TEntity> GenerateGrid(IHtmlHelper<AutoCrudAdminIndexViewModel> htmlHelper)
        => htmlHelper
            .Grid(this.GetQueryWithIncludes(this.MasterGridFilter))
            .Build(columns =>
            {
                this.BuildGridColumns(columns, this.ColumnStringMaxLength);
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

    private IGridColumnsOf<TEntity> BuildGridColumns(
        IGridColumnsOf<TEntity> columns,
        int stringMaxLength)
    {
        if (this.ShownColumnNames.Any() && this.HiddenColumnNames.Any())
        {
            throw new Exception("Both shown and hidden column names are declared. Leave only one of them");
        }

        Func<PropertyInfo, bool> filter;

        if (this.ShownColumnNames.Any())
        {
            filter = x => this.ShownColumnNames.Contains(x.Name);
        }
        else
        {
            filter = x => !this.HiddenColumnNames.Contains(x.Name) &&
                          !x.PropertyType.IsEnumerableExceptString() &&
                          !x.GetCustomAttributes<NotMappedAttribute>().Any();
        }

        var primaryKeys = EntityType.GetPrimaryKeyPropertyInfos();
        var foreignKeys = EntityType.GetForeignKeyPropertyInfos();

        var properties = EntityType
            .GetProperties()
            .Where(filter)
            .OrderBy(property => property != primaryKeys.FirstOrDefault());

        properties = primaryKeys
            .Skip(1)
            .Aggregate(
                properties,
                (current, pk)
                    => current.ThenBy(property => property != pk))
            .ThenBy(property => property.Name);

        var columnsResult = properties
            .Aggregate(
                columns,
                (currentColumns, prop) => (IGridColumnsOf<TEntity>)GenerateColumnExpressionMethod
                    .MakeGenericMethod(prop.PropertyType)
                    .Invoke(null, new object[] { currentColumns, prop, stringMaxLength, foreignKeys }) !);

        foreach (var customGridColumn in this.CustomColumns)
        {
            var column = columnsResult
                .Add(customGridColumn.ValueFunc)
                .Titled(customGridColumn.Name)
                .Filterable(true)
                .Sortable(true);

            customGridColumn.ConfigurationFunc?.Invoke(column);
        }

        return columnsResult;
    }

    private IGridColumnsOf<TEntity> BuildGridActions(
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
                        this.RouteData.Values["controller"] !.ToString(),
                        RouteValueDictionary.FromArray(
                            EntityType.GetPrimaryKeyValue(model)
                                .ToArray() !),
                        new { }))
                    .Titled("Action");
            });

        return columns;
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

                entityDict.TryGetValue(prop.Name, out var strValue);
                object? safeValue;

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

    private async Task ValidateBeforeSave(
        TEntity existingEntity,
        TEntity newEntity,
        AdminActionContext actionContext)
    {
        var errors = this.GetValidatorResults(existingEntity, newEntity, actionContext)
            .Concat(await this.GetAsyncValidatorResults(existingEntity, newEntity, actionContext))
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
        AdminActionContext actionContext)
        => this.EntityValidators
            .Select(v => v(existingEntity, newEntity, actionContext));

    private async Task<IEnumerable<ValidatorResult>> GetAsyncValidatorResults(
        TEntity existingEntity,
        TEntity newEntity,
        AdminActionContext actionContext)
    {
        var resultTasks = this.AsyncEntityValidators
            .Select(v => v(existingEntity, newEntity, actionContext))
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
            return (null!, newEntity);
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
        var keyValues = entityType.GetPrimaryKeyValue(newEntity).ToDictionary(x => x.Key, x => x.Value.ToString());
        var existingEntity = this.GetQueryWithIncludes()
            .Where(ExpressionsBuilder.ForByEntityPrimaryKey<TEntity>(keyValues!))
            .FirstOrDefault();

        return existingEntity ?? throw new Exception(
            "The Action cannot be performed, because the original entity was not found in the db.");
    }

    private IQueryable<TEntity> GetQueryWithIncludes(Expression<Func<TEntity, bool>>? filter = null)
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

        if (filter != null)
        {
            setForGrid = setForGrid.Where(filter);
        }

        return this.ApplyIncludes(setForGrid);
    }

    private IActionResult RedirectToActionWithFilter(
        string controllerName,
        string columnName,
        object value,
        string actionName,
        string filter)
    {
        var queryParamName = UrlsHelper.GetQueryParamForColumnAndFilter(columnName, filter);
        var controller = controllerName.ToControllerBaseUri();
        var routeValues = new Dictionary<string, string?>
        {
            { queryParamName, value.ToString() },
        };

        return this.RedirectToAction(actionName, controller, routeValues);
    }

    private FormControlViewModel AddDefaultOptions(FormControlViewModel formControl)
    {
        if (formControl.Options.Any())
        {
            var shouldUseCustomGenerator = this.DefaultOptionsGenerators.TryGetValue(formControl.Type, out var generator);

            var defaultOption = shouldUseCustomGenerator
                ? generator?.Invoke()
                : Activator.CreateInstance(formControl.Type);

            if (defaultOption != null)
            {
                formControl.Options = new List<object>() { defaultOption }.Concat(formControl.Options);
            }
        }

        return formControl;
    }
}