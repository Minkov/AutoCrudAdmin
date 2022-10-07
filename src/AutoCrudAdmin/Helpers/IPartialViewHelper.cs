namespace AutoCrudAdmin.Helpers;

using AutoCrudAdmin.ViewModels;
using Microsoft.AspNetCore.Http;

public interface IPartialViewHelper
{
    string GetViewResult(HttpContext httpContext, ExpandableMultiChoiceCheckBoxFormControlViewModel checkbox, string viewName);
}