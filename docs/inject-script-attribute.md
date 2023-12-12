# The InjectScript attribute

The InjectScript attribute is an integral part of the AutoCrudAdmin framework, designed to enhance the EntityForm view by injecting custom JavaScript modules. This attribute allows developers to add client-side logic to the form without modifying the core files. It leverages the power of ASP.NET Core's MVC filters to inject the script automatically.

# Problem Statement 

Customizing the behavior of an entity form usually requires modifications to the base HTML or JavaScript files, making it hard to manage and update. This traditional method is cumbersome, prone to errors, and not modular.

# Traditional Methods and Drawbacks

- Inline Scripting: Embedding JavaScript directly into the HTML. This is not modular and makes the HTML file bulky.

- Global Script Files: Adding custom logic in a global JavaScript file, making it hard to manage and leading to potential conflicts.

- Manual Injection: Manually adding `<script>` tags to each form, which is time-consuming and error-prone.

# Introducing InjectScript

The InjectScript attribute solves these issues by offering a way to inject a script module directly into the EntityForm view. This is done by adding the attribute to the controller action responsible for rendering the form.

## Syntax
```csharp
    [InjectScript("/js/entities/users/edit.js")]
    public override Task<IActionResult> Edit(IDictionary<string, string> complexId, string postEndpointName)
    {
    return base.Edit(complexId, postEndpointName);
    }
```

## Parameters  

- Script Path: The attribute takes a single parameter—the path to the JavaScript file to be injected. The path should start from the application's static files folder location.

  - Correct: "/js/entities/manualJobs/create.js"

  - Incorrect: "wwwroot/js/entities/manualJobs/create.js"

## Behavior

- Module Loading: The script is loaded as a JavaScript module, allowing for modular code.
- Scope: The script is scoped to the specific EntityForm where the attribute is used.

# How It Works

When the controller action is invoked, the attribute intercepts the call. 

It then adds the script path to the `ViewData["AdditionalScriptPath"]` dictionary.

The EntityForm view checks this dictionary and injects the script accordingly.

# Usage Example

In EntityForm.cshtml:
```csharp
var additionalScriptPath = this.ViewData["AdditionalScriptPath"] as string;
```

# Real-World Scenario

Imagine you have an entity form where a user can select multiple options from a dropdown. You want to show or hide other form fields based on the user's selection. By using the InjectScript attribute, you can achieve this functionality without altering the EntityForm view.

# Future Outlook 

The InjectScript attribute offers a modular approach to script injection, but its potential doesn't end there. Future implementations could include:

- Conditional script loading based on user roles or permissions.
- Support for injecting multiple scripts.

# Conclusion and Call to Action

The InjectScript attribute provides an elegant solution for enhancing entity forms in AutoCrudAdmin. It eliminates the need for cumbersome workarounds, making your development process more efficient. Consider using this attribute in your next project to experience its benefits firsthand.