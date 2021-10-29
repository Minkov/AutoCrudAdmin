namespace GenericDotNetCoreAdmin.ViewModels
{
    using System;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class GenericAdminIndexViewModel
    {
        public Func<IHtmlHelper<GenericAdminIndexViewModel>, IHtmlContent> GenerateGrid { get; set; }
    }
}