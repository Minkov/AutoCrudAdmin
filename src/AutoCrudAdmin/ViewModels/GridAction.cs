namespace AutoCrudAdmin.ViewModels
{
    public class GridAction
    {
        private string? name;

        public string Action { get; set; } = null!;

        public string Name
        {
            get => this.name ??= this.Action;
            init => this.name = value;
        }
    }
}