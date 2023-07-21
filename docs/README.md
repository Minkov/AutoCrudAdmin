AutoCrudAdmin is a library that provides automatic CRUD (Create, Read, Update, Delete) screens and API endpoints for entity classes in an ASP.NET Core application. 

## Installation

Install the `AutoCrudAdmin` NuGet package in your ASP.NET Core project.

```
dotnet add package AutoCrudAdmin
```

## Usage

1. Call `services.AddAutoCrudAdmin()` in `ConfigureServices()` in `Startup.cs`.
2. Call `app.UseAutoCrudAdmin()` in `Configure()` in `Startup.cs`.
3. You have CRUD views and opereations for each entity in you DbContexts
4. You can customize the default behavior by creating a controller that inherits from `AutoCrudAdminController<TEntity>` where `TEntity` is the entity class. 

That's it! AutoCrudAdmin will automatically generate CRUD screens and API endpoints for the entities.

The CRUD screens will be available under `/ControllerName` like `/Products`, `/Customers` etc. The API endpoints will be at `/ControllerName/Create`, `/ControllerName/Edit` etc.

# Customizing Behavior

AutoCrudAdmin provides many options to customize the generated CRUD screens and API behavior.

## Customizing Columns

The grid view that lists entities on the Index screen can be customized by overriding `ShownColumnNames`, `HiddenColumnNames` and `CustomColumns` in the controller.

For example:

```csharp
public class ProductsController : AutoCrudAdminController<Product> 
{
  protected override IEnumerable<string> ShownColumnNames => 
    new[] { "Name", "Price" };

  protected override IEnumerable<CustomGridColumn<Product>> CustomColumns =>
    new[]
    {
      new CustomGridColumn<Product>  
      {
        Name = "Discount",
        ValueFunc = p => p.CalculateDiscount().ToString("0.##")
      }
    };
}
```

## Customizing Form Fields

Similarly, the forms for Create and Edit actions can be customized via `ShownFormControlNames`, `HiddenFormControlNames`, `ShownFormControlNamesOnCreate` etc.

### GenerateFormControls Methods

The `GenerateFormControls` methods are designed to be the main extensibility point for customizing the form fields displayed on Create and Edit pages.

By default, they auto-generate a basic set of form controls for all model properties, but you can customize this in several ways:

#### 1. Accept the Defaults

The default implementation will:

- Create standard form controls for each primitive property like strings, numbers etc.

- Create dropdowns for navigation properties related to other entities.

This requires no customization and can be used as-is.

#### 2. Modify the Defaults 

You can modify the default form control collection to:

- Hide fields completely:

```csharp
protected override IEnumerable<string> HiddenFormControlNames = { "Description" }; 
```

- Show only certain fields:

```csharp 
protected override IEnumerable<string> ShownFormControlNames = { "Name", "Price" };
```

- Read-only fields:

```csharp
someFormControl.IsReadOnly = true; 
```

- Change control type - e.g. textbox to textarea

#### 3. Add Custom Controls

Add completely custom controls by creating new `FormControlViewModel` instances, e.g.:

```csharp
var ratingControl = new FormControlViewModel  
{
  // populate options from web API
  Options = await GetRatingOptionsFromAPI() 
};

return base.GenerateFormControls().Concat(new[] { ratingControl });
```

#### 4. Override Completely 

For full control, override `GenerateFormControls` and explicitly define the `FormControlViewModel` collection.

Let me know if this helps explain the intent and flexibility better! I'm happy to clarify or expand any part.

## Adding Custom Actions

Custom controller actions can be added as needed and will be available alongside the standard AutoCrudAdmin actions.

## Validating Entities

To perform validation, override `EntityValidators`, or `AsyncEntityValidators`, and return validation result objects:

```csharp 
protected override IEnumerable<Func<Product, Product, AdminActionContext, ValidatorResult>> EntityValidators => 
  new Func<Product, Product, AdminActionContext, ValidatorResult>[]
  {
    (existing, updated, context) => {
      if (updated.Quantity < 0) {
        return ValidatorResult.Error("Quantity cannot be negative");  
      }
      return ValidatorResult.Success();
    }
  };
```

# Adding Navigation Menu

To generate a navigation menu with links to the CRUD screens for each entity, use the `NavHelper` class:

```csharp
@foreach (var item in NavHelper.GetNavItems()) 
{
  <li>
    <a asp-controller="@item" asp-action="Index">@item</a> 
  </li>
}
```

# Changing Layout

The default layout is `_AutoCrudAdmin_Layout`. To use a custom layout, set the `LayoutName` property in `AutoCrudAdminOptions`.

For example:
```csharp
services.AddAutoCrudAdmin(options => {
   options.LayoutName = "_MyCustomLayout"; 
});
```

# Adding Authentication

Authentication can be added by providing `IAutoCrudAuthFilter` implementations to the `AutoCrudAdminOptions.Authorization` collection. 

For example:

```csharp
options.Authorization.Add(new MyAuthFilter()); 
```

# Handling Files

The `FormFilesContainer` class is used to pass files from forms to API endpoints. The files are available in the `AdminActionContext.Files` property.

# Overriding Views

To override the default AutoCrudAdmin views, create view files with the same name in `/Views/AutoCrudAdmin/`.


Here is example documentation for some of the key customization options in the `AutoCrudAdminController`:

# Customizing AutoCrudAdminController

The `AutoCrudAdminController<TEntity>` base class contains many virtual methods and properties that can be overridden to customize the CRUD behavior for an entity.

## Customizing Grid

### Custom Columns

Add custom columns to the index grid view:

```csharp
protected override IEnumerable<CustomGridColumn<TEntity>> CustomColumns =>
  new[]
  {
    new CustomGridColumn<TEntity>  
    {
      Name = "Discount",
      ValueFunc = e => e.CalculateDiscount().ToString("P") 
    }
  };
```

### Shown Columns 

Explicitly specify shown columns:

```csharp
protected override IEnumerable<string> ShownColumns => 
  new[] { "Name", "Price" };
```

### Hidden Columns

Hide columns:

```csharp
protected override IEnumerable<string> HiddenColumns => 
  new[] { "Description" };
```

### Custom Actions

Add custom actions to each row:

```csharp
protected override IEnumerable<GridAction> CustomActions =>
  new[]
  {
    new GridAction { 
      Name = "Notify",
      Action = "Notify"
    }
  };
```

### Rows Per Page

Customize rows per page options:

```csharp
protected override IEnumerable<Tuple<int, string>> PageSizes =>
  new[]
  {
    new Tuple<int, string>(10, "10 per page"),
    new Tuple<int, string>(50, "50 per page")
  };
```

## Customizing the forms

### Shown Fields

Explicitly show fields:

```csharp
protected override IEnumerable<string> ShownFormControlNames => 
  new[] { "Name", "Price" }; 
```

### Hidden Fields

Hide fields:

```csharp
protected override IEnumerable<string> HiddenFormControlNames =>
  new[] { "Description" };
```

### Read-only Fields

Make fields read-only:

```csharp
protected override async Task BeforeGeneratingForm(...) {
  someFormControl.IsReadOnly = true;
}
```

### Validation

Validate on save:

```csharp
protected override IEnumerable<Func<TEntity, TEntity, ValidatorResult>> EntityValidators =>
  new[]
  {
    (existing, updated) => {
      if (updated.Quantity < 0) {
        return ValidatorResult.Error("Invalid quantity");
      }
      return ValidatorResult.Success();
    },
    this.CustomValidateEntity,
  };
```


## Customizing Actions

### Changing Redirect

Override to redirect to custom route after save:

```csharp
protected override object GetDefaultRouteValuesForPostEntityFormRedirect(TEntity entity) 
{
  return new { id = entity.Id, foo = "bar" };
}
```

### Async Initialization 

Perform async logic before rendering views:

```csharp
protected override async Task BeforeGeneratingForm(...) 
{
  await MyService.LoadExtraDataAsync();
}
```

## Customizing Queries

### Eager Loading

Customize eager loading for performance:

```csharp
protected override IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query)
{
  return query.Include(e => e.Details); 
}
``` 

### Filtering

Set global query filter:

```csharp
protected override Expression<Func<TEntity, bool>> MasterGridFilter 
{
  get { return e => !e.IsDeleted; }
}
```

## Misc Customizations

### String Limits

Limit max length of strings:

```csharp
protected override int? ColumnStringMaxLength => 25; 
```



# Troubleshooting

Some common issues:

- Make sure DbContext is registered properly in `ConfigureServices()`
- For navigation menu to show up, controller must inherit from `AutoCrudAdminController`