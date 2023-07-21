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

Then register in Startup.cs:

```csharp
services.AddAutoCrudAdmin(options => {
  options.Authorization.Add(new RequireAuthFilter());
});
```

You can also use declarative authorization attributes like `[Authorize]`.

For role-based auth, apply attributes like `[Authorize(Roles = "Admin")]` to controller classes or actions.