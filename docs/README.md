# AutoCrudAdmin

AutoCrudAdmin is a library that provides automatic CRUD (Create, Read, Update, Delete) screens and API endpoints for entity classes in an ASP.NET Core application.

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Customization](#customization)
  - [Customizing Grid](#customizing-grid)
  - [Customizing Form Fields](#customizing-form-fields)
  - [GenerateFormControls Methods](#generateformcontrols-methods)
  - [Adding Custom Actions](#adding-custom-actions)
  - [Validating Entities](#validating-entities)  
- [Navigation Menu](#navigation-menu)
- [Changing Layout](#changing-layout)
- [Authentication](#authentication) 
- [Handling Files](#handling-files)
- [Overriding Views](#overriding-views)
- [Customizing AutoCrudAdminController](#customizing-autocrudadmincontroller)
- [Troubleshooting](#troubleshooting)

## Installation

Install the [AutoCrudAdmin](https://github.com/minkov/autocrudadmin) NuGet package in your ASP.NET Core project.

```
dotnet add package AutoCrudAdmin
```

## Usage

1. Call `services.AddAutoCrudAdmin()` in `ConfigureServices()` in `Startup.cs`.

2. Call `app.UseAutoCrudAdmin()` in `Configure()` in `Startup.cs`.

3. You have CRUD views and operations for each entity in your DbContexts.

4. You can customize the default behavior by creating a controller that inherits from `AutoCrudAdminController<TEntity>` where `TEntity` is the entity class.  

That's it! AutoCrudAdmin will automatically generate CRUD screens and API endpoints for the entities.

The CRUD screens will be available under `/ControllerName` like `/Products`, `/Customers` etc. The API endpoints will be at `/ControllerName/Create`, `/ControllerName/Edit` etc.

## Customization

AutoCrudAdmin provides options to customize the generated CRUD screens and API behavior.

### Customizing Grid 

The grid view that lists entities on the Index screen can be customized by overriding methods in the controller.

For example:

```csharp
public class ProductsController : AutoCrudAdminController<Product>
{
  protected override IEnumerable<string> ShownColumnNames =>
    new[] { "Name", "Price" };
      
  // Add custom column
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

See [API Reference](https://github.com/minkov/autocrudadmin/blob/main/src/AutoCrudAdmin/Controllers/AutoCrudAdminController.cs) for all options.

For details, see [Customizing the Grid](grid.md) 

### Customizing Form Fields

Similarly, the forms for Create and Edit actions can be customized via methods like:

- `ShownFormControlNames`
- `HiddenFormControlNames` 
- `ShownFormControlNamesOnCreate`

See [API Reference](https://github.com/minkov/autocrudadmin/blob/main/src/AutoCrudAdmin/Controllers/AutoCrudAdminController.cs) for all options.

For details, see [Customizing Forms](forms.md)

### GenerateFormControls Methods

The [`GenerateFormControls`](https://github.com/minkov/autocrudadmin/blob/master/src/AutoCrudAdmin/Controllers/AutoCrudAdminController.cs) methods are the main way to customize the form fields on Create and Edit pages.

By default, they auto-generate controls for all model properties. You can customize this in several ways:

#### 1. Accept the Defaults

The default implementation creates controls for:

- Primitive properties like strings, numbers etc.

- Navigation properties as dropdowns.

This requires no customization.

#### 2. Modify the Defaults

You can modify the defaults like:

- Hide fields with `HiddenFormControlNames` 

- Show only certain fields using `ShownFormControlNames`

- Make fields read-only

#### 3. Add Custom Controls

Add completely custom controls by creating `FormControlViewModel`, e.g.:

```csharp 
// populate from API
var ratingControl = new FormControlViewModel();

return base.GenerateFormControls().Concat(new[] { ratingControl }); 
```

#### 4. Override Completely

For full control, override `GenerateFormControls` and explicitly build controls.

### Adding Custom Actions

Additional controller actions can be added and will show alongside the standard CRUD actions.

### Validating Entities

To perform validation, override `EntityValidators`:

```csharp
protected override IEnumerable<Func<Product, Product, ValidatorResult>> EntityValidators =>
  new Func<Product, Product, ValidatorResult>[]
  {
    (existing, updated) => {
      if (updated.Quantity < 0) {
        return ValidatorResult.Error("Invalid quantity");
      }
      return ValidatorResult.Success();
    }
  };
```

## Navigation Menu

Generate a menu with links to CRUD screens using the [`NavHelper`](https://github.com/minkov/autocrudadmin/blob/master/src/AutoCrudAdmin/Helpers/NavHelper.cs):

```csharp
@foreach (var item in NavHelper.GetNavItems()) {
  <li>
    <a asp-controller="@item">@item</a> 
  </li>
}
```

## Changing Layout

The default layout is `_AutoCrudAdmin_Layout`. To use a custom layout: 

```csharp
services.AddAutoCrudAdmin(options => {
  options.LayoutName = "_CustomLayout"; 
});
```

## Authentication

Add authentication by providing [`IAutoCrudAuthFilter`](https://github.com/minkov/autocrudadmin/blob/master/src/AutoCrudAdmin/Filters/IAutoCrudAuthFilter.cs) implementations:

```csharp
options.Authorization.Add(new MyAuthFilter());
```

See [Auth Filters Docs](auth.md) for examples.

## Handling Files

The [`FormFilesContainer`](https://github.com/minkov/autocrudadmin/blob/master/src/AutoCrudAdmin/ViewModels/FormFilesContainer.cs) class passes files from forms to API endpoints. 

Files are available in `AdminActionContext.Files`.

## Overriding Views

Override default views by creating view files in `/Views/AutoCrudAdmin/`.

## Customizing AutoCrudAdminController

The `AutoCrudAdminController` base class has many virtual members that can customize CRUD behavior:

### Customize Grid

Customize grid columns:

```csharp
protected override IEnumerable<CustomGridColumn<TEntity>> CustomColumns {..}
```

Add row actions:

```csharp  
protected override IEnumerable<GridAction> CustomActions {..}
```

See [API reference](https://github.com/minkov/autocrudadmin/blob/master/src/AutoCrudAdmin/Controllers/AutoCrudAdminController.cs) for full options.

### Customize Forms

Show/hide form fields:

```csharp
protected override IEnumerable<string> ShownFormControlNames {..}  
protected override IEnumerable<string> HiddenFormControlNames {..}
```

See [API reference](https://github.com/minkov/autocrudadmin/blob/master/src/AutoCrudAdmin/Controllers/AutoCrudAdminController.cs) for full options. 

### Other Customizations

Override methods like:

- `GetDefaultRouteValuesForPostEntityFormRedirect` - Redirect after save
- `ApplyIncludes` - Eager load related data

## Troubleshooting 

Some common issues:

- Ensure DbContext registered properly in `ConfigureServices()`
- For navigation menu, controller must inherit from `AutoCrudAdminController`
- Check for conflicts with other packages