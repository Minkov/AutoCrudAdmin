Additional controller actions can be added alongside the default CRUD operations.

For example:

```csharp
public class ProductsController : AutoCrudAdminController<Product>
{

  [HttpGet]
  public IActionResult NotifyLowStock()
  {
    // Notify for products with low stock   
  }

  protected override IEnumerable<GridAction> CustomActions =>
    new GridAction[]
    {
      new GridAction {
        Name = "Notify Low Stock",
        Action = "NotifyLowStock"  
      }
    };
}
```

The `CustomActions` property adds it as a button on the index grid.
