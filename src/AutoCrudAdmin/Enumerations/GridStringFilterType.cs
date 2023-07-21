namespace AutoCrudAdmin.Enumerations;

/// <summary>
/// Defines the types of filters that can be applied to string data in a grid in the AutoCrudAdmin system.
/// </summary>
public enum GridStringFilterType
{
    /// <summary>
    /// Represents an equals filter, where the grid data matches the filter value exactly.
    /// </summary>
    Equals = 1,

    /// <summary>
    /// Represents a not equals filter, where the grid data does not match the filter value.
    /// </summary>
    NotEquals = 2,

    /// <summary>
    /// Represents a contains filter, where the grid data contains the filter value.
    /// </summary>
    Contains = 3,

    /// <summary>
    /// Represents a starts with filter, where the grid data starts with the filter value.
    /// </summary>
    StartsWith = 4,

    /// <summary>
    /// Represents an ends with filter, where the grid data ends with the filter value.
    /// </summary>
    EndsWith = 5,
}