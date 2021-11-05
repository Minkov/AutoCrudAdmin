namespace AutoCrudAdmin.ViewModels
{
    using System;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class AutoCrudAdminIndexViewModel
    {
        public Func<IHtmlHelper<AutoCrudAdminIndexViewModel>, IHtmlContent> GenerateGrid { get; set; }
    }
}