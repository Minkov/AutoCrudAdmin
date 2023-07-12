namespace AutoCrudAdmin.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoCrudAdmin.Controllers;
using AutoCrudAdmin.Extensions;
using AutoCrudAdmin.Helpers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

public class AutoCrudAdminControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    static AutoCrudAdminControllerFeatureProvider()
        => Types = ReflectionHelper.DbSetProperties
            .Select(p => p.PropertyType)
            .Select(dt => dt.GetGenericArguments().FirstOrDefault() !)
            .ToList();

    private static IEnumerable<Type> Types { get; set; }

    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        => Types
            .ToList()
            .ForEach(type =>
            {
                var typeArgs = new[] { type };
                var controllerType = typeof(AutoCrudAdminController<>)
                    .MakeGenericType(typeArgs)
                    .GetTypeInfo();
                feature.Controllers.Add(controllerType);
            });
}