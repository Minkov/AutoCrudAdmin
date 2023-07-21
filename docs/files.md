The `FormFilesContainer` class allows file uploads:

```csharp
public async Task<IActionResult> PostEdit(
  Product product, 
  FormFilesContainer files) 
{
  // Access uploaded files
  foreach(var file in files.Files) {
    // save file
  }
}
```

On the view, use:

```html
<form asp-action="PostEdit" enctype="multipart/form-data">

</form>
```
