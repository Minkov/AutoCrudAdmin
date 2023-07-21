Customizing the grid

# Custom Columns

Add custom columns to the index grid view:

```csharp
protected override IEnumerable<CustomGridColumn<TEntity>> CustomColumns {
  new CustomGridColumn<TEntity>  
  {
    Name = "Discount",
    ValueFunc = e => e.CalculateDiscount() 
  }
};
```

# Shown Columns

Explicitly specify shown columns:

```csharp
protected override IEnumerable<string> ShownColumns {
  new[] { "Name", "Price" }
};
```

# Hidden Columns

Hide columns:

```csharp
protected override IEnumerable<string> HiddenColumns {
  new[] { "Description" }
};
```

# Custom Row Actions

Add custom actions for each grid row:

```csharp
protected override IEnumerable<GridAction> CustomActions {

  new GridAction { 
    Name = "Notify",
    Action = "Notify"
  }

};
```

# Rows Per Page

Customize rows per page options:

```csharp
protected override IEnumerable<Tuple<int, string>> PageSizes {
  
  new Tuple<int, string>(10, "10 per page"),
  new Tuple<int, string>(50, "50 per page")
  
};
```
