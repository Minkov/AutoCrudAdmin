namespace AutoCrudAdmin.Extensions;

using AutoCrudAdmin.Enumerations;
using Microsoft.AspNetCore.Mvc;
using System;

public static class ControllerExtensions
{
    public static bool TryGetEntityIdForNumberColumnFilter(
        this Controller controller,
        string columnName,
        out int entityId,
        GridNumberFilterType numberFilterType = GridNumberFilterType.Equals)
        => TryGetEntityIdForColumnFilter(controller, columnName, out entityId, numberFilterType.ToString());

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