namespace AutoCrudAdmin.ViewModels
{
    public class GridAction
    {
        private string name = default!;

        public string Action { get; set; } = default!;

        public string Name
        {
            get => this.name ??= this.Action;
            init => this.name = value;
        }
    }
}