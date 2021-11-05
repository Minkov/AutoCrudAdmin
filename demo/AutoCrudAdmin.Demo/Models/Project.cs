namespace AutoCrudAdmin.Demo.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Project
    {
        [Key]
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public DateTime OpenDate { get; set; }
        
        public DateTime DueDate { get; set; }
        
        public ICollection<Task> Tasks { get; set; }

        public override string ToString()
            => this.Name;
    }
}