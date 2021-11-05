namespace AutoCrudAdmin.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using AutoCrudAdmin.Controllers;
    using Microsoft.AspNetCore.Mvc.ApplicationParts;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.EntityFrameworkCore;

    public class AutoCrudAdminControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        static AutoCrudAdminControllerFeatureProvider()
            // TODO:  Must be extracted, as it is repeated in GenericAdminControllerNameConvention
            => Types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(DbContext)))
                .SelectMany(t => t.GetProperties())
                .Where(p => p.PropertyType.IsGenericType
                            && p.PropertyType.Name.StartsWith("DbSet"))
                .Select(p => p.PropertyType)
                .Select(dt => dt.GetGenericArguments().FirstOrDefault())
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
}