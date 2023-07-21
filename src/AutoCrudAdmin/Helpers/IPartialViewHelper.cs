namespace AutoCrudAdmin.Helpers;

using AutoCrudAdmin.ViewModels;
using Microsoft.AspNetCore.Http;

/// <summary>
/// The IPartialViewHelper interface defines a contract for classes that assist in working
/// with partial views in the context of the AutoCrudAdmin system.
/// </summary>
public interface IPartialViewHelper
{
    /// <summary>
    /// The GetViewResult method retrieves the result of a rendered view as a string.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <param name="checkbox">The checkbox form control view model which will be passed to the view.</param>
    /// <param name="viewName">The name of the view to render.</param>
    /// <returns>A string containing the result of the rendered view.</returns>
    string GetViewResult(
        HttpContext httpContext,
        ExpandableMultiChoiceCheckBoxFormControlViewModel checkbox,
        string viewName);
}