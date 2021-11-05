namespace AutoCrudAdmin.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoCrudAdmin.Controllers;
    using AutoCrudAdmin.Extensions;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;
    using Microsoft.EntityFrameworkCore;

    [AttributeUsage(AttributeTargets.Class)]
    public class AutoCrudAdminControllerNameConvention : Attribute, IControllerModelConvention
    {
        private static IEnumerable<Type> Controllers { get; set; }
        private static Dictionary<Type, string> EntityTypeToNameMap { get; set; }

        static AutoCrudAdminControllerNameConvention()
        {
            Controllers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsSubclassOfRawGeneric(typeof(AutoCrudAdminController<>)))
                .ToList();

            EntityTypeToNameMap = ReflectionHelper.DbSetProperties
                .ToDictionary(
                    set => set.PropertyType.GetGenericArguments().FirstOrDefault(),
                    set => set.Name
                );
        }

        public void Apply(ControllerModel controller)
        {
            if ((!controller.ControllerType.IsGenericType
                 || controller.ControllerType.GetGenericTypeDefinition() != typeof(AutoCrudAdminController<>))
                && !controller.ControllerType.IsSubclassOf(typeof(AutoCrudAdminController<>)))
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