The `IAutoCrudAuthFilter` interface allows implementing custom authorization logic.

For example, to require authentication:

```csharp 
public class RequireAuthFilter : IAutoCrudAuthFilter {

  public bool Authorize(HttpContext context)   
  {
    return context.User.Identity.IsAuthenticated;
  }

}
```

Register in Startup.cs:

```csharp
services.AddAutoCrudAdmin(options => {
  options.Authorization.Add(new RequireAuthFilter()); 
});
```

Can also use declarative attributes like `[Authorize]`.