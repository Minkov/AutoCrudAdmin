
# The Grid page

For each entity in the `DbContext`, a page is created:
- {PROJECT_URL}/projects
- {PROJECT_URL}/tasks

For example, having the following:

<table>
<tr>
<th>Entities</th>
<th>DbContext</th>
</tr>
<tr>
<td>

```cs
public class Project
{
    [Key]
    public int Id { get; set; }
     
    public string Name { get; set; }
     
    public ICollection<Task> Tasks { get; set; }

    public override string ToString()
      => this.Name;
}

public class Task
{
   [Key]
   public string Id { get; set; }
   
   public string Name { get; set; }

   [Required]
   public DateTime DueDate { get; set; }

   [Required]
   public TaskExecutionType ExecutionType { get; set; }

   [Required]
   public int ProjectId { get; set; }

   public Project Project { get; set; }
}
```

</td>
<td>

```cs
AutoCrudGridExampleDbContext : DbContext
{
  // ... setup stuff
  public DbSet<Task> Tasks { get; set; }

  public DbSet<Project> Projects { get; set; }
}
```
<br/>
<br/>
<br/>
<br/>
<br/>
<br/>
<br/>
<br/>
<br/>
<br/>
<br/>
<br/>
<br/>
<br/>
<br/>
<br/>
<br/>
<br/>

</td>
</tr>
</table>
