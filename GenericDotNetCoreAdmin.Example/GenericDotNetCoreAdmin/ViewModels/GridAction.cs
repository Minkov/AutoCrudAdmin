namespace GenericDotNetCoreAdmin.ViewModels
{
    public class GridAction
    {
        private string name;

        public string Action { get; set; }

        public string Name
        {
            get => this.name ??= this.Action;
            init => this.name = value;
        }
    }
}