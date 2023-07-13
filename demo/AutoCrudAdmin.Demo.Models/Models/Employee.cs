namespace AutoCrudAdmin.Demo.Models.Models;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Employee
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string Username { get; set; } = default!;

    [MaxLength(30)]
    public string Email { get; set; } = default!;

    [MaxLength(30)]
    public string Phone { get; set; } = default!;

    public ICollection<EmployeeTasks> EmployeeTasks { get; set; } = new HashSet<EmployeeTasks>();

    public override string ToString()
        => $"{this.Id}, {this.Username}";
}
