namespace AutoCrudAdmin.Demo.Models.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Project
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public DateTime OpenDate { get; set; }

    public DateTime DueDate { get; set; }

    public ICollection<Task> Tasks { get; set; } = new HashSet<Task>();

    public override string ToString()
        => $"{this.Id}, {this.Name}";
}