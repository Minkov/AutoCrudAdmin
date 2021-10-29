namespace GenericDotNetCoreAdmin.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GenericDotNetCoreAdmin.Controllers;
    using GenericDotNetCoreAdmin.Extensions;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;
    using Microsoft.EntityFrameworkCore;

    [AttributeUsage(AttributeTargets.Class)]
    public class GenericAdminControllerNameConvention : Attribute, IControllerModelConvention
    {
        private static IEnumerable<Type> Controllers { get; set; }
        private static Dictionary<Type, string> EntityTypeToNameMap { get; set; }

        static GenericAdminControllerNameConvention()
        {
            Controllers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsSubclassOfRawGeneric(typeof(GenericAdminController<>)))
                .ToList();

            EntityTypeToNameMap = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(DbContext)))
                .SelectMany(t => t.GetProperties())
                .Where(p => p.PropertyType.IsGenericType
                            && p.PropertyType.Name.StartsWith("DbSet"))
                .ToDictionary(
                    set => set.PropertyType.GetGenericArguments().FirstOrDefault(),
                    set => set.Name
                );
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
            controller.ControllerName = EntityTypeToNameMap[entityType];
            controller.RouteValues["Controller"] = EntityTypeToNameMap[entityType];
        }
    }
}