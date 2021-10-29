namespace GenericDotNetCoreAdmin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Task
    {
        [Key] public int Id { get; set; }

        public string Name { get; set; }

        public DateTime OpenDate { get; set; }

        public DateTime DueDate { get; set; }

        [Required] 
        public TaskExecutionType ExecutionType { get; set; }

        [Required] 
        public TaskLabelType LabelType { get; set; }

        public int ProjectId { get; set; }

        public Project Project { get; set; }
        
        public ICollection<EmployeeTasks> EmployeeTasks { get; set; }
    }
}