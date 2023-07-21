namespace AutoCrudAdmin.Enumerations;

/// <summary>
/// Defines the types of filters that can be applied to numerical data in a grid in the AutoCrudAdmin system.
/// </summary>
public enum GridNumberFilterType
{
    /// <summary>
    /// Represents an equals filter, where the grid data matches the filter value exactly.
    /// </summary>
    Equals = 1,

    /// <summary>
    /// Represents a not equals filter, where the grid data does not match the filter value.
    /// </summary>
    NotEquals = 22,

    /// <summary>
    /// Represents a less than filter, where the grid data is less than the filter value.
    /// </summary>
    LessThan = 3,

    /// <summary>
    /// Represents a greater than filter, where the grid data is greater than the filter value.
    /// </summary>
    GreaterThan = 4,

    /// <summary>
    /// Represents a less than or equal to filter, where the grid data is less than or equal to the filter value.
    /// </summary>
    LessThanOrEqual = 5,

    /// <summary>
    /// Represents a greater than or equal to filter, where the grid data is greater than or equal to the filter value.
    /// </summary>
    GreaterThanOrEqual = 6,
}