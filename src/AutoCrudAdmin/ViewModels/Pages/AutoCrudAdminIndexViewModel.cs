namespace AutoCrudAdmin.ViewModels.Pages;

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

/// <summary>
/// Represents a ViewModel for the AutoCrudAdmin index page.
/// This ViewModel encapsulates the data necessary for rendering the grid and toolbar actions on the index page.
/// </summary>
public class AutoCrudAdminIndexViewModel
{
    /// <summary>
    /// Gets or sets a function that takes an <see cref="IHtmlHelper{AutoCrudAdminIndexViewModel}"/> and returns
    /// an <see cref="IHtmlContent"/>. This function is used to generate the HTML content for the grid on the index page.
    /// </summary>
    public Func<IHtmlHelper<AutoCrudAdminIndexViewModel>, IHtmlContent> GenerateGrid { get; set; } = default!;

    /// <summary>
    /// Gets or sets the toolbar actions for the index page.
    /// These actions represent the various operations that can be performed from the toolbar on the index page.
    /// </summary>
    public IEnumerable<AutoCrudAdminGridToolbarActionViewModel> ToolbarActions { get; set; } = default!;
}