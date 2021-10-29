namespace GenericDotNetCoreAdmin.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GenericDotNetCoreAdmin.Controllers;
    using GenericDotNetCoreAdmin.Extensions;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;

    [AttributeUsage(AttributeTargets.Class)]
    public class GenericAdminControllerNameConvention : Attribute, IControllerModelConvention
    {
        private static IEnumerable<Type> Controllers { get; set; }

        static GenericAdminControllerNameConvention()
        {
            Controllers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsSubclassOfRawGeneric(typeof(GenericAdminController<>)))
                .ToList();
        }

        public void Apply(ControllerModel controller)
        {
            if ((!controller.ControllerType.IsGenericType
                 || controller.ControllerType.GetGenericTypeDefinition() != typeof(GenericAdminController<>))
                && !controller.ControllerType.IsSubclassOf(typeof(GenericAdminController<>)))
            {
                return;
            }

            if (Controllers.Any(controllerType => controllerType.IsSubclassOf(controller.ControllerType)))
            {
                return;
            }

            var entityType = controller.ControllerType.GenericTypeArguments[0];
            controller.ControllerName = entityType.Name;
            controller.RouteValues["Controller"] = entityType.Name;
        }
    }
}