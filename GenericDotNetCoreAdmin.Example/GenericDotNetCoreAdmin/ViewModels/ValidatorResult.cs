namespace GenericDotNetCoreAdmin.ViewModels
{
    public class ValidatorResult
    {
        public bool IsValid { get; set; }

        public string Message { get; set; }

        public static ValidatorResult Success()
            => new()
            {
                IsValid = true,
            };

        public static ValidatorResult Error(string message)
            => new()
            {
                IsValid = false,
                Message = message,
            };
    }
}