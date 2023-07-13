namespace AutoCrudAdmin.Demo.Models.Models;

public class EmployeeTasks
{
    public int EmployeeId { get; set; }

    public Employee Employee { get; set; } = default!;

    public int TaskId { get; set; }

    public Task Task { get; set; } = default!;
}