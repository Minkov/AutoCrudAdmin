namespace AutoCrudAdmin.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using AutoCrudAdmin.Controllers;
using AutoCrudAdmin.Extensions;
using AutoCrudAdmin.Helpers;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

/// <summary>
/// The <see cref="AutoCrudAdminControllerNameConvention"/> class defines a naming convention for AutoCrudAdmin controllers.
/// It extends the Attribute class and implements the IControllerModelConvention interface.
/// It's primary function is to set a controller name and route for every not explicitly named AutoCrudAdmin controller.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AutoCrudAdminControllerNameConvention : Attribute, IControllerModelConvention
{
    static AutoCrudAdminControllerNameConvention()
    {
        Controllers = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => p.IsSubclassOfRawGeneric(typeof(AutoCrudAdminController<>)))
            .ToList();

        EntityTypeToNameMap = ReflectionHelper.DbSetProperties
            .DistinctBy(x => x.PropertyType.GetGenericArguments().FirstOrDefault())
            .ToDictionary(
                set => set.PropertyType.GetGenericArguments().FirstOrDefault() !,
                set => set.Name);
    }

    private static IEnumerable<Type> Controllers { get; set; }

    private static Dictionary<Type, string> EntityTypeToNameMap { get; set; }

    /// <summary>
    /// The <see cref="Apply"/>  method is used to apply the naming convention to the ControllerModel.
    /// It checks if a controller is an <see cref="AutoCrudAdminController"/> and whether it has been explicitly named.
    /// If not explicitly named, it sets the controller name and route to the name of the entity type in plural.
    /// </summary>
    /// <param name="controller">The <see cref="ControllerModel"/> that the convention will be applied to.</param>
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