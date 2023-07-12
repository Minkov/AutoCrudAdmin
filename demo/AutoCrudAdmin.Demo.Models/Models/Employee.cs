namespace AutoCrudAdmin.Demo.Models.Models;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Employee
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string Username { get; set; }


    [MaxLength(30)]
    public string Email { get; set; }


    [MaxLength(30)]
    public string Phone { get; set; }

    public override string ToString()
        => $"{this.Id}, {this.Username}";
        
    public ICollection<EmployeeTasks> EmployeeTasks { get; set; }
}