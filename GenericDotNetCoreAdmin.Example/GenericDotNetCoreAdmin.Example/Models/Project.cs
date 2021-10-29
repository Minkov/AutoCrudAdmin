namespace GenericDotNetCoreAdmin.Example.Models
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
    }
}