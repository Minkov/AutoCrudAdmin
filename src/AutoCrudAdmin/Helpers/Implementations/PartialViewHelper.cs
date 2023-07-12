namespace AutoCrudAdmin.Helpers.Implementations;

using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Routing;
using AutoCrudAdmin.ViewModels;

/// <summary>
/// The PartialViewHelper class is an implementation of the IPartialViewHelper interface.
/// It provides methods for working with partial views in the context of the AutoCrudAdmin system.
/// </summary>
public class PartialViewHelper : IPartialViewHelper
{
    private readonly IRazorViewEngine razorViewEngine;
    private readonly ITempDataProvider tempDataProvider;
    private readonly Dictionary<string, ViewEngineResult> cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="PartialViewHelper"/> class.
    /// </summary>
    /// <param name="razorViewEngine">The razor view engine used to render views.</param>
    /// <param name="tempDataProvider">The temporary data provider used to store data between requests.</param>
    public PartialViewHelper(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider)
    {
        this.razorViewEngine = razorViewEngine;
        this.tempDataProvider = tempDataProvider;
        this.cache = new Dictionary<string, ViewEngineResult>();
    }

    /// <summary>
    /// The GetViewResult method retrieves the result of a rendered view as a string.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <param name="checkbox">The checkbox form control view model which will be passed to the view.</param>
    /// <param name="viewName">The name of the view to render.</param>
    /// <returns>A string containing the result of the rendered view.</returns>
    public string GetViewResult(
        HttpContext httpContext,
        ExpandableMultiChoiceCheckBoxFormControlViewModel checkbox,
        string viewName)
    {
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

        using var sw = new StringWriter();
        if (!this.cache.TryGetValue(viewName, out var viewResult))
        {
            viewResult = this.razorViewEngine.FindView(actionContext, viewName, false);

            if (viewResult == null || !viewResult.Success)
            {
                throw new ArgumentNullException(viewName, $"{viewName} is not found!");
            }

            this.cache.TryAdd(viewName, viewResult);
        }

        var viewDictionary =
            new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = checkbox.Expand,
            };

        var viewContext = new ViewContext(
            actionContext,
            viewResult.View,
            viewDictionary,
            new TempDataDictionary(actionContext.HttpContext, this.tempDataProvider),
            sw,
            new HtmlHelperOptions());

        viewResult.View.RenderAsync(viewContext).GetAwaiter().GetResult();
        return sw.ToString();
    }
}