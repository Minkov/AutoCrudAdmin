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

public class PartialViewHelper : IPartialViewHelper
{
    private readonly IRazorViewEngine razorViewEngine;
    private readonly ITempDataProvider tempDataProvider;
    private readonly Dictionary<string, ViewEngineResult> cache;

    public PartialViewHelper(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider)
    {
        this.razorViewEngine = razorViewEngine;
        this.tempDataProvider = tempDataProvider;
        this.cache = new Dictionary<string, ViewEngineResult>();
    }

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