namespace AutoCrudAdmin.Extensions;

using AutoCrudAdmin.Enumerations;
using Microsoft.AspNetCore.Mvc;

public static class ControllerExtensions
{
    public static bool TryGetEntityIdForColumnFilter(
        this Controller controller,
        string columnName,
        out int entityId,
        GridNumberFilterType numberFilterType = GridNumberFilterType.Equals)
    {
        if (controller.TempData.TryGetValue(columnName, out var entityIdFromTempData))
        {
            if (int.TryParse(entityIdFromTempData?.ToString(), out entityId))
            {
                return true;
            }
        }

        if (controller.HttpContext.Request
            .TryGetQueryValueForColumnFilter(columnName, out var entityIdFromQuery, numberFilterType))
        {
            if (int.TryParse(entityIdFromQuery, out entityId))
            {
                return true;
            }
        }

        entityId = default;
        return false;
    }
}