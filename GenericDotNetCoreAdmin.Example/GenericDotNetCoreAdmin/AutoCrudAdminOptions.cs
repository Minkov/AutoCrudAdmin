namespace GenericDotNetCoreAdmin
{
    using System.Collections.Generic;
    using System.Linq;
    using GenericDotNetCoreAdmin.Filters;

    public class AutoCrudAdminOptions
    {
        public AutoCrudAdminOptions()
        {
            this.Authorization = Enumerable.Empty<IAutoCrudAuthFilter>();
            this.AsyncAuthorization = Enumerable.Empty<IAsyncAuthCrudFilter>();
        }

        public IEnumerable<IAutoCrudAuthFilter> Authorization { get; set; }


        public IEnumerable<IAsyncAuthCrudFilter> AsyncAuthorization { get; set; }
    }
}