namespace AutoCrudAdmin.Extensions;

using AutoCrudAdmin.Enumerations;
using Microsoft.AspNetCore.Mvc;
using System;

/// <summary>
/// The <see cref="ControllerExtensions"/> class provides extension methods for the Controller class.
/// These extensions add functionality related to the AutoCrudAdmin system.
/// </summary>
public static class ControllerExtensions
{
    /// <summary>
    /// Tries to get the entity ID for a number column filter from the Controller.
    /// </summary>
    /// <param name="controller">The Controller instance that this method extends.</param>
    /// <param name="columnName">The name of the column for which the filter is being applied.</param>
    /// <param name="entityId">When this method returns, contains the entity ID associated with the specified column, if the column is found; otherwise, contains the default value.</param>
    /// <param name="numberFilterType">The type of filter to be applied to the column. If not provided, equals filter will be used.</param>
    /// <returns>true if the Controller contains an element with the specified column; otherwise, false.</returns>
    public static bool TryGetEntityIdForNumberColumnFilter(
        this Controller controller,
        string columnName,
        out int entityId,
        GridNumberFilterType numberFilterType = GridNumberFilterType.Equals)
        => TryGetEntityIdForColumnFilter(controller, columnName, out entityId, numberFilterType.ToString());

    /// <summary>
    /// Tries to get the entity ID for a string column filter from the Controller.
    /// </summary>
    /// <param name="controller">The Controller instance that this method extends.</param>
    /// <param name="columnName">The name of the column for which the filter is being applied.</param>
    /// <param name="entityId">When this method returns, contains the entity ID associated with the specified column, if the column is found; otherwise, contains the default value.</param>
    /// <param name="stringFilterType">The type of filter to be applied to the column. If not provided, equals filter will be used.</param>
    /// <returns>true if the Controller contains an element with the specified column; otherwise, false.</returns>
    public static bool TryGetEntityIdForStringColumnFilter(
        this Controller controller,
        string columnName,
        out string? entityId,
        GridStringFilterType stringFilterType = GridStringFilterType.Equals)
        => TryGetEntityIdForColumnFilter(controller, columnName, out entityId, stringFilterType.ToString());

    private static bool TryGetEntityIdForColumnFilter<TEntityId>(
        Controller controller,
        string columnName,
        out TEntityId? entityId,
        string filter)
    {
        object? id = null;
        string? idAsString;
        var isSuccessful = false;

        if (controller.TempData.TryGetValue(columnName, out var entityIdFromTempData))
        {
            idAsString = entityIdFromTempData?.ToString();
        }
        else if (controller.HttpContext.Request
            .TryGetQueryValueForColumnFilter(columnName, out idAsString, filter))
        {
        }

        if (typeof(TEntityId) == typeof(string) && !string.IsNullOrWhiteSpace(idAsString))
        {
            id = idAsString;
            isSuccessful = true;
        }
        else if (int.TryParse(idAsString, out var entityIdInt))
        {
            id = entityIdInt;
            isSuccessful = true;
        }

        entityId = id == default
            ? default
            : (TEntityId)Convert.ChangeType(id, typeof(TEntityId));
        return isSuccessful;
    }
}