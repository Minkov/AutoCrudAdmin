namespace AutoCrudAdmin.ViewModels
{
    /// <summary>
    /// A class that is used to indicate the result of the validation.
    /// </summary>
    public class ValidatorResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the validation is successful or not.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the message of the validation result.
        /// </summary>
        public string Message { get; set; } = null!;

        /// <summary>
        /// A method used when the validation is successful.
        /// </summary>
        /// <returns>A new validation result with a IsValid property set to true.</returns>
        public static ValidatorResult Success()
            => new ()
            {
                IsValid = true,
            };

        /// <summary>
        /// A method used when there is an error at validation.
        /// </summary>
        /// <param name="message">The error message of the validation.</param>
        /// <returns>A new validation result with IsValid set to false and the error message that caused the validation to fail.</returns>
        public static ValidatorResult Error(string message)
            => new ()
            {
                IsValid = false,
                Message = message,
            };
    }
}