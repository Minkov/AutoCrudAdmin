namespace GenericDotNetCoreAdmin.Attributes
{
    using System.Collections.Generic;
    using System.Reflection;
    using GenericDotNetCoreAdmin.Controllers;
    using GenericDotNetCoreAdmin.Models;
    using Microsoft.AspNetCore.Mvc.ApplicationParts;
    using Microsoft.AspNetCore.Mvc.Controllers;

    public class GenericAdminControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var types = new[] { typeof(Employee), typeof(EmployeeTasks), typeof(Project), typeof(Task) };
            foreach (var entityType in types)
            {
                var typeArgs = new[] { entityType };
                var controllerType = typeof(GenericAdminController<>)
                    .MakeGenericType(typeArgs)
                    .GetTypeInfo();
                feature.Controllers.Add(controllerType);
            }
        }
    }
}