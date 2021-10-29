namespace GenericDotNetCoreAdmin.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using GenericDotNetCoreAdmin.Controllers;
    using Microsoft.AspNetCore.Mvc.ApplicationParts;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.EntityFrameworkCore;

    public class GenericAdminControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        static GenericAdminControllerFeatureProvider()
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
                    var controllerType = typeof(GenericAdminController<>)
                        .MakeGenericType(typeArgs)
                        .GetTypeInfo();
                    feature.Controllers.Add(controllerType);
                });
    }
}