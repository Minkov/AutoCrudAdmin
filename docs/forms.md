
The `AutoCrudAdmin` provides ways to customize the forms as well.

### Shown Fields

Explicitly show fields:

```csharp
protected override IEnumerable<string> ShownFormControlNames {
  new[] { "Name", "Price" }
};
```

### Hidden Fields

Hide fields:

```csharp
protected override IEnumerable<string> HiddenFormControlNames {
  new[] { "Description" }  
};
``` 

### Read-only Fields

Make fields read-only:

```csharp  
someFormControl.IsReadOnly = true;
```

### Validation

Validate on save:

```csharp
protected override IEnumerable<Func<TEntity, TEntity, ValidatorResult>> EntityValidators {

  (existing, updated) => {
    if (updated.Quantity < 0) {
      return ValidatorResult.Error("Invalid quantity");
    }
    return ValidatorResult.Success();
  }

};
```

```csharp
protected override IEnumerable<Func<TEntity, TEntity, ValidatorResult>> EntityValidators {

  (existing, updated) => {
    if (updated.Quantity < 0) {
      return ValidatorResult.Error("Invalid quantity");
    }
    return ValidatorResult.Success();
  }

};
```