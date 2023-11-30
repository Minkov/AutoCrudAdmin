namespace AutoCrudAdmin.Extensions
{
    using System.Linq;

    /// <summary>
    /// The <see cref="EntityExtensions"/> class provides extension methods for the TEntity interface.
    /// These extensions add functionality related to the AutoCrudAdmin system.
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// The CopyFormPropertiesToExistingEntityFromNewEntity copies properties from one entity to another.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity that this method operates on.</typeparam>
        /// <param name="existingEntity">The entity that receives the copied properties.</param>
        /// <param name="newEntity">The entity that gets copied.</param>
        public static void CopyFormPropertiesToExistingEntityFromNewEntity<TEntity>(this TEntity existingEntity, TEntity newEntity)
        => typeof(TEntity)
        .GetProperties()
        .Where(p => p.CanWrite && !p.PropertyType.IsNavigationProperty())
        .ToList()
        .ForEach(property => property.SetValue(existingEntity, property.GetValue(newEntity, null), null));
    }
}
