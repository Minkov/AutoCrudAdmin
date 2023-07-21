
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

Customize form generation:

```csharp
protected override IEnumerable<FormControlViewModel> GenerateFormControls() {

  // 1. Use defaults

  // 2. Modify defaults   
   
  // 3. Add custom controls
   
  // 4. Override completely 

}
```