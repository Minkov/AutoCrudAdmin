namespace AutoCrudAdmin.Extensions;

using AutoCrudAdmin.Enumerations;
using Microsoft.AspNetCore.Mvc;
using System;

public static class ControllerExtensions
{
    public static bool TryGetEntityIdForColumnFilter<TEntityId>(
        this Controller controller,
        string columnName,
        out TEntityId? entityId,
        GridNumberFilterType numberFilterType = GridNumberFilterType.Equals)
    {
        object? id = null;
        string? idAsString;
        var isSuccessful = false;

        if (controller.TempData.TryGetValue(columnName, out var entityIdFromTempData))
        {
            idAsString = entityIdFromTempData?.ToString();
        }
        else if (controller.HttpContext.Request
            .TryGetQueryValueForColumnFilter(columnName, out idAsString, numberFilterType))
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