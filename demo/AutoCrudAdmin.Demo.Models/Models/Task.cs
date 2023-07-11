namespace AutoCrudAdmin.Demo.Models.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Task
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        public DateTime OpenDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public TaskExecutionType ExecutionType { get; set; }

        [Required]
        public TaskLabelType LabelType { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public Project Project { get; set; }

        public ICollection<EmployeeTasks> EmployeeTasks { get; set; }

        public override string ToString()
            => $"{this.Id}, {this.Name}";
    }
}