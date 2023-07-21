
Ways to customize forms.

### Show/Hide Fields

```csharp
protected override IEnumerable<string> ShownFormControlNames {
  // explicit fields
}

protected override IEnumerable<string> HiddenFormControlNames {
  // hidden fields  
} 
```

### Read-only Fields

```csharp
someFormControl.IsReadOnly = true; 
```

### GenerateFormControls Methods 

The `GenerateFormControls` methods are the main way to customize the form fields generated for Create and Edit pages.

There are a few overload options:

```csharp
// Basic signature
protected override IEnumerable<FormControlViewModel> GenerateFormControls(
  TEntity entity, 
  EntityAction action,
  IDictionary<string, string> entityDict)

// With option filters  
protected override IEnumerable<FormControlViewModel> GenerateFormControls(
  TEntity entity,
  EntityAction action, 
  IDictionary<string, string> entityDict,
  IDictionary<string, Expression<Func<object, bool>>> optionFilters)  

// Async version  
protected override async Task<IEnumerable<FormControlViewModel>> GenerateFormControlsAsync(...)

```

The methods return a collection of `FormControlViewModel` representing each form control.

### Default Implementation

By default, it creates controls for:

- Primitive properties like strings, numbers.
- Navigation properties as dropdowns loaded from DbSets.

This requires no customization for basic cases.

### Customizing Defaults

You can customize the defaults like:

**Hide fields**

```csharp
protected override IEnumerable<string> HiddenFormControlNames { 
  // fields to hide
}
```

**Show only certain fields** 

```csharp
protected override IEnumerable<string> ShownFormControlNames {
  // fields to show
} 
```

**Make fields read-only**

```csharp
formControl.IsReadOnly = true;
```

### Adding Custom Controls

To add completely custom controls, create and add `FormControlViewModel` instances:

```csharp
protected override IEnumerable<FormControlViewModel> GenerateFormControls() 
{
  // Create custom control
  var ratingControl = new FormControlViewModel();
  
  // Add to default fields
  return base.GenerateFormControls().Concat(new[] { ratingControl });
}
```

### Overriding Completely 

For full control, override `GenerateFormControls` and explicitly build the controls:

```csharp
protected override IEnumerable<FormControlViewModel> GenerateFormControls()
{
  return new FormControlViewModel[]
  {
     // Explicitly build controls
  };
}
```

Let me know if you need any clarification or have additional questions!