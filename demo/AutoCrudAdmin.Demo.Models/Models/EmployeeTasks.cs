namespace AutoCrudAdmin.Demo.Models.Models
{
    using System.ComponentModel.DataAnnotations;

    public class EmployeeTasks
    {
        public int EmployeeId { get; set; }
        
        public Employee Employee { get; set; }
        
        public int TaskId { get; set; }
        
        public Task Task { get; set; }
    }
}