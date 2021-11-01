namespace GenericDotNetCoreAdmin
{
    using System.Collections.Generic;
    using GenericDotNetCoreAdmin.Filters;

    public class AutoCrudAdminOptions
    {
        public IEnumerable<IAutoCrudAuthFilter> Authorization { get; set; }
    }
}