namespace AutoCrudAdmin.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using AutoCrudAdmin.Controllers;
    using AutoCrudAdmin.Extensions;
    using AutoCrudAdmin.Helpers;
    using Microsoft.AspNetCore.Mvc.ApplicationParts;
    using Microsoft.AspNetCore.Mvc.Controllers;

    /// <summary>
    /// <see cref="AutoCrudAdminControllerFeatureProvider"/> is an internal class that acts as a feature provider for AutoCrudAdmin controllers.
    /// </summary>
    internal class AutoCrudAdminControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        static AutoCrudAdminControllerFeatureProvider()
            => Types = ReflectionHelper.DbSetProperties
                .Select(p => p.PropertyType)
                .Select(dt => dt.GetGenericArguments().FirstOrDefault())
                .Where(t => t != null)
                .Select(t => t!)
                .ToList();

        private static IEnumerable<Type> Types { get; set; }

        /// <summary>
        /// PopulateFeature is a method that populates the feature with the controller types.
        /// This method gets all the controller types that are part of the AutoCrudAdmin and adds them to the feature's controllers list.
        /// </summary>
        /// <param name="parts">The list of application parts.</param>
        /// <param name="feature">The ControllerFeature that will be populated.</param>
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
}