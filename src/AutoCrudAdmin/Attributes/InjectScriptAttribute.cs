namespace AutoCrudAdmin.Attributes
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    /// <summary>
    /// An attribute that injects a script into the EntityForm view.
    /// The script is loaded as a module.
    /// The script path starts from the currently running application static files folder location.
    /// Correct value: "/js/entities/manualJobs/create.js".
    /// Wrong value: "wwwroot/js/entities/manualJobs/create.js".
    /// </summary>
    public class InjectScriptAttribute : Attribute, IActionFilter
    {
        private readonly string scriptPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectScriptAttribute"/> class.
        /// </summary>
        /// <param name="scriptPath">The path to the script to inject.</param>
        public InjectScriptAttribute(string scriptPath)
        {
            this.scriptPath = scriptPath;
        }

        /// <inheritdoc/>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            this.AddScriptToViewData(context);
        }

        /// <inheritdoc/>
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        /// <summary>
        /// Adds the script path to the view data. And the view will load the script as module.
        /// </summary>
        /// <param name="context">The action executing context.</param>
        private void AddScriptToViewData(ActionExecutingContext context)
        {
            var controller = context.Controller as Controller;

            if (controller?.ViewData != null)
            {
                controller.ViewData["AdditionalScriptPath"] = this.scriptPath;
            }
        }
    }
}